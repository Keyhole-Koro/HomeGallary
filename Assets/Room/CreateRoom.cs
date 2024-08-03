using UnityEngine;

public class CreateRoom : MonoBehaviour
{
    void Start()
    {
        RoomManager.Instance.CreateRoom("room", 5, new Vector3(5, 0, 5), 10f, 0.1f, 4f);
    }
}
