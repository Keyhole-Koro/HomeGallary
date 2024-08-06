using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMenuMode : GameState
{
    public float raycastDistance = 300f;
    private LayerMask hitLayers = LayerMask.GetMask("SpawnedItemLayer");

    public override void EnterState()
    {
        ScreenManager.Instance.ShowItemMenu();
        PlayerCameraController.Instance.SetCursorMode();
    }

    public override void UpdateState() { }

    public override void ExitState()
    {
        ScreenManager.Instance.HideItemMenu();
        PlayerCameraController.Instance.SetViewMode();
    }

    public override void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ModeManager.Instance.PopState();
        }

        if (InputUtils.OnLeftMouseButtonDown())
        {
            Ray ray = CameraManager.Instance
                .GetCurrentCamera()
                .ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, hitLayers))
            {
                ModeManager.Instance.PushState(new ItemPlacementMode(hit.collider.gameObject));
            }
        }
    }
}
