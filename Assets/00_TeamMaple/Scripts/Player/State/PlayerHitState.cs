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

        // 1ȸ �ǰ� ��ȿ or ������ ��� ȸ��
        if (statHolder.HasOnceInvalid || statHolder.HasInvincible)
        {
            statHolder.OnInvalidation();
            
            PlayerController.ChangeState(IPlayerState.EState.Idle);
        }
        else
        {
            // ȸ���ϴ� ���
            if (isDodged)
            {
                PlayerController.ChangeState(IPlayerState.EState.Idle);
            }
            else
            {
                Debug.Log($" statHolder.IsDamageToHeal :: {statHolder.IsDamageToHeal}");
                
                // �ǰ� �������� ü�� ȸ������ ��ȯ ��ų On�� ���
                if (statHolder.IsDamageToHeal)
                {
                    LocalPlayer.PlayerStatHolder.Hp.Add(10);
                    
                    PlayerController.ChangeState(IPlayerState.EState.Idle);
                }
                else
                {
                    LocalPlayer.PlayerStatHolder.Hp.Subtract(10);

                    if (LocalPlayer.PlayerStatHolder.Hp.Value <= 0)
                    {
                        PlayerController.ChangeState(IPlayerState.EState.Dead);
                    }
                    else
                    {
                        Debug.Log($"stun Ÿ����");
                        PlayerController.ChangeState(IPlayerState.EState.Stun);
                    }
                }
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
