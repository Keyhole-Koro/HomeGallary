using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPlacementMode : GameState
{
    private ItemData item;
    private GameObject targetItem;
    private PlaceItem placeItemInstance;

    // Spawn a new object in this senario
    public ItemPlacementMode(ItemData item)
    {
        this.item = item;
    }

    // For replacement
    public ItemPlacementMode(GameObject targetItem)
    {
        this.targetItem = targetItem;
    }

    public override void EnterState()
    {
        if (!targetItem && item != null)
        {
            targetItem = SpawnItem.Instance.SpawnItemObject(item);
        }
        string cameraName = "camera_" + item.id;
        GameObject cameraObject = CameraManager.Instance.CreateAndRegisterCamera(cameraName);
        cameraObject.transform.position = PlayerCameraController.Instance.GetCameraPosition();
        Quaternion currentRotation =
            PlayerCameraController.Instance.GetPlayerBodytransformRotation();
        Quaternion newRotation = new Quaternion(0f, currentRotation.y, 0f, currentRotation.w);
        cameraObject.transform.rotation = newRotation;
        CameraManager.Instance.SwitchCamera(cameraName);

        PlayerCameraController.Instance.SetCursorMode();

        placeItemInstance = targetItem.AddComponent<PlaceItem>();

        placeItemInstance.UpdateCamera(cameraObject);

        placeItemInstance.StartPlacceItem();
    }

    public override void UpdateState() { }

    public override void ExitState()
    {
        placeItemInstance.FinishItemPlacement();
    }

    public override void HandleInput()
    {
        placeItemInstance.UpdatePlaceItem();
    }
}
