using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    void Start() { }

    void Update()
    {
        GameState currentState = ModeManager.Instance.CurrentState;
        if (currentState != null)
        {
            currentState.HandleInput();
        }
    }

    public void OnItemInMenuClicked(ItemData item)
    {
        ModeManager.Instance.PushState(new ItemPlacementMode(item));
    }

    public void OnItemPlacementDoneButtonClicked()
    {
        ModeManager.Instance.PopState();
    }
}
