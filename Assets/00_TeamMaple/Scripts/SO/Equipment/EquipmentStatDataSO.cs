using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;

public enum EquipmentRank
{
    N,
    R,
    SR
}

[System.Serializable]
public class EquipmentStatData
{
    public int Id { get; set; }    // 엔진 아이디
    public int Level { get; set; }    // 엔진 레벨
    public string Name { get; set; }    // 엔진 이름
    public EquipmentRank Rank { get; set; }    // 엔진 등급
    public UnimoStat StatType1 { get; set; }    // 강화할 유니모 스탯 1
    public float StatValue1 { get; set; }    // 강화할 값 1
    public UnimoStat StatType2 { get; set; }    // 강화할 유니모 스탯 2
    public float StatValue2 { get; set; }    // 강화할 값 2
    public UnimoStat StatType3 { get; set; }    // 강화할 유니모 스탯 3
    public float StatValue3 { get; set; }    // 강화할 값 3
    public UnimoStat StatType4 { get; set; }    // 강화할 유니모 스탯 4
    public float StatValue4 { get; set; }    // 강화할 값 4
    public int Skill1 { get; set; }    // 보유 스킬 아이디 1
    public int Skill2 { get; set; }    // 보유 스킬 아이디 2
}

[CreateAssetMenu(fileName = "EquipmentStatDataSO", menuName = "Scriptable Object/EquipmentStatDataSO")]
public class EquipmentStatDataSO : ScriptableObject
{
    [Header("EquipmentStatDataCsv")]
    [SerializeField] private TextAsset equipmentStatDataCsv;

    [Header("EquipmentStatLevelUpDataSO")]
    [SerializeField] private EquipmentStatLevelUpDataSO equipmentStatLevelUpDataSO;
    
    // EquipmentStatDataSO + 레벨업 데이터 적용 최종값 반환
    public EquipmentStatData GetFinalEquipmnetStatData(int equipmentID)
    {
        EquipmentStatData baseData = GetEquipmentStatData(equipmentID);
        if (baseData == null)
            return null;

        EquipmentStatLevelUpData levelUpData = null;
        if (equipmentStatLevelUpDataSO != null)
            levelUpData = equipmentStatLevelUpDataSO.GetEquipmentStatLevelUpData(baseData.Rank, baseData.Level);

        // 복사본 생성 (기존 데이터 수정 방지)
        EquipmentStatData merged = new EquipmentStatData
        {
            Id = baseData.Id,
            Level = baseData.Level,
            Name = baseData.Name,
            Rank = baseData.Rank,
            StatType1 = baseData.StatType1,
            StatValue1 = baseData.StatValue1,
            StatType2 = baseData.StatType2,
            StatValue2 = baseData.StatValue2,
            StatType3 = baseData.StatType3,
            StatValue3 = baseData.StatValue3,
            StatType4 = baseData.StatType4,
            StatValue4 = baseData.StatValue4,
            Skill1 = baseData.Skill1,
            Skill2 = baseData.Skill2
        };

        if (levelUpData != null)
        {
            // 레벨업 데이터 반영 (null일 경우 무시)
            merged.Level = levelUpData.Level;
            merged.StatValue1 += GetLevelUpValue(levelUpData, baseData.StatType1);
            merged.StatValue2 += GetLevelUpValue(levelUpData, baseData.StatType1);
            merged.StatValue3 += GetLevelUpValue(levelUpData, baseData.StatType1);
            merged.StatValue4 += GetLevelUpValue(levelUpData, baseData.StatType1);
        }

        return merged;
    }
    
    // 기존 데이터 로드 함수
    public EquipmentStatData GetEquipmentStatData(int equipmentID)
    {
        if (equipmentStatDataCsv == null)
        {
            Debug.LogError("EquipmentStatDataCsv is null");
            return null;
        }

        using (StringReader reader = new StringReader(equipmentStatDataCsv.text))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read(); // unimo_id, name, hp, ...
            csv.ReadHeader();   // 헤더 등록
            
            IEnumerable<EquipmentStatData> records = csv.GetRecords<EquipmentStatData>();

            foreach (EquipmentStatData record in records)
            {
                if (record.Id == equipmentID)
                    return record;
            }
        }

        Debug.LogWarning($"EquipmentStatData with ID {equipmentID} not found.");
        return null;
    }
    
    // 각 StatType별로 LevelUp에서 값을 찾아 반환
    private float GetLevelUpValue(EquipmentStatLevelUpData levelUp, UnimoStat statType)
    {
        return statType switch
        {
            UnimoStat.Hp => levelUp.Hp,
            UnimoStat.Def => levelUp.Def,
            UnimoStat.Speed => levelUp.Speed,
            UnimoStat.BloomRange => levelUp.BloomRange,
            UnimoStat.BloomSpeed => levelUp.BloomSpeed,
            UnimoStat.FlowerRate => levelUp.FlowerRate,
            UnimoStat.RareFlowerRate => levelUp.RareFlowerRate,
            UnimoStat.Dodge => levelUp.Dodge,
            UnimoStat.StunRecovery => levelUp.StunRecovery,
            UnimoStat.HpRecovery => levelUp.HpRecovery,
            UnimoStat.FlowerDropSpeed => levelUp.FlowerDropSpeed,
            UnimoStat.FlowerDropAmount => levelUp.FlowerDropAmount,
            _ => 0f
        };
    }
}
