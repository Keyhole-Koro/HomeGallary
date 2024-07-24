using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Mode
{
    VIEW,
    EDIT,
};

public class ModeManager : Singleton<ModeManager>
{
    bool editable = false;

    void Start() { }

    // Update is called once per frame
    void Update() { }

    public void On_I_KeyDown()
    {
        EditMode.Instance.TurnOn();
    }
}
