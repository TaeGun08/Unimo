using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("PlayerStates")]
    [SerializeField] private PlayerState[] playerSates;
    
    public Dictionary<IPlayerState.EState, PlayerState> StatesDic { get; private set; }
    public PlayerState CurrentState { get; set; }

    private void Awake()
    {
        StatesDic = new Dictionary<IPlayerState.EState, PlayerState>();
        
        foreach (PlayerState playerSate in playerSates)
        {
            playerSate.PlayerController = this;
            StatesDic.Add(playerSate.State, playerSate);
            playerSate.gameObject.SetActive(false);
        }
        
        CurrentState = StatesDic[IPlayerState.EState.Idle];
        CurrentState.gameObject.SetActive(true);
    }

    public void ChangeState(IPlayerState.EState newState)
    {
        CurrentState.StateExit();
        CurrentState.gameObject.SetActive(false);
        
        CurrentState = StatesDic[newState];
        
        CurrentState.gameObject.SetActive(true);
        CurrentState.StateEnter();
    }
}
