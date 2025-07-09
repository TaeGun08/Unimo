using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public override void StateEnter()
    {
    }

    protected override void StateUpdate()
    {
        Vector3 dir = new Vector3(PlayerController.VirtualJoystickCtrl.dir.x, 0, PlayerController.VirtualJoystickCtrl.dir.y);
        PlayerController.transform.position += dir * (5f * Time.deltaTime); //방향, 속도
        
        if (PlayerController.VirtualJoystickCtrl.dir.magnitude < 0.01f)
        {
            PlayerController.ChangeState(IPlayerState.EState.Idle);
        }
    }

    public override void StateExit()
    {
    }
}
