using System;
using UnityEngine;

public class PlayerMoveState : PlayerState
{
    private readonly float rotateThreshold = 0.05f;
    private float rotateTime = 0.3f;
    
    private float turnCalmVelocity;

    public override void StateEnter()
    {
    }

    protected override void StateUpdate()
    {
        if (mapSetter.IsInMap(PlayerController.transform.position) == false)
        {
            PlayerController.transform.position = mapSetter.FindNearestPoint(PlayerController.transform.position);
        }
        
        PlayerController.transform.ApplyMovementAndRotation(PlayerController.VirtualJoystickCtrl.Dir, 
            LocalPlayer.PlayerStatHolder.Speed.Value, turnCalmVelocity, 0.01f);

        if (PlayerController.VirtualJoystickCtrl.Dir.magnitude < 0.01f)
        {
            PlayerController.ChangeState(IPlayerState.EState.Idle);
        }
    }

    public override void StateExit()
    {
    }
}
