using UnityEngine;

public class PlayerHitState : PlayerState
{
    public override void StateEnter()
    {
        // StatCalculator에서 Dodge 확률을 받아옴 (예: 0.25면 25%)
        var dodgeChance = LocalPlayer.StatCalculator.Dodge;

        // 0~1 랜덤값 생성해서, dodgeChance 이하이면 회피
        bool isDodged = Random.value < dodgeChance;
        
        if (isDodged)
        {
            PlayerController.ChangeState(IPlayerState.EState.Idle);
        }
        else
        {
            PlayerController.ChangeState(IPlayerState.EState.Stun);
        }
    }

    protected override void StateUpdate()
    {
    }

    public override void StateExit()
    {
    }
}
