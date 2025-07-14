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

    // ���ϸ� ���� ������ �ݿ�
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

    #region ���� ���� ��ȭ �ݿ�

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
                value += EquipmentStatLevelUpData.Hp;    // ���� ���� ������ �ݿ�
                CalculateHp(value);
                break;
            case UnimoStat.Def:
                value += EquipmentStatLevelUpData.Def;    // ���� ���� ������ �ݿ�
                CalculateDef(value);
                break;
            case UnimoStat.Speed:
                value += EquipmentStatLevelUpData.Speed;    // ���� ���� ������ �ݿ�
                CalculateSpeed(value);
                break;
            case UnimoStat.BloomRange:
                value += EquipmentStatLevelUpData.BloomRange;    // ���� ���� ������ �ݿ�
                CalculateBloomRange(value);
                break;
            case UnimoStat.BloomSpeed:
                value += EquipmentStatLevelUpData.BloomSpeed;    // ���� ���� ������ �ݿ�
                CalculateBloomSpeed(value);
                break;
            case UnimoStat.FlowerRate:
                value += EquipmentStatLevelUpData.FlowerRate;    // ���� ���� ������ �ݿ�
                CalculateFlowerRate(value);
                break;
            case UnimoStat.RareFlowerRate:
                value += EquipmentStatLevelUpData.RareFlowerRate;    // ���� ���� ������ �ݿ�
                CalculateRareFlowerRate(value);
                break;
            case UnimoStat.Dodge:
                value += EquipmentStatLevelUpData.Dodge;    // ���� ���� ������ �ݿ�
                CalculateDodge(value);
                break;
            case UnimoStat.StunRecovery:
                value += EquipmentStatLevelUpData.StunRecovery;    // ���� ���� ������ �ݿ�
                CalculateStunRecovery(value);
                break;
            case UnimoStat.HpRecovery:
                value += EquipmentStatLevelUpData.HpRecovery;    // ���� ���� ������ �ݿ�
                CalculateHpRecovery(value);
                break;
            case UnimoStat.FlowerDropSpeed:
                value += EquipmentStatLevelUpData.FlowerDropSpeed;    // ���� ���� ������ �ݿ�
                CalculateFlowerDropSpeed(value);
                break;
            case UnimoStat.FlowerDropAmount:
                value += EquipmentStatLevelUpData.FlowerDropAmount;    // ���� ���� ������ �ݿ�
                CalculateFlowerDropAmount(value);
                break;
        }
    }

    #endregion

    #region ���� ���

    // (�⺻ ��ġ + ��ȭ ��ġ) �� �غؿ��� ����
    private void CalculateHp(float value)
    {
        UnimoStatData.Hp = (int)(UnimoStatData.Hp * value);
    }
    
    // (�⺻ ��ġ + ��ȭ ��ġ) �� �غؿ��� ����
    private void CalculateDef(float value)
    {
        UnimoStatData.Def = (int)(UnimoStatData.Def * value);
    }
    
    // (�⺻ ��ġ �� ��ȭ ��ġ) �� �غؿ��� ����
    private void CalculateSpeed(float value)
    {
        UnimoStatData.Speed = (int)(UnimoStatData.Speed * value);
    }
    
    // (�⺻ ��ġ + ��ȭ ��ġ) �� �غؿ��� ����
    private void CalculateBloomRange(float value)
    {
        UnimoStatData.BloomRange = (int)(UnimoStatData.BloomRange * value);
    }
    
    // ���� ��ȭ �ð� �� (1 + (�⺻ ��ġ + ��ȭ ��ġ) �� �غؿ��� ����)
    private void CalculateBloomSpeed(float value)
    {
        
    }
    
    // �������� ���� ������ �ֱ� �� ((�⺻ ��ġ + ��ȭ ��ġ) �� �غؿ��� ����)
    private void CalculateFlowerRate(float value)
    {
        
    }
    
    // (�⺻ ��ġ + ��ȭ ��ġ) �� �غؿ��� ����
    private void CalculateRareFlowerRate(float value)
    {
        UnimoStatData.RareFlowerRate *= value;
    }
    
    // (�⺻ ��ġ + ��ȭ ��ġ) �� �غؿ��� ����
    private void CalculateDodge(float value)
    {
        UnimoStatData.Dodge *= value;
    }
    
    // 1 - ((�⺻ ��ġ + ��ȭ ��ġ) �� �غؿ��� ����)
    private void CalculateStunRecovery(float value)
    {
        UnimoStatData.StunRecovery = 1 - UnimoStatData.StunRecovery * value;
    }
    
    // (�⺻ ��ġ + ��ȭ ��ġ) �� �غؿ��� ����
    private void CalculateHpRecovery(float value)
    {
        UnimoStatData.HpRecovery *= value;
    }
    
    // ���� ���� �ð� �� (1 + (�⺻ ��ġ + ��ȭ ��ġ) �� �غؿ��� ����)
    private void CalculateFlowerDropSpeed(float value)
    {
        
    }
    
    // �ִ� ���� �ֱ� �� (1 - (�⺻ ��ġ + ��ȭ ��ġ) �� �غؿ��� ����)
    private void CalculateFlowerDropAmount(float value)
    {
        
    }

    #endregion
}
