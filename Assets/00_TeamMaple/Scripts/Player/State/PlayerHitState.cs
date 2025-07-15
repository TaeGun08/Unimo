using UnityEngine;

public class PlayerHitState : PlayerState
{
    public override void StateEnter()
    {
        // StatCalculator���� Dodge Ȯ���� �޾ƿ� (��: 0.25�� 25%)
        var dodgeChance = LocalPlayer.StatCalculator.Dodge;

        // 0~1 ������ �����ؼ�, dodgeChance �����̸� ȸ��
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
