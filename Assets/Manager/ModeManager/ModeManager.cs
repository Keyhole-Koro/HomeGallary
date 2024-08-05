using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState
{
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void HandleInput();
}

public class ModeManager : Singleton<ModeManager>
{
    private Stack<GameState> stateStack = new Stack<GameState>();

    public GameState CurrentState => stateStack.Count > 0 ? stateStack.Peek() : null;

    void Start()
    {
        PushState(new ViewMode());
    }

    public void PushState(GameState newState)
    {
        if (CurrentState != null)
        {
            CurrentState.ExitState();
        }
        stateStack.Push(newState);
        newState.EnterState();
    }

    public void PopState()
    {
        if (stateStack.Count > 0)
        {
            GameState stateToExit = stateStack.Pop();
            stateToExit.ExitState();

            if (stateStack.Count > 0)
            {
                stateStack.Peek().EnterState();
            }
        }
    }

    public void PopToState<T>() where T : GameState
    {
        while (stateStack.Count > 0 && !(stateStack.Peek() is T))
        {
            stateStack.Peek().ExitState();
            stateStack.Pop();
        }
        if (stateStack.Count > 0 && stateStack.Peek() is T)
        {
            stateStack.Peek().EnterState();
        }
    }

    private void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.UpdateState();
        }
    }
}
