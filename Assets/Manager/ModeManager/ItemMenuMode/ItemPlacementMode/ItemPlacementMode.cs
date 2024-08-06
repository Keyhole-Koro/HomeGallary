using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ItemPlacementMode : GameState
{
    private ItemData item;
    private GameObject targetItem;
    private GameObject cameraFollowing;
    private Camera camera;
    private PlaceItem placeItemInstance;
    private EventSystem eventSystem;

    // Spawn a new object in this senario
    public ItemPlacementMode(ItemData item)
    {
        this.item = item;
        targetItem = SpawnItem.Instance.SpawnItemObject(item);
    }

    // For replacement
    public ItemPlacementMode(GameObject targetItem)
    {
        this.item = targetItem.GetComponent<ItemDataComponent>().itemData;
        this.targetItem = targetItem;
    }

    public override void EnterState()
    {
        // Set up camera follows to the target item
        string cameraName = "camera_" + item.id;
        cameraFollowing = CameraManager.Instance.CreateAndRegisterCamera(cameraName);
        camera = cameraFollowing.GetComponent<Camera>();
        cameraFollowing.transform.position = PlayerCameraController.Instance.GetCameraPosition();
        Quaternion currentRotation =
            PlayerCameraController.Instance.GetPlayerBodytransformRotation();
        Quaternion newRotation = new Quaternion(0f, currentRotation.y, 0f, currentRotation.w);
        cameraFollowing.transform.rotation = newRotation;
        CameraManager.Instance.SwitchCamera(cameraName);

        PlayerCameraController.Instance.SetCursorMode();

        placeItemInstance = targetItem.AddComponent<PlaceItem>();

        placeItemInstance.UpdateCamera(cameraFollowing);

        placeItemInstance.StartPlaceItem();
    }

    public override void UpdateState()
    {
        placeItemInstance.UpdatePlaceItem();
    }

    public override void ExitState()
    {
        placeItemInstance.FinishItemPlacement();
    }

    public override void HandleInput()
    {
        if (InputUtils.OnLeftMouseButtonDown())
        {
            placeItemInstance.ifRayCastStarted = true;

            if (eventSystem == null)
                eventSystem = ItemMenuEventSystem.Instance.GetEventSystem();

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            // 2D ray
            PointerEventData pointerEventData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            eventSystem.RaycastAll(pointerEventData, results);

            if (results.Count > 0)
            {
                RaycastResult frontMostResult = placeItemInstance.GetFrontMostResult(results);
                if (frontMostResult.gameObject != null)
                {
                    if (frontMostResult.gameObject.name == ItemPlacementButton.Instance.buttonName)
                    {
                        InputManager.Instance.OnItemPlacementDoneButtonClicked();
                    }
                }
            }
            /* 3D ray*/
            else if (
                Physics.Raycast(
                    ray,
                    out RaycastHit hit,
                    placeItemInstance.raycastDistance,
                    placeItemInstance.hitLayers
                )
            )
            {
                placeItemInstance.UpdateTargetPosition(hit.point);
            }
        }
    }
}
