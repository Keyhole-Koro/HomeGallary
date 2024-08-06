using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class PlaceItem : Singleton<PlaceItem>
{
    private Camera ItemPlacementCamera;
    public float raycastDistance = 300f;
    private float moveSpeed = 20f;
    public Vector3 targetPosition;
    private float precision = 0.1f;
    private float minHeight = 0.5f;
    public LayerMask hitLayers;
    public float cameraRotationSpeed = 2f;
    public float cameraDistance = 5f;
    public float cameraMoveSpeed = 5f;

    public bool ifRayCastStarted = false;
    private ItemPlacementButton itemPlacementButton;
    private RoomData lastRoom;

    public void StartPlaceItem()
    {
        targetPosition = transform.position;
        hitLayers = LayerMask.GetMask("RoomLayer");

        itemPlacementButton = ItemPlacementButton.Instance.CreateUI();
    }

    public void UpdatePlaceItem()
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance <= 0.1f)
        {
            itemPlacementButton.SetVisible();
        }
        else
        {
            itemPlacementButton.SetInvisible();
        }

        MoveTowards(targetPosition);

        if (ifRayCastStarted)
        {
            AlignItemToCamera();
            AdjustCameraView();
        }
    }

    public void UpdateTargetPosition(Vector3 position)
    {
        targetPosition = position;
        targetPosition.y = Mathf.Max(targetPosition.y, minHeight);
    }

    public RaycastResult GetFrontMostResult(List<RaycastResult> results)
    {
        RaycastResult frontMostResult = InputUtils.GetFrontMostResult(results);
        float minDistance = frontMostResult.distance;

        foreach (var result in results)
        {
            if (result.distance < minDistance)
            {
                minDistance = result.distance;
                frontMostResult = result;
            }
        }

        return frontMostResult;
    }

    void AlignItemToCamera()
    {
        if (ItemPlacementCamera != null)
        {
            Vector3 directionToCamera = transform.position - ItemPlacementCamera.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera, Vector3.up);
            ItemPlacementCamera.transform.rotation = targetRotation;
        }
    }

    void AdjustCameraView()
    {
        if (ItemPlacementCamera != null)
        {
            Vector3 screenPoint = ItemPlacementCamera.WorldToViewportPoint(transform.position);

            // Adjust the camera if the item is out of the camera view
            if (screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1)
            {
                Vector3 directionToPointer =
                    transform.position - ItemPlacementCamera.transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(directionToPointer, Vector3.up);
                ItemPlacementCamera.transform.rotation = Quaternion.Slerp(
                    ItemPlacementCamera.transform.rotation,
                    targetRotation,
                    Time.deltaTime * cameraRotationSpeed
                );
            }
        }
    }

    public void UpdateCamera(GameObject cameraObject)
    {
        ItemPlacementCamera = cameraObject.GetComponent<Camera>();

        if (ItemPlacementCamera == null)
        {
            Debug.LogError("The provided GameObject does not contain a Camera component.");
        }
    }

    void MoveTowards(Vector3 target)
    {
        float distance = Vector3.Distance(transform.position, target);

        if (distance > precision)
        {
            Vector3 direction = (target - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
        else
        {
            RoomData room = RoomManager.Instance.FindRoomContainingPoint(target);
            if (room != null)
            {
                transform.position = target;
                AlignItemToFloor(room);
                lastRoom = room;
            }
            else
            {
                AlignItemToFloor(lastRoom);
            }

            Vector3 cameraTargetPosition = transform.position - transform.forward * cameraDistance;
            RaycastHit hit;
            if (
                Physics.Raycast(
                    transform.position,
                    cameraTargetPosition - transform.position,
                    out hit,
                    cameraDistance,
                    hitLayers
                )
            )
            {
                cameraTargetPosition = hit.point;
            }

            ItemPlacementCamera.transform.position = Vector3.Lerp(
                ItemPlacementCamera.transform.position,
                cameraTargetPosition,
                cameraMoveSpeed * Time.deltaTime
            );
        }
    }

    void AlignItemToFloor(RoomData room)
    {
        GameObject attachingWall = room.findAttachingToWall(transform.position);
        if (attachingWall == null) // skip for aligning
        {
            return;
        }

        Vector3 floorCenter = room.GetFloorCenter();
        Vector3 wallDirection = attachingWall.transform.forward;
        Quaternion targetRotation = Quaternion.LookRotation(wallDirection, Vector3.up);

        // Add +90 degrees rotation around the y-axis
        Quaternion additionalRotation = Quaternion.Euler(0, 90, 0);
        Quaternion finalRotation = targetRotation * additionalRotation;

        transform.rotation = finalRotation;
    }

    Vector3 GetWallDirectionForPosition(Vector3 position)
    {
        GameObject closestWall = FindClosestWall(position);
        if (closestWall != null)
        {
            return closestWall.transform.forward;
        }
        return Vector3.forward;
    }

    GameObject FindClosestWall(Vector3 position)
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        GameObject closestWall = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject wall in walls)
        {
            float distance = Vector3.Distance(position, wall.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestWall = wall;
            }
        }

        return closestWall;
    }

    GameObject FindAttatchingWall(Vector3 position)
    {
        foreach (GameObject room in GameObject.FindGameObjectsWithTag("Room"))
        {
            if (IsPositionInsideRoom(position, room))
            {
                return room;
            }
        }
        return null;
    }

    bool IsPositionInsideRoom(Vector3 position, GameObject room)
    {
        Collider roomCollider = room.GetComponent<Collider>();
        if (roomCollider != null)
        {
            return Physics.CheckBox(position, Vector3.one * 0.1f, Quaternion.identity, hitLayers);
        }
        return false;
    }

    public void FinishItemPlacement()
    {
        ItemPlacementButton.Instance.DestroyButton();
        CameraManager.Instance.DestroyAndLeaveCurrentCamera();
        RoomData room = RoomManager.Instance.FindRoomContainingPoint(transform.position);

        if (room != null)
        {
            // If a room is found, set the parent to this room
            transform.SetParent(room.room.transform);
        }
        else if (lastRoom != null)
        {
            // If no current room is found but there's a last room, set parent to the last room
            transform.SetParent(lastRoom.room.transform);
        }
        else
        {
            // If neither current room nor last room is found, log an error
            Debug.LogError(
                "Error: No room found for the item placement at the specified location."
            );
        }

        Destroy(this);
    }
}
