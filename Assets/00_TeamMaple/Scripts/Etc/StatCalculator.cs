using System;
using UnityEngine;

// 유니모 + 엔진 스탯 계산 결과 반환 (스킬 포함 X)
public class StatCalculator
{
    private UnimoStatData unimoStatData;
    private EquipmentStatData equipmentStatData;
    
    public StatCalculator(UnimoStatData unimoData, EquipmentStatData equipmentData)
    {
        unimoStatData = unimoData;
        equipmentStatData = equipmentData;
    }

    // 스탯을 직접 반환하는 프로퍼티들
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
    
    // 장비 스탯 합산
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
    
    #region 계산 메서드

    // (기본 수치 + 강화 수치) × (붕붕엔진 기본 수치 + 붕붕엔진 강화 배율) × 시설물 배율 × 패시브 스킬 배율 × 액티브 스킬 배율
    private int CalculateHp()
    {
        float equipValue = GetEquipmentValue(UnimoStat.Hp);
        float total = unimoStatData.Hp * equipValue;

        return Mathf.RoundToInt(total);
    }

    // (기본 수치 + 강화 수치) × (붕붕엔진 기본 수치 + 붕붕엔진 강화 배율) × 시설물 배율 × 패시브 스킬 배율 × 액티브 스킬 배율
    private int CalculateDef()
    {
        float equipValue = GetEquipmentValue(UnimoStat.Def);
        float total = unimoStatData.Def * equipValue;
        
        return Mathf.RoundToInt(total);
    }

    // (기본 수치 + 강화 수치) × (붕붕엔진 기본 수치 + 붕붕엔진 강화 배율) × 시설물 배율 × 패시브 스킬 배율 × 액티브 스킬 배율
    private float CalculateSpeed()
    {
        float equipValue = GetEquipmentValue(UnimoStat.Speed);
        float total = unimoStatData.Speed * equipValue;
        return total;
    }

    // (기본 수치 + 강화 수치) × (붕붕엔진 기본 수치 + 붕붕엔진 강화 배율) × 패시브 스킬 배율 × 액티브 스킬 배율
    private int CalculateBloomRange()
    {
        float equipValue = GetEquipmentValue(UnimoStat.BloomRange);
        float total = unimoStatData.BloomRange * equipValue;
        
        return Mathf.RoundToInt(total);
    }

    // 기준 개화 시간 ÷ ((1 + (기본 수치 + 강화 수치) × (붕붕엔진 기본 수치 + 붕붕엔진 강화 배율)) × 시설물 배율 × 패시브 스킬 배율 × 액티브 스킬 배율)
    private float CalculateBloomSpeed()
    {
        float equipValue = GetEquipmentValue(UnimoStat.BloomSpeed);
        float total = unimoStatData.BloomSpeed * equipValue;
        
        // 기준 개화 시간(12초) ÷ (1 + total)
        float baseTime = 12f;
        float denominator = 1f + total;
        
        return baseTime / denominator;
    }

    // 스테이지 별꽃 리스폰 주기 ÷ ((기본 수치 + 강화 수치) × (붕붕엔진 기본 수치 + 붕붕엔진 강화 배율) × 패시브 스킬 배율 × 액티브 스킬 배율)
    private float CalculateFlowerRate()
    {
        float equipValue = GetEquipmentValue(UnimoStat.FlowerRate);
        float total = unimoStatData.FlowerRate * equipValue;
        
        // 스테이지 별꽃 리스폰 주기(예: 1초) ÷ total
        float baseRate = 1f;
        
        return baseRate / total;
    }

    // (기본 수치 + 강화 수치) × (붕붕엔진 기본 수치 + 붕붕엔진 강화 배율) × 패시브 스킬 배율 × 액티브 스킬 배율
    private float CalculateRareFlowerRate()
    {
        float equipValue = GetEquipmentValue(UnimoStat.RareFlowerRate);
        float total = unimoStatData.RareFlowerRate * equipValue;
        
        return total;
    }

    // (기본 수치 + 강화 수치) × (붕붕엔진 기본 수치 + 붕붕엔진 강화 배율) × 시설물 배율 × 패시브 스킬 배율 × 액티브 스킬 배율
    private float CalculateDodge()
    {
        float equipValue = GetEquipmentValue(UnimoStat.Dodge);
        float total = unimoStatData.Dodge * equipValue;
        
        return total;
    }

    // 1 - ((기본 수치 + 강화 수치) × (붕붕엔진 기본 수치 + 붕붕엔진 강화 배율) × 패시브 스킬 배율 × 액티브 스킬 배율)
    private float CalculateStunRecovery()
    {
        float equipValue = GetEquipmentValue(UnimoStat.StunRecovery);
        float total = unimoStatData.StunRecovery * equipValue;
        
        return 1f - total;
    }

    // (기본 수치 + 강화 수치) × (붕붕엔진 기본 수치 + 붕붕엔진 강화 배율) × 패시브 스킬 배율 × 액티브 스킬 배율
    private float CalculateHpRecovery()
    {
        float equipValue = GetEquipmentValue(UnimoStat.HpRecovery);
        float total = unimoStatData.HpRecovery * equipValue;
        
        return total;
    }

    // 기준 낙하 시간 ÷ (1 + (기본 수치 + 강화 수치) × (붕붕엔진 기본 수치 + 붕붕엔진 강화 배율) × 패시브 스킬 배율 × 액티브 스킬 배율)
    private float CalculateFlowerDropSpeed()
    {
        float equipValue = GetEquipmentValue(UnimoStat.FlowerDropSpeed);
        float total = unimoStatData.FlowerDropSpeed * equipValue;
        
        // 기준 낙하 시간(예: 1초) ÷ (1 + total)
        float baseTime = 3f;
        float denominator = 1f + total;
        
        return baseTime / denominator;
    }

    // 최대 낙하 주기 × (1 - (기본 수치 + 강화 수치) × (붕붕엔진 기본 수치 + 붕붕엔진 강화 배율) × 패시브 스킬 배율 × 액티브 스킬 배율)
    private float CalculateFlowerDropAmount()
    {
        float equipValue = GetEquipmentValue(UnimoStat.FlowerDropAmount);
        float total = unimoStatData.FlowerDropAmount * equipValue;
        
        // 최대 낙하 주기(예: 1f) × (1 - total)
        float maxDrop = 2f;
        float multiplier = 1f - total;
        
        return maxDrop * multiplier;
    }
    
    // 타입 변환
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
