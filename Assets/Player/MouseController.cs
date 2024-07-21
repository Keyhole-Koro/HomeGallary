using UnityEngine;

public class MouseController : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Sensitivity of mouse movement
    public Transform playerBody; // Transform of the player object

    private float xRotation = 0f; // Current rotation around the x-axis

    void Start()
    {
        // Lock and hide the cursor (if needed)
        Cursor.lockState = CursorLockMode.Locked;

        // Set the camera to face forward initially
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -30f, 30f); // Limit rotation angle in the vertical direction

        // Rotate the camera up/down based on mouseY input, clamped within -30 to 30 degrees
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate the player left/right based on mouseX input
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
