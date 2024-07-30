using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    void Start() { }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.I))
        {
            ModeManager.Instance.On_I_KeyDown();
        }
    }

    public void OnItemInMenuClicked(ItemDataManager.ItemData item)
    {
        GameObject spawnedItem = SpawnItem.Instance.SpawnItemObject(item);
        ModeManager.Instance.TrunOffOpenItemMenuMode();
        string cameraName = "camera_" + item.id;
        GameObject cameraObject = CameraManager.Instance.CreateAndRegisterCamera(cameraName);

        cameraObject.transform.position = PlayerCameraController.Instance.GetCameraPosition();

        Quaternion currentRotation =
            PlayerCameraController.Instance.GetPlayerBodytransformRotation();

        Quaternion newRotation = new Quaternion(
            0f,
            currentRotation.y,
            currentRotation.z,
            currentRotation.w
        );

        // Set the new rotation
        cameraObject.transform.rotation = newRotation;

        // Switch to the newly created camera
        CameraManager.Instance.SwitchCamera(cameraName);

        PlayerCameraController.Instance.SetCursorMode();

        PlaceItem.Instance.UpdateCamera(cameraObject);
    }
}
