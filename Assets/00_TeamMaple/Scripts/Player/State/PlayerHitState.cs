using UnityEngine;

public class PlayerHitState : PlayerState
{
    public override void StateEnter()
    {
        // StatHolder 캐싱
        var statHolder = LocalPlayer.Instance.PlayerStatHolder;
        
        // StatHolder에서 Dodge 확률을 받아옴 (예: 0.25면 25%)
        var dodgeChance = statHolder.Dodge.Value;

        // 0~1 랜덤값 생성해서, dodgeChance 이하이면 회피
        bool isDodged = Random.value < dodgeChance;

        if (statHolder.CanInvalid != InvalidType.None)
        {
            statHolder.OnInvalidation();
        }
        else
        {
            if (isDodged)
            {
                PlayerController.ChangeState(IPlayerState.EState.Idle);
            }
            else
            {
                PlayerController.ChangeState(IPlayerState.EState.Stun);
            }
        }
    }

    protected override void StateUpdate()
    {
    }

    public override void StateExit()
    {
    }
}
