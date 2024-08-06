using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    private Vector3 playerPosition = new Vector3(0, 0, 0);
    public float moveSpeed = 5f;
    public Rigidbody rb;

    public void Setup()
    {
        transform.position = playerPosition;
        if (!rb)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.freezeRotation = true;
        }
        if (!gameObject.GetComponent<BoxCollider>())
        {
            BoxCollider playerCollider = gameObject.AddComponent<BoxCollider>();
            playerCollider.size = new Vector3(1f, 1f, 1f);
        }
    }

    public void UpdatePlayer()
    {
        Vector3 moveDirection = PlayerCameraController.Instance.GetCameraRotation();
        transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime, Space.World);
        transform.forward = PlayerCameraController.Instance.transform.forward;
        playerPosition = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public Vector3 GetPlayerPosition() => transform.position;
}
