using UnityEngine;

public class PlayerHitState : PlayerState
{
    public override void StateEnter()
    {
        // StatHolder ĳ��
        var statHolder = LocalPlayer.Instance.PlayerStatHolder;
        
        // StatHolder���� Dodge Ȯ���� �޾ƿ� (��: 0.25�� 25%)
        var dodgeChance = statHolder.Dodge.Value;

        // 0~1 ������ �����ؼ�, dodgeChance �����̸� ȸ��
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
