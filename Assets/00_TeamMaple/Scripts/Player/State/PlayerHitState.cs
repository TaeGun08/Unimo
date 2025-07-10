using UnityEngine;

public class PlayerHitState : PlayerState
{
    public override void StateEnter()
    {
        var res = LocalPlayer.Instance.UnimoData.DodgeCalculation(LocalPlayer.Instance.UnimoData.Dodge);      
        
        if (res)
        {
            PlayerController.ChangeState(IPlayerState.EState.Stun);
        }
        else
        {
            PlayerController.ChangeState(IPlayerState.EState.Idle);
        }
    }

    protected override void StateUpdate()
    {
    }

    public override void StateExit()
    {
    }
}
