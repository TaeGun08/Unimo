using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("PlayerStates")]
    [SerializeField] private PlayerState[] playerSates;
    
    public Dictionary<IPlayerState.EState, PlayerState> StatesDic { get; private set; }
    public PlayerState CurrentState { get; set; }
    
    private VirtualJoystickCtrl_ST001  virtualJoystickCtrl;
    public VirtualJoystickCtrl_ST001 VirtualJoystickCtrl => virtualJoystickCtrl;
    
    public Animator UnimoAnim { get; set; }
    public Animator EgineAnim { get; set; }

    private IEnumerator Start()
    {
        yield return null;
        
        virtualJoystickCtrl = GetComponent<VirtualJoystickCtrl_ST001>();
        
        StatesDic = new Dictionary<IPlayerState.EState, PlayerState>();
        
        foreach (PlayerState playerSate in playerSates)
        {
            StatesDic.Add(playerSate.State, playerSate);
            StatesDic[playerSate.State].PlayerController = this;
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
