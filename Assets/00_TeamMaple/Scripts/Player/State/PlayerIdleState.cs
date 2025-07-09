using System;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public override void StateEnter()
    {
    }

    protected override void StateUpdate()
    {
        if (PlayerController.VirtualJoystickCtrl.Dir.magnitude > 0.01f)
        {
            PlayerController.ChangeState(IPlayerState.EState.Move);
        }
    }

    public override void StateExit()
    {
    }
}
