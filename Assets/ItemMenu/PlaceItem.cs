using UnityEngine;

public class PlaceItem : Singleton<PlaceItem>
{
    private Camera camera;
    private float raycastDistance = 300f; // Maximum distance for raycast
    private float moveSpeed = 20f; // Speed at which the item moves towards the target position
    private Vector3 targetPosition; // Current target position
    private float precision = 0.1f; // Precision (tolerance range) for reaching the target
    private float minHeight = 0.5f; // Minimum height to maintain above the ground

    private GameObject pointerInstance; // Instance of the pointer

    public LayerMask hitLayers; // LayerMask to filter hit objects
    public float cameraRotationSpeed = 2f; // Speed at which the camera rotates to follow the pointer
    public float cameraDistance = 5f; // Distance to maintain between the camera and the item
    public float cameraMoveSpeed = 5f; // Speed at which the camera moves towards the item

    bool ifRayCastStarted = false;

    void Start()
    {
        // Initialize the target position to the item's current position
        targetPosition = transform.position;

        // Set LayerMask to only include the "RoomLayer" layer
        hitLayers = LayerMask.GetMask("RoomLayer");

        // Create the pointer prefab dynamically
        CreatePointer();
    }

    void Update()
    {
        // Check for input and perform raycast only if the input is detected
        if (IsInputActive())
        {
            // Cast a ray from the camera to the mouse or touch position
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            ifRayCastStarted = true;

            // Check if the ray hits any surface with the specified layers
            if (Physics.Raycast(ray, out hit, raycastDistance, hitLayers))
            {
                // Update the target position to the hit point
                targetPosition = hit.point;
                targetPosition.y = Mathf.Max(targetPosition.y, minHeight); // Ensure the item is above the floor

                // Set the pointer's position to the hit point
                pointerInstance.transform.position = targetPosition;
                pointerInstance.SetActive(true);
            }
            else
            {
                // Hide the pointer if no hit occurs
                pointerInstance.SetActive(false);
            }
        }
        else
        {
            // Hide the pointer when input is not active
            pointerInstance.SetActive(false);
        }

        // Move the item towards the target position
        MoveTowards(targetPosition);

        // Adjust camera position and rotation
        if (ifRayCastStarted)
        {
            AdjustCameraView();
        }
    }

    public void UpdateCamera(GameObject cameraObject)
    {
        // Set the camera from the provided game object
        camera = cameraObject.GetComponent<Camera>();

        if (camera == null)
        {
            Debug.LogError("Provided GameObject does not contain a Camera component.");
        }
    }

    void CreatePointer()
    {
        // Create a sphere and set it as the pointer prefab
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        // Set the size of the sphere
        sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        // Apply a red material to the sphere
        Material redMaterial = new Material(Shader.Find("Standard")) { color = Color.red };
        sphere.GetComponent<Renderer>().material = redMaterial;

        // Hide the pointer initially
        sphere.SetActive(false);

        // Assign the pointer instance
        pointerInstance = sphere;
    }

    void MoveTowards(Vector3 target)
    {
        // Calculate the distance between the item and the target position
        float distance = Vector3.Distance(transform.position, target);

        // Move the item towards the target position with a constant speed
        if (distance > precision)
        {
            // Calculate the direction to the target position
            Vector3 direction = (target - transform.position).normalized;
            // Move the item towards the target position
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
        else
        {
            // Snap the item to the target position to avoid precision issues
            transform.position = target;

            // Calculate the target position for the camera
            Vector3 cameraTargetPosition = transform.position - transform.forward * cameraDistance;

            // Perform a raycast from the item to the target camera position to check for obstructions
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
                // Adjust the camera position to just before hitting the obstruction
                cameraTargetPosition = hit.point;
            }

            // Smoothly move the camera to the target position
            camera.transform.position = Vector3.Lerp(
                camera.transform.position,
                cameraTargetPosition,
                cameraMoveSpeed * Time.deltaTime
            );
        }
    }

    void AdjustCameraView()
    {
        if (camera != null && pointerInstance != null)
        {
            // Check if the pointer is outside the camera's view frustum
            Vector3 screenPoint = camera.WorldToViewportPoint(pointerInstance.transform.position);

            // If the pointer is out of the camera's view, rotate the camera
            if (screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1)
            {
                // Calculate the direction to rotate the camera
                Vector3 directionToPointer =
                    pointerInstance.transform.position - camera.transform.position;

                // Calculate the desired rotation
                Quaternion targetRotation = Quaternion.LookRotation(directionToPointer, Vector3.up);

                // Smoothly rotate the camera to face the pointer while limiting the vertical rotation
                Quaternion currentRotation = camera.transform.rotation;
                Quaternion newRotation = Quaternion.Slerp(
                    currentRotation,
                    targetRotation,
                    Time.deltaTime * cameraRotationSpeed
                );

                // Keep the camera's pitch angle within reasonable bounds
                Vector3 eulerAngles = newRotation.eulerAngles;
                eulerAngles.x = Mathf.Clamp(eulerAngles.x, 10f, 80f); // Restrict vertical rotation
                newRotation = Quaternion.Euler(eulerAngles);

                // Apply the rotation to the camera
                camera.transform.rotation = newRotation;
            }
        }
    }

    bool IsInputActive()
    {
        // Check if mouse button is pressed or touch is active
        return Input.GetMouseButton(0)
            || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary);
    }
}
