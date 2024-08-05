using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewMode : GameState
{
    private bool isItemMenuOpened = false;

    public override void EnterState()
    {
        CameraManager.Instance.SwitchToPlayerCamera();
        PlayerController.Instance.EnableMove();
        PlayerCameraController.Instance.SetViewMode();
    }

    public override void UpdateState() { }

    public override void ExitState()
    {
        PlayerController.Instance.DisableMove();
    }

    public override void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ModeManager.Instance.PushState(new OpenItemMenuMode());
        }
    }
}
