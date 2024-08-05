using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenItemMenuMode : GameState
{
    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public override void EnterState()
    {
        ScreenManager.Instance.ShowItemMenu();
        PlayerController.Instance.DisableMove();
        PlayerCameraController.Instance.SetCursorMode();
    }

    public override void UpdateState() { }

    public override void ExitState()
    {
        ScreenManager.Instance.HideItemMenu();
        PlayerController.Instance.EnableMove();
        PlayerCameraController.Instance.SetViewMode();
    }

    public override void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ModeManager.Instance.PopState();
        }
    }
}
