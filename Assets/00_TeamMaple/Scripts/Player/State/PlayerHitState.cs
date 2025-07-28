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

        // 1회 피격 무효 or 무적일 경우 회피
        if (statHolder.HasOnceInvalid || statHolder.HasInvincible)
        {
            statHolder.OnInvalidation();
            
            PlayerController.ChangeState(IPlayerState.EState.Idle);
        }
        else
        {
            // 회피하는 경우
            if (isDodged)
            {
                PlayerController.ChangeState(IPlayerState.EState.Idle);
            }
            else
            {
                int damage = Mathf.Max(10, LocalPlayer.CombatEvent.Damage - (int)(statHolder.Def.Value * 0.1f));
                Debug.Log($" statHolder.IsDamageToHeal :: {statHolder.IsDamageToHeal}");
                
                // 피격 데미지를 체력 회복으로 전환 스킬 On인 경우
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
