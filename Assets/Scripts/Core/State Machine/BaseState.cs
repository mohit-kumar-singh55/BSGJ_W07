using System;
using UnityEngine;

public abstract class BaseState<EState> where EState : Enum         // EState is Generic of Enum Type
{
    public BaseState(EState key)
    {
        StateKey = key;
    }

    public EState StateKey { get; private set; }

    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();
    public abstract EState GetNextState();
    public virtual void OnTriggerEnter(Collider other) { }
}
