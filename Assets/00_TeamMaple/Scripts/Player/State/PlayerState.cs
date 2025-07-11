using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Serialization;

public abstract class PlayerState : MonoBehaviour, IPlayerState
{
    public PlayerController PlayerController { get; set; }
    public LocalPlayer LocalPlayer {get; private set;}
    
    protected MapRangeSetter mapSetter;
    
    [Header("PlayerState Settings")]
    [SerializeField] private IPlayerState.EState state;

    public IPlayerState.EState State => state;
    
    protected void Start()
    {
        LocalPlayer = LocalPlayer.Instance;
        mapSetter = PlaySystemRefStorage.mapSetter;
    }

    protected void Update()
    {
        if (PlayerController == null || 
            LocalPlayer == null || 
            mapSetter == null) return;
        
        StateUpdate();
    }

    public abstract void StateEnter();
    
    protected abstract void StateUpdate();
    
    public abstract void StateExit();
}
