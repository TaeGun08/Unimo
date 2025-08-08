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

        // �⺻��, �ּҰ�, �ִ밪���� ClampedValue �ʱ�ȭ
        // �ּ�, �ִ밪 ���� �ʿ�
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
    
    // 1ȸ �ǰ� ��ȿ �ο�
    public void GiveOnceInvalid()
    {
        HasOnceInvalid = true;
    }

    // 1ȸ �ǰ� ��ȿ ����
    public void RemoveOnceInvalid()
    {
        HasOnceInvalid = false;
    }
    
    // ���� �ο�
    public void GiveInvincible()
    {
        HasInvincible = true;
    }
    
    // ���� ����
    public void RemoveInvincible()
    {
        HasInvincible = false;
    }

    // InvalidType�� None�� �ƴ� ��, �ǰ� �ÿ� ȣ��
    public void OnInvalidation()
    {
        if (HasInvincible)
        {
            Debug.Log("����! ������ ��ȿ");
            return;
        }
        
        if (HasOnceInvalid)
        {
            Debug.Log("1ȸ �ǰ� ��ȿȭ! ������ ��ȿ");
            HasOnceInvalid = false;
            OnOnceInvalidUsed?.Invoke();    // ��Ÿ�� ��ŸƮ
        }
    }

    // �ǰ� �������� ü�� ȸ������ ��ȯ ���� ����
    public void SetDamageToHeal(bool yesOrNo)
    {
        IsDamageToHeal = yesOrNo;
    }
}
