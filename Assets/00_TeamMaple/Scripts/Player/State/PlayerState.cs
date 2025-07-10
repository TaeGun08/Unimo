using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Serialization;

public abstract class PlayerState : MonoBehaviour, IPlayerState
{
    public PlayerController PlayerController { get; set; }
    public LocalPlayer LocalPlayer {get; private set;}
    
    [Header("PlayerState Settings")]
    [SerializeField] private IPlayerState.EState state;

    public IPlayerState.EState State => state;

    protected void Start()
    {
        // 여기서 초기화 해주면 null뜨는 듯 ( ? )
        LocalPlayer = LocalPlayer.Instance;
    }

    protected void Update()
    {
        StateUpdate();
    }

    public abstract void StateEnter();
    
    protected abstract void StateUpdate();
    
    public abstract void StateExit();
}
