using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewMode : GameState
{
    public override void EnterState()
    {
        PlayerController.Instance.Setup();
        PlayerCameraController.Instance.Setup();
    }

    public override void UpdateState()
    {
        PlayerController.Instance.UpdatePlayer();
        PlayerCameraController.Instance.UpdatePlayerCamera();
    }

    public override void ExitState() { }

    public override void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ModeManager.Instance.PushState(new ItemMenuMode());
        }
    }
}
