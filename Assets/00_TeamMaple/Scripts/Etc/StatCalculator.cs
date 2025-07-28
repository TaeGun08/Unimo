using System;
using UnityEngine;

// ���ϸ� + ���� ���� ��� ��� ��ȯ (��ų ���� X)
public class StatCalculator
{
    private UnimoStatData unimoStatData;
    private EquipmentStatData equipmentStatData;
    
    public StatCalculator(UnimoStatData unimoData, EquipmentStatData equipmentData)
    {
        unimoStatData = unimoData;
        equipmentStatData = equipmentData;
    }

    // ������ ���� ��ȯ�ϴ� ������Ƽ��
    public int Hp => CalculateHp();
    public int Def => CalculateDef();
    public float Speed => CalculateSpeed();
    public int BloomRange => CalculateBloomRange();
    public float BloomSpeed => CalculateBloomSpeed();
    public float FlowerRate => CalculateFlowerRate();
    public float RareFlowerRate => CalculateRareFlowerRate();
    public float Dodge => CalculateDodge();
    public float StunRecovery => CalculateStunRecovery();
    public float HpRecovery => CalculateHpRecovery();
    public float FlowerDropSpeed => CalculateFlowerDropSpeed();
    public float FlowerDropAmount => CalculateFlowerDropAmount();
    
    // ��� ���� �ջ�
    private float GetEquipmentValue(UnimoStat statType)
    {
        float value = 1f;
        
        if (equipmentStatData != null)
        {
            if (equipmentStatData.StatType1 == statType) value = equipmentStatData.StatValue1;
            if (equipmentStatData.StatType2 == statType) value = equipmentStatData.StatValue2;
            if (equipmentStatData.StatType3 == statType) value = equipmentStatData.StatValue3;
            if (equipmentStatData.StatType4 == statType) value = equipmentStatData.StatValue4;
        }
        
        return value;
    }
    
    #region ��� �޼���

    // (�⺻ ��ġ + ��ȭ ��ġ) �� (�غؿ��� �⺻ ��ġ + �غؿ��� ��ȭ ����) �� �ü��� ���� �� �нú� ��ų ���� �� ��Ƽ�� ��ų ����
    private int CalculateHp()
    {
        float equipValue = GetEquipmentValue(UnimoStat.Hp);
        float total = unimoStatData.Hp * equipValue;

        return Mathf.RoundToInt(total);
    }

    // (�⺻ ��ġ + ��ȭ ��ġ) �� (�غؿ��� �⺻ ��ġ + �غؿ��� ��ȭ ����) �� �ü��� ���� �� �нú� ��ų ���� �� ��Ƽ�� ��ų ����
    private int CalculateDef()
    {
        float equipValue = GetEquipmentValue(UnimoStat.Def);
        float total = unimoStatData.Def * equipValue;
        
        return Mathf.RoundToInt(total);
    }

    // (�⺻ ��ġ + ��ȭ ��ġ) �� (�غؿ��� �⺻ ��ġ + �غؿ��� ��ȭ ����) �� �ü��� ���� �� �нú� ��ų ���� �� ��Ƽ�� ��ų ����
    private float CalculateSpeed()
    {
        float equipValue = GetEquipmentValue(UnimoStat.Speed);
        float total = unimoStatData.Speed * equipValue;
        return total;
    }

    // (�⺻ ��ġ + ��ȭ ��ġ) �� (�غؿ��� �⺻ ��ġ + �غؿ��� ��ȭ ����) �� �нú� ��ų ���� �� ��Ƽ�� ��ų ����
    private int CalculateBloomRange()
    {
        float equipValue = GetEquipmentValue(UnimoStat.BloomRange);
        float total = unimoStatData.BloomRange * equipValue;
        
        return Mathf.RoundToInt(total);
    }

    // ���� ��ȭ �ð� �� ((1 + (�⺻ ��ġ + ��ȭ ��ġ) �� (�غؿ��� �⺻ ��ġ + �غؿ��� ��ȭ ����)) �� �ü��� ���� �� �нú� ��ų ���� �� ��Ƽ�� ��ų ����)
    private float CalculateBloomSpeed()
    {
        float equipValue = GetEquipmentValue(UnimoStat.BloomSpeed);
        float total = unimoStatData.BloomSpeed * equipValue;
        
        // ���� ��ȭ �ð�(12��) �� (1 + total)
        float baseTime = 12f;
        float denominator = 1f + total;
        
        return baseTime / denominator;
    }

    // �������� ���� ������ �ֱ� �� ((�⺻ ��ġ + ��ȭ ��ġ) �� (�غؿ��� �⺻ ��ġ + �غؿ��� ��ȭ ����) �� �нú� ��ų ���� �� ��Ƽ�� ��ų ����)
    private float CalculateFlowerRate()
    {
        float equipValue = GetEquipmentValue(UnimoStat.FlowerRate);
        float total = unimoStatData.FlowerRate * equipValue;
        
        // �������� ���� ������ �ֱ�(��: 1��) �� total
        float baseRate = 1f;
        
        return baseRate / total;
    }

    // (�⺻ ��ġ + ��ȭ ��ġ) �� (�غؿ��� �⺻ ��ġ + �غؿ��� ��ȭ ����) �� �нú� ��ų ���� �� ��Ƽ�� ��ų ����
    private float CalculateRareFlowerRate()
    {
        float equipValue = GetEquipmentValue(UnimoStat.RareFlowerRate);
        float total = unimoStatData.RareFlowerRate * equipValue;
        
        return total;
    }

    // (�⺻ ��ġ + ��ȭ ��ġ) �� (�غؿ��� �⺻ ��ġ + �غؿ��� ��ȭ ����) �� �ü��� ���� �� �нú� ��ų ���� �� ��Ƽ�� ��ų ����
    private float CalculateDodge()
    {
        float equipValue = GetEquipmentValue(UnimoStat.Dodge);
        float total = unimoStatData.Dodge * equipValue;
        
        return total;
    }

    // 1 - ((�⺻ ��ġ + ��ȭ ��ġ) �� (�غؿ��� �⺻ ��ġ + �غؿ��� ��ȭ ����) �� �нú� ��ų ���� �� ��Ƽ�� ��ų ����)
    private float CalculateStunRecovery()
    {
        float equipValue = GetEquipmentValue(UnimoStat.StunRecovery);
        float total = unimoStatData.StunRecovery * equipValue;
        
        return 1f - total;
    }

    // (�⺻ ��ġ + ��ȭ ��ġ) �� (�غؿ��� �⺻ ��ġ + �غؿ��� ��ȭ ����) �� �нú� ��ų ���� �� ��Ƽ�� ��ų ����
    private float CalculateHpRecovery()
    {
        float equipValue = GetEquipmentValue(UnimoStat.HpRecovery);
        float total = unimoStatData.HpRecovery * equipValue;
        
        return total;
    }

    // ���� ���� �ð� �� (1 + (�⺻ ��ġ + ��ȭ ��ġ) �� (�غؿ��� �⺻ ��ġ + �غؿ��� ��ȭ ����) �� �нú� ��ų ���� �� ��Ƽ�� ��ų ����)
    private float CalculateFlowerDropSpeed()
    {
        float equipValue = GetEquipmentValue(UnimoStat.FlowerDropSpeed);
        float total = unimoStatData.FlowerDropSpeed * equipValue;
        
        // ���� ���� �ð�(��: 1��) �� (1 + total)
        float baseTime = 3f;
        float denominator = 1f + total;
        
        return baseTime / denominator;
    }

    // �ִ� ���� �ֱ� �� (1 - (�⺻ ��ġ + ��ȭ ��ġ) �� (�غؿ��� �⺻ ��ġ + �غؿ��� ��ȭ ����) �� �нú� ��ų ���� �� ��Ƽ�� ��ų ����)
    private float CalculateFlowerDropAmount()
    {
        float equipValue = GetEquipmentValue(UnimoStat.FlowerDropAmount);
        float total = unimoStatData.FlowerDropAmount * equipValue;
        
        // �ִ� ���� �ֱ�(��: 1f) �� (1 - total)
        float maxDrop = 2f;
        float multiplier = 1f - total;
        
        return maxDrop * multiplier;
    }
    
    // Ÿ�� ��ȯ
    public static readonly (UnimoStat stat, Func<StatCalculator, float> getter)[] _finalStats =
    {
        (UnimoStat.Hp, s => s.Hp),
        (UnimoStat.Def, s => s.Def),
        (UnimoStat.Speed, s => s.Speed),
        (UnimoStat.BloomRange, s => s.BloomRange),
        (UnimoStat.BloomSpeed, s => s.BloomSpeed),
        (UnimoStat.FlowerRate, s => s.FlowerRate),
        (UnimoStat.RareFlowerRate, s => s.RareFlowerRate),
        (UnimoStat.Dodge, s => s.Dodge),
        (UnimoStat.StunRecovery, s => s.StunRecovery),
        (UnimoStat.HpRecovery, s => s.HpRecovery),
        (UnimoStat.FlowerDropSpeed, s => s.FlowerDropSpeed),
        (UnimoStat.FlowerDropAmount, s => s.FlowerDropAmount),
    };
    
    public static readonly (UnimoStat stat, Func<UnimoStatData, float> getter)[] _unimoStats =
    {
        (UnimoStat.Hp, d => d.Hp),
        (UnimoStat.Def, d => d.Def),
        (UnimoStat.Speed, d => d.Speed),
        (UnimoStat.BloomRange, d => d.BloomRange),
        (UnimoStat.BloomSpeed, d => d.BloomSpeed),
        (UnimoStat.FlowerRate, d => d.FlowerRate),
        (UnimoStat.RareFlowerRate, d => d.RareFlowerRate),
        (UnimoStat.Dodge, d => d.Dodge),
        (UnimoStat.StunRecovery, d => d.StunRecovery),
        (UnimoStat.HpRecovery, d => d.HpRecovery),
        (UnimoStat.FlowerDropSpeed, d => d.FlowerDropSpeed),
        (UnimoStat.FlowerDropAmount, d => d.FlowerDropAmount),
    };
    
    public static readonly (UnimoStat stat, Func<UnimoStatLevelUpData, float> getter)[] _unimoNextStats =
    {
        (UnimoStat.Hp, d => d.PlusHp),
        (UnimoStat.Def, d => d.PlusDef),
        (UnimoStat.Speed, d => d.PlusSpeed),
        (UnimoStat.BloomRange, d => d.PlusBloomRange),
        (UnimoStat.BloomSpeed, d => d.PlusBloomSpeed),
        (UnimoStat.FlowerRate, d => d.PlusFlowerRate),
        (UnimoStat.RareFlowerRate, d => d.PlusRareFlowerRate),
        (UnimoStat.Dodge, d => d.PlusDodge),
        (UnimoStat.StunRecovery, d => d.PlusStunRecovery),
        (UnimoStat.HpRecovery, d => d.PlusHpRecovery),
        (UnimoStat.FlowerDropSpeed, d => d.PlusFlowerDropSpeed),
        (UnimoStat.FlowerDropAmount, d => d.PlusFlowerDropAmount),
    };

    #endregion
}
