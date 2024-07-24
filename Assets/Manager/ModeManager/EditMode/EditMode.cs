using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditMode : Singleton<EditMode>
{
    bool editable = false;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public void TurnOn()
    {
        editable = !editable;
        if (editable)
        {
            ScreenManager.Instance.ShowItemMenu();
            MouseController.Instance.SetPlayMode();
            PlayerController.Instance.DisableMove();
        }
        else
        {
            ScreenManager.Instance.HideItemMenu();
            MouseController.Instance.SetSelectMode();
            PlayerController.Instance.EnableMove();
        }
    }
}
