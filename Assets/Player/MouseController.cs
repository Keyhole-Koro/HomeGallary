using UnityEngine;

public class MouseController : Singleton<MouseController>
{
    public float mouseSensitivity = 100f; // Sensitivity of mouse movement
    public Transform playerBody; // Transform of the player object

    private float xRotation = 0f; // Current rotation around the x-axis
    private bool isPlayMode = false;

    bool editable = false;

    void Start()
    {
        SetCursorLockState(CursorLockMode.Locked);
    }

    void Update()
    {
        if (!isPlayMode)
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

    public void SetPlayMode()
    {
        isPlayMode = true;
        SetCursorLockState(CursorLockMode.None);
        Cursor.visible = true;
    }

    public void SetSelectMode()
    {
        isPlayMode = false;
        SetCursorLockState(CursorLockMode.Locked);
        Cursor.visible = false;
    }

    void SetCursorLockState(CursorLockMode lockMode)
    {
        Cursor.lockState = lockMode;
    }
}
