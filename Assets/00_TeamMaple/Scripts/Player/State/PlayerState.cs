using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Serialization;

public abstract class PlayerState : MonoBehaviour, IPlayerState
{
    public PlayerController PlayerController { get; set; }

    [Header("PlayerState Settings")]
    [SerializeField] private IPlayerState.EState state;

    public IPlayerState.EState State => state;

    protected void Update()
    {
        StateUpdate();
    }

    public abstract void StateEnter();
    
    protected abstract void StateUpdate();
    
    public abstract void StateExit();
}
