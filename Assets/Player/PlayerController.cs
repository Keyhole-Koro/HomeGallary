using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    public float moveSpeed = 5f;

    bool movable = true;

    private Camera playerCamera; // Reference to the camera that is a child of the player

    Rigidbody rb;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>(); // Get the camera from the child objects

        if (playerCamera == null)
        {
            Debug.LogError("No camera found as a child of the player!");
        }

        // Add and get Rigidbody component
        rb = gameObject.AddComponent<Rigidbody>();
        rb.freezeRotation = true; // Freeze rotation (if needed)
        rb.useGravity = true; // Enable gravity (if needed)

        BoxCollider playerCollider = gameObject.AddComponent<BoxCollider>();
        playerCollider.size = new Vector3(1f, 1f, 1f);

        playerCamera.transform.localPosition = new Vector3(0f, 1f, 0f);
    }

    void Update()
    {
        if (movable)
        {
            // Check if playerCamera is not null before using it
            if (playerCamera != null)
            {
                // Get keyboard input
                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");

                // Calculate movement direction relative to the camera's forward direction
                Vector3 cameraForward = playerCamera.transform.forward; // Get camera's forward direction
                cameraForward.y = 0f; // Limit to horizontal direction

                // Calculate player's movement direction relative to the camera's orientation
                Quaternion cameraRotation = Quaternion.LookRotation(cameraForward);
                Vector3 moveDirection =
                    cameraRotation * new Vector3(horizontalInput, 0f, verticalInput);

                // Normalize movement direction and apply speed to move
                transform.Translate(
                    moveDirection.normalized * moveSpeed * Time.deltaTime,
                    Space.World
                );

                // Instantiate a cube at the player's position when space key is pressed
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Instantiate(Resources.Load("Cube"), transform.position, Quaternion.identity);
                }
            }
            else
            {
                Debug.LogError("Player camera is not assigned or found!");
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Reset player's velocity and angular velocity on collision
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    public void EnableMove()
    {
        movable = true;
    }

    public void DisableMove()
    {
        movable = false;
    }

    // Function to get the player's position
    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    public Vector3 GetGlobalCameraPosition()
    {
        return GetPlayerPosition() + playerCamera.transform.localPosition;
    }
}
