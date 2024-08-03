using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class PlaceItem : Singleton<PlaceItem>
{
    private Camera ItemPlacementCamera;
    private float raycastDistance = 300f;
    private float moveSpeed = 20f;
    public Vector3 targetPosition;
    private float precision = 0.1f;
    private float minHeight = 0.5f;
    private GameObject pointerInstance;
    public LayerMask hitLayers;
    public float cameraRotationSpeed = 2f;
    public float cameraDistance = 5f;
    public float cameraMoveSpeed = 5f;

    private bool ifRayCastStarted = false;
    private ItemPlacementButton itemPlacementButton;

    EventSystem eventSystem;

    void Start()
    {
        targetPosition = transform.position;
        hitLayers = LayerMask.GetMask("RoomLayer");
        CreatePointer();

        itemPlacementButton = ItemPlacementButton.Instance.CreateUI();
    }

    void Update()
    {
        if (transform.position == targetPosition)
        {
            itemPlacementButton.SetVisible();
        }
        else
        {
            itemPlacementButton.SetInvisible();
        }

        if (IsInputActive())
        {
            ifRayCastStarted = true;

            if (eventSystem == null)
                eventSystem = ItemMenuEventSystem.Instance.GetEventSystem();

            // 2D ray
            Ray ray = ItemPlacementCamera.ScreenPointToRay(Input.mousePosition);

            PointerEventData pointerEventData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            eventSystem.RaycastAll(pointerEventData, results);

            if (results.Count > 0)
            {
                RaycastResult frontMostResult = GetFrontMostResult(results);
                if (frontMostResult.gameObject != null && frontMostResult.gameObject != null)
                {
                    if (frontMostResult.gameObject.name == ItemPlacementButton.Instance.buttonName)
                    {
                        InputManager.Instance.OnItemPlacementDoneButtonClicked();
                    }
                }
            }
            else if /*3D ray*/
            (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, hitLayers))
            {
                targetPosition = hit.point;
                targetPosition.y = Mathf.Max(targetPosition.y, minHeight);

                pointerInstance.transform.position = targetPosition;
                pointerInstance.SetActive(true);
            }
            else
            {
                pointerInstance.SetActive(false);
            }
        }
        else
        {
            pointerInstance.SetActive(false);
        }

        MoveTowards(targetPosition);

        if (ifRayCastStarted)
        {
            AlignItemToCamera();
            AdjustCameraView();
        }
    }

    private RaycastResult GetFrontMostResult(List<RaycastResult> results)
    {
        if (results.Count == 0)
        {
            return default;
        }

        RaycastResult frontMostResult = results[0];
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
        if (ItemPlacementCamera != null && pointerInstance != null)
        {
            Vector3 screenPoint = ItemPlacementCamera.WorldToViewportPoint(
                pointerInstance.transform.position
            );

            // Adjust the camera if the pointer is out of the camera view
            if (screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1)
            {
                Vector3 directionToPointer =
                    pointerInstance.transform.position - ItemPlacementCamera.transform.position;
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

    void CreatePointer()
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        Material redMaterial = new Material(Shader.Find("Standard")) { color = Color.red };
        sphere.GetComponent<Renderer>().material = redMaterial;
        sphere.SetActive(false);
        pointerInstance = sphere;
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
            transform.position = target;

            RoomData room = RoomManager.Instance.FindRoomContainingPoint(target);
            if (room != null)
            {
                AlignItemToFloor(room);
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
        if (attachingWall == null) /* skip for aligning*/
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

    bool IsInputActive()
    {
        return Input.GetMouseButton(0);
    }

    public void FinishItemPlacement()
    {
        ItemPlacementButton.Instance.DestroyButton();
        RoomData room = RoomManager.Instance.FindRoomContainingPoint(transform.position);

        if (room != null)
        {
            transform.SetParent(room.room.transform);
        }
        else
        {
            Debug.LogError(
                "Error: No room found for the item placement at the specified location."
            );
        }

        Destroy(this);
    }
}
