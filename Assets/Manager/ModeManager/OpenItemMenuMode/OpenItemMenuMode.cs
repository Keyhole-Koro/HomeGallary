using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenItemMenuMode : Singleton<OpenItemMenuMode>
{
    bool editable = false;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public void TurnOn()
    {
        ScreenManager.Instance.ShowItemMenu();
        CameraController.Instance.SetPlayMode();
        PlayerController.Instance.DisableMove();
    }

    public void TurnOff()
    {
        ScreenManager.Instance.HideItemMenu();
        CameraController.Instance.SetSelectMode();
        PlayerController.Instance.EnableMove();
    }
}
