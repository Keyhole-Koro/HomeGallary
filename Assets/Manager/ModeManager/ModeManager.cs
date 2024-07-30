using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeManager : Singleton<ModeManager>
{
    bool isOpenItemMenuMode = false;

    void Start()
    {
        ViewMode.Instance.TurnOn();
    }

    // Update is called once per frame
    void Update() { }

    public void On_I_KeyDown()
    {
        isOpenItemMenuMode = !isOpenItemMenuMode;
        if (isOpenItemMenuMode)
        {
            OpenItemMenuMode.Instance.TurnOn();
        }
        else
        {
            OpenItemMenuMode.Instance.TurnOff();
        }
    }

    public void TrunOffOpenItemMenuMode()
    {
        OpenItemMenuMode.Instance.TurnOff();
        isOpenItemMenuMode = false;
    }
}
