using UnityEngine;

public class StatCalculator
{
    public UnimoStatData UnimoStatData { get; private set; }
    private UnimoStatLevelUpData UnimoStatLevelUpData;
    private EquipmentStatData EquipmentStatData;
    private EquipmentStatLevelUpData EquipmentStatLevelUpData;
    private EquipmentSkillLevelUpData EquipmentSkillLevelUpData;
    
    public StatCalculator(
        UnimoStatData unimoData, 
        UnimoStatLevelUpData unimoLevelUpData, 
        EquipmentStatData equipmentData, 
        EquipmentStatLevelUpData equipmentStatLevelUpData,
        EquipmentSkillLevelUpData equipmentSkillLevelUpData)
    {
        UnimoStatData = unimoData;
        UnimoStatLevelUpData = unimoLevelUpData;
        EquipmentStatData = equipmentData;
        EquipmentStatLevelUpData = equipmentStatLevelUpData;
        EquipmentSkillLevelUpData = equipmentSkillLevelUpData;

        ApplyUnimoLevelUpStat();
        
        ApplyEquipmentStat(EquipmentStatData.StatType1, EquipmentStatData.StatValue1);
        ApplyEquipmentStat(EquipmentStatData.StatType2, EquipmentStatData.StatValue2);
        ApplyEquipmentStat(EquipmentStatData.StatType3, EquipmentStatData.StatValue3);
        ApplyEquipmentStat(EquipmentStatData.StatType4, EquipmentStatData.StatValue4);
    }

    // 유니모 레벨 데이터 반영
    private void ApplyUnimoLevelUpStat()
    {
        UnimoStatData.Hp += UnimoStatLevelUpData.PlusHp;
        UnimoStatData.Def += UnimoStatLevelUpData.PlusDef;
        UnimoStatData.Speed *= UnimoStatLevelUpData.PlusSpeed;
        UnimoStatData.BloomRange += UnimoStatLevelUpData.PlusBloomRange;
        UnimoStatData.BloomSpeed += UnimoStatLevelUpData.PlusBloomSpeed;
        UnimoStatData.FlowerRate += UnimoStatLevelUpData.PlusFlowerRate;
        UnimoStatData.RareFlowerRate += UnimoStatLevelUpData.PlusRareFlowerRate;
        UnimoStatData.Dodge += UnimoStatLevelUpData.PlusDodge;
        UnimoStatData.StunRecovery += UnimoStatLevelUpData.PlusStunRecovery;
        UnimoStatData.HpRecovery += UnimoStatLevelUpData.PlusHpRecovery;
        UnimoStatData.FlowerDropSpeed += UnimoStatLevelUpData.PlusFlowerDropSpeed;
        UnimoStatData.FlowerDropAmount += UnimoStatLevelUpData.PlusFlowerDropAmount;
    }

    #region 엔진 스탯 강화 반영

    private void ApplyEquipmentStat(UnimoStat statType, float value)
    {
        if (statType == UnimoStat.None)
            return;

        AddEquipmentStat(statType, value);
    }
    
    private void AddEquipmentStat(UnimoStat statType, float value)
    {
        switch (statType)
        {
            case UnimoStat.Hp:
                value += EquipmentStatLevelUpData.Hp;    // 엔진 레벨 데이터 반영
                CalculateHp(value);
                break;
            case UnimoStat.Def:
                value += EquipmentStatLevelUpData.Def;    // 엔진 레벨 데이터 반영
                CalculateDef(value);
                break;
            case UnimoStat.Speed:
                value += EquipmentStatLevelUpData.Speed;    // 엔진 레벨 데이터 반영
                CalculateSpeed(value);
                break;
            case UnimoStat.BloomRange:
                value += EquipmentStatLevelUpData.BloomRange;    // 엔진 레벨 데이터 반영
                CalculateBloomRange(value);
                break;
            case UnimoStat.BloomSpeed:
                value += EquipmentStatLevelUpData.BloomSpeed;    // 엔진 레벨 데이터 반영
                CalculateBloomSpeed(value);
                break;
            case UnimoStat.FlowerRate:
                value += EquipmentStatLevelUpData.FlowerRate;    // 엔진 레벨 데이터 반영
                CalculateFlowerRate(value);
                break;
            case UnimoStat.RareFlowerRate:
                value += EquipmentStatLevelUpData.RareFlowerRate;    // 엔진 레벨 데이터 반영
                CalculateRareFlowerRate(value);
                break;
            case UnimoStat.Dodge:
                value += EquipmentStatLevelUpData.Dodge;    // 엔진 레벨 데이터 반영
                CalculateDodge(value);
                break;
            case UnimoStat.StunRecovery:
                value += EquipmentStatLevelUpData.StunRecovery;    // 엔진 레벨 데이터 반영
                CalculateStunRecovery(value);
                break;
            case UnimoStat.HpRecovery:
                value += EquipmentStatLevelUpData.HpRecovery;    // 엔진 레벨 데이터 반영
                CalculateHpRecovery(value);
                break;
            case UnimoStat.FlowerDropSpeed:
                value += EquipmentStatLevelUpData.FlowerDropSpeed;    // 엔진 레벨 데이터 반영
                CalculateFlowerDropSpeed(value);
                break;
            case UnimoStat.FlowerDropAmount:
                value += EquipmentStatLevelUpData.FlowerDropAmount;    // 엔진 레벨 데이터 반영
                CalculateFlowerDropAmount(value);
                break;
        }
    }

    #endregion

    #region 스탯 계산

    // (기본 수치 + 강화 수치) × 붕붕엔진 배율
    private void CalculateHp(float value)
    {
        UnimoStatData.Hp = (int)(UnimoStatData.Hp * value);
    }
    
    // (기본 수치 + 강화 수치) × 붕붕엔진 배율
    private void CalculateDef(float value)
    {
        UnimoStatData.Def = (int)(UnimoStatData.Def * value);
    }
    
    // (기본 수치 × 강화 수치) × 붕붕엔진 배율
    private void CalculateSpeed(float value)
    {
        UnimoStatData.Speed = (int)(UnimoStatData.Speed * value);
    }
    
    // (기본 수치 + 강화 수치) × 붕붕엔진 배율
    private void CalculateBloomRange(float value)
    {
        UnimoStatData.BloomRange = (int)(UnimoStatData.BloomRange * value);
    }
    
    // 기준 개화 시간 ÷ (1 + (기본 수치 + 강화 수치) × 붕붕엔진 배율)
    private void CalculateBloomSpeed(float value)
    {
        
    }
    
    // 스테이지 별꽃 리스폰 주기 ÷ ((기본 수치 + 강화 수치) × 붕붕엔진 배율)
    private void CalculateFlowerRate(float value)
    {
        
    }
    
    // (기본 수치 + 강화 수치) × 붕붕엔진 배율
    private void CalculateRareFlowerRate(float value)
    {
        UnimoStatData.RareFlowerRate *= value;
    }
    
    // (기본 수치 + 강화 수치) × 붕붕엔진 배율
    private void CalculateDodge(float value)
    {
        UnimoStatData.Dodge *= value;
    }
    
    // 1 - ((기본 수치 + 강화 수치) × 붕붕엔진 배율)
    private void CalculateStunRecovery(float value)
    {
        UnimoStatData.StunRecovery = 1 - UnimoStatData.StunRecovery * value;
    }
    
    // (기본 수치 + 강화 수치) × 붕붕엔진 배율
    private void CalculateHpRecovery(float value)
    {
        UnimoStatData.HpRecovery *= value;
    }
    
    // 기준 낙하 시간 ÷ (1 + (기본 수치 + 강화 수치) × 붕붕엔진 배율)
    private void CalculateFlowerDropSpeed(float value)
    {
        
    }
    
    // 최대 낙하 주기 × (1 - (기본 수치 + 강화 수치) × 붕붕엔진 배율)
    private void CalculateFlowerDropAmount(float value)
    {
        
    }

    #endregion
}
