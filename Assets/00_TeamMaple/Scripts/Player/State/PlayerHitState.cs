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
                int damage = Mathf.Max(10, LocalPlayer.CombatEvent.Damage - (int)(statHolder.Def.Value * 0.1f));
                Debug.Log($" statHolder.IsDamageToHeal :: {statHolder.IsDamageToHeal}");
                
                // �ǰ� �������� ü�� ȸ������ ��ȯ ��ų On�� ���
                if (statHolder.IsDamageToHeal)
                {
                    if (StageLoader.IsBonusStageByIndex(Base_Mng.Data.data.CurrentStage) == false)
                    {
                        LocalPlayer.PlayerStatHolder.Hp.Add(damage);
                    }
                    
                    PlayerController.ChangeState(IPlayerState.EState.Idle);
                }
                else
                {
                    if (StageLoader.IsBonusStageByIndex(Base_Mng.Data.data.CurrentStage) == false)
                    {
                        LocalPlayer.PlayerStatHolder.Hp.Subtract(damage);
                    }

                    if (LocalPlayer.PlayerStatHolder.Hp.Value <= 0)
                    {
                        PlayerController.ChangeState(IPlayerState.EState.Dead);
                    }
                    else
                    {
                        PlayerController.ChangeState(IPlayerState.EState.Stun);
                    }
                }
            }
        }
        
        LocalPlayer.CombatEvent = null;
    }

    protected override void StateUpdate()
    {
    }

    public override void StateExit()
    {
    }
}
