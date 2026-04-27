using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> States = new();
    protected bool IsTransitioningState = false;
    public BaseState<EState> CurrentState { get; protected set; }

    protected virtual void Start()
    {
        CurrentState.EnterState();
    }

    protected virtual void Update()
    {
        EState nextStateKey = CurrentState.GetNextState();

        if (IsTransitioningState) return;

        if (nextStateKey.Equals(CurrentState.StateKey)) CurrentState.UpdateState();
        else TransitionToState(nextStateKey);
        // if (!nextStateKey.Equals(CurrentState.StateKey)) TransitionToState(nextStateKey);
    }

    public void TransitionToState(EState stateKey)
    {
        IsTransitioningState = true;
        CurrentState.ExitState();
        CurrentState = States[stateKey];
        CurrentState.EnterState();
        IsTransitioningState = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        CurrentState.OnTriggerEnter(other);
    }
}
