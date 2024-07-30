using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenItemMenuMode : Singleton<OpenItemMenuMode>
{
    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public void TurnOn()
    {
        ScreenManager.Instance.ShowItemMenu();
        PlayerController.Instance.DisableMove();
        PlayerCameraController.Instance.SetCursorMode();
    }

    public void TurnOff()
    {
        ScreenManager.Instance.HideItemMenu();
        PlayerController.Instance.EnableMove();
        PlayerCameraController.Instance.SetViewMode();
    }
}
