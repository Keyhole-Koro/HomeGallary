using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    public float moveSpeed = 5f;
    private bool movable = true;
    private Rigidbody rb;

    void Start()
    {
        // Add Rigidbody if not already present
        rb = gameObject.AddComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Add and configure BoxCollider
        BoxCollider playerCollider = gameObject.AddComponent<BoxCollider>();
        playerCollider.size = new Vector3(1f, 1f, 1f);
    }

    void Update()
    {
        if (movable && CameraController.Instance != null)
        {
            Vector3 moveDirection = CameraController.Instance.GetCameraRotation();
            transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime, Space.World);
            transform.forward = CameraController.Instance.transform.forward;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Stop player movement on collision
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void EnableMove() => movable = true;

    public void DisableMove() => movable = false;

    public Vector3 GetPlayerPosition() => transform.position;
}
