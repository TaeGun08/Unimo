using System;
using UnityEngine;

public class PlayerStatHolder
{
    public ClampedInt Hp { get; private set; }
    public ClampedInt Def { get; private set; }
    public ClampedFloat Speed { get; private set; }
    public ClampedInt BloomRange { get; private set; }
    public ClampedFloat BloomSpeed { get; private set; }
    public ClampedFloat FlowerRate { get; private set; }
    public ClampedFloat RareFlowerRate { get; private set; }
    public ClampedFloat Dodge { get; private set; }
    public ClampedFloat StunRecovery { get; private set; }
    public ClampedFloat HpRecovery { get; private set; }
    public ClampedFloat FlowerDropSpeed { get; private set; }
    public ClampedFloat FlowerDropAmount { get; private set; }
    public bool HasOnceInvalid { get; private set; }
    public bool HasInvincible { get; private set; }
    public bool IsDamageToHeal { get; private set; }

    private StatCalculator statCalculator;

    public PlayerStatHolder(StatCalculator calculator)
    {
        statCalculator = calculator;

        // 기본값, 최소값, 최대값으로 ClampedValue 초기화
        // 최소, 최대값 수정 필요
        Hp                = new ClampedInt(statCalculator.Hp, 0, statCalculator.Hp);
        Def               = new ClampedInt(statCalculator.Def, 0, 9999);
        Speed             = new ClampedFloat(statCalculator.Speed, 0f, 99f);
        BloomRange        = new ClampedInt(statCalculator.BloomRange, 0, 300);
        BloomSpeed        = new ClampedFloat(statCalculator.BloomSpeed, 0f, 999f);
        FlowerRate        = new ClampedFloat(statCalculator.FlowerRate, 0.5f, 100f);
        RareFlowerRate    = new ClampedFloat(statCalculator.RareFlowerRate, 0f, 1f);
        Dodge             = new ClampedFloat(statCalculator.Dodge, 0f, 1f);
        StunRecovery      = new ClampedFloat(statCalculator.StunRecovery, 0f, 1f);
        HpRecovery        = new ClampedFloat(statCalculator.HpRecovery, 0f, 100f);
        FlowerDropSpeed   = new ClampedFloat(statCalculator.FlowerDropSpeed, 0f, 100f);
        FlowerDropAmount  = new ClampedFloat(statCalculator.FlowerDropAmount, 0f, 100f);

        HasOnceInvalid = false;
        HasInvincible = false;
        IsDamageToHeal = false;
    }
    
    public event Action OnOnceInvalidUsed;
    
    // 1회 피격 무효 부여
    public void GiveOnceInvalid()
    {
        HasOnceInvalid = true;
    }

    // 무적 부여
    public void GiveInvincible()
    {
        HasInvincible = true;
    }
    
    // 무적 삭제
    public void RemoveInvincible()
    {
        HasInvincible = false;
    }

    // InvalidType이 None이 아닐 때, 피격 시에 호출
    public void OnInvalidation()
    {
        if (HasInvincible)
        {
            Debug.Log("무적! 데미지 무효");
            return;
        }
        
        if (HasOnceInvalid)
        {
            Debug.Log("1회 피격 무효화! 데미지 무효");
            HasOnceInvalid = false;
            OnOnceInvalidUsed?.Invoke();    // 쿨타임 스타트
        }
    }

    // 피격 데미지를 체력 회복으로 전환 여부 설정
    public void SetDamageToHeal(bool yesOrNo)
    {
        IsDamageToHeal = yesOrNo;
    }
}
