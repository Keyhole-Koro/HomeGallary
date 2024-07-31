using UnityEngine;

public class CreateRoom : MonoBehaviour
{
    void Start()
    {
        RoomManager.Instance.CreateRoom("room", 5);
    }
}
