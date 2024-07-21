using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Camera playerCamera; // Reference to the camera that is a child of the player

    void Start()
    {
        transform.position = new Vector3(0f, 1f, 0f); // Set initial position

        // Get the Camera component from the child objects
        playerCamera = GetComponentInChildren<Camera>();

        if (playerCamera == null)
        {
            Debug.LogError("No camera found as a child of the player!");
        }

        BoxCollider playerCollider = gameObject.AddComponent<BoxCollider>();
        playerCollider.size = new Vector3(1f, 1f, 1f);
    }

    void Update()
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
            transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime, Space.World);

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

    void OnCollisionEnter(Collision collision)
    {
        //        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor"))
        //        {
        // プレイヤーの速度をゼロに設定して、動きを停止させる
        print("entered");
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        //        }
    }
}
