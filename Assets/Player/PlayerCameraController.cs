using UnityEngine;

public class PlayerCameraController : Singleton<PlayerCameraController>
{
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool isPlayMode = false;

    void Start()
    {
        // Set camera's initial position and lock cursor
        transform.position = PlayerController.Instance.transform.position + new Vector3(0f, 1f, 0f);
        SetCursorLockState(CursorLockMode.Locked);
    }

    void Update()
    {
        if (isPlayMode)
        {
            // Get mouse input for rotation
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // Update and clamp vertical rotation
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -30f, 30f);

            // Update horizontal rotation
            yRotation += mouseX;

            // Apply rotations and update camera position
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }

    public void SetViewMode()
    {
        isPlayMode = true;
        SetCursorLockState(CursorLockMode.Locked);
        Cursor.visible = false;
    }

    public void SetCursorMode()
    {
        isPlayMode = false;
        SetCursorLockState(CursorLockMode.None);
        Cursor.visible = true;
    }

    public Vector3 GetCameraRotation()
    {
        // Return the direction based on input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 cameraForward = transform.forward;
        cameraForward.y = 0f;

        Quaternion cameraRotation = Quaternion.LookRotation(cameraForward);
        return cameraRotation * new Vector3(horizontalInput, 0f, verticalInput);
    }

    public Vector3 GetCameraPosition() => transform.position;

    private void SetCursorLockState(CursorLockMode lockMode) => Cursor.lockState = lockMode;

    public float GetXRotation() => xRotation;

    public Quaternion GetPlayerBodytransformRotation() =>
        PlayerController.Instance.transform.rotation;
}
