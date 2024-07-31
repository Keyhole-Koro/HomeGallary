using UnityEngine;

public class PlaceItem : Singleton<PlaceItem>
{
    private Camera camera;
    private float raycastDistance = 300f;
    private float moveSpeed = 20f;
    private Vector3 targetPosition;
    private float precision = 0.1f;
    private float minHeight = 0.5f;
    private float wallDetectionDistance = 1.0f;

    private GameObject pointerInstance;

    public LayerMask hitLayers;
    public float cameraRotationSpeed = 2f;
    public float cameraDistance = 5f;
    public float cameraMoveSpeed = 5f;

    private bool ifRayCastStarted = false;

    void Start()
    {
        targetPosition = transform.position;
        hitLayers = LayerMask.GetMask("RoomLayer");
        CreatePointer();
    }

    void Update()
    {
        // Check if input is active (mouse or touch input)
        if (IsInputActive())
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            ifRayCastStarted = true;

            if (Physics.Raycast(ray, out hit, raycastDistance, hitLayers))
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

    void AlignItemToCamera()
    {
        if (camera != null)
        {
            Vector3 directionToCamera = transform.position - camera.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera, Vector3.up);
            camera.transform.rotation = targetRotation;
        }
    }

    void AdjustCameraView()
    {
        if (camera != null && pointerInstance != null)
        {
            Vector3 screenPoint = camera.WorldToViewportPoint(pointerInstance.transform.position);

            // If the pointer is out of the camera view, adjust the camera
            if (screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1)
            {
                Vector3 directionToPointer =
                    pointerInstance.transform.position - camera.transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(directionToPointer, Vector3.up);
                camera.transform.rotation = Quaternion.Slerp(
                    camera.transform.rotation,
                    targetRotation,
                    Time.deltaTime * cameraRotationSpeed
                );
            }
        }
    }

    public void UpdateCamera(GameObject cameraObject)
    {
        camera = cameraObject.GetComponent<Camera>();

        if (camera == null)
        {
            Debug.LogError("Provided GameObject does not contain a Camera component.");
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

            GameObject room = FindRoomContainingTarget(target);
            if (room != null)
            {
                Vector3 floorCenter = RoomManager.Instance.GetFloorCenter(room);
                AlignItemToFloor(floorCenter);
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

            camera.transform.position = Vector3.Lerp(
                camera.transform.position,
                cameraTargetPosition,
                cameraMoveSpeed * Time.deltaTime
            );
        }
    }

    void AlignItemToFloor(Vector3 floorCenter)
    {
        Vector3 wallDirection = GetWallDirectionForPosition(transform.position);
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

    GameObject FindRoomContainingTarget(Vector3 position)
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
        return Input.GetMouseButton(0)
            || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary);
    }

    bool IsTouchingWall(Vector3 position)
    {
        RaycastHit hit;
        return Physics.Raycast(position, Vector3.forward, out hit, wallDetectionDistance, hitLayers)
            || Physics.Raycast(position, Vector3.back, out hit, wallDetectionDistance, hitLayers)
            || Physics.Raycast(position, Vector3.left, out hit, wallDetectionDistance, hitLayers)
            || Physics.Raycast(position, Vector3.right, out hit, wallDetectionDistance, hitLayers);
    }
}
