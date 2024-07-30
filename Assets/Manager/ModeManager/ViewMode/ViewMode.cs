using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewMode : Singleton<ViewMode>
{
    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public void TurnOn()
    {
        CameraManager.Instance.SwitchToPlayerCamera();
        PlayerController.Instance.EnableMove();
        PlayerCameraController.Instance.SetViewMode();
    }

    public void TurnOff()
    {
        PlayerController.Instance.DisableMove();
    }
}
