using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using UnityEngine;

// N등급 엔진 강화 데이터
[System.Serializable]
public class EquipmentStatLevelUpData
{
    // 이 중 각 엔진에 해당하는 스탯만 강화함
    public int Level { get; set; }
    public float Hp { get; set; }
    public float Def { get; set; }
    public float Speed { get; set; }
    public float BloomRange { get; set; }
    public float BloomSpeed { get; set; }
    public float FlowerRate { get; set; }
    public float RareFlowerRate { get; set; }
    public float Dodge { get; set; }
    public float StunRecovery { get; set; }
    public float HpRecovery { get; set; }
    public float FlowerDropSpeed { get; set; }
    public float FlowerDropAmount { get; set; }
}

[CreateAssetMenu(fileName = "EquipmentStatLevelUpDataSO", menuName = "Scriptable Object/EquipmentStatLevelUpDataSO")]
public class EquipmentStatLevelUpDataSO : ScriptableObject
{
    [Header("EquipmentStatLevelUpDataCsv")]
    [SerializeField] private TextAsset nRankEquipmentStatLevelUpDataCsv;
    [SerializeField] private TextAsset rRankEquipmentStatLevelUpDataCsv;
    [SerializeField] private TextAsset srRankEquipmentStatLevelUpDataCsv;

    /// <summary>
    /// 입력 받은 아이디에 따라 데이터 반환
    /// </summary>
    public EquipmentStatLevelUpData GetEquipmentStatLevelUpData(EquipmentRank rank, int level)
    {
        TextAsset targetCsv = rank switch
        {
            EquipmentRank.N => nRankEquipmentStatLevelUpDataCsv,
            EquipmentRank.R => rRankEquipmentStatLevelUpDataCsv,
            EquipmentRank.SR => srRankEquipmentStatLevelUpDataCsv,
            _ => null
        };

        if (targetCsv == null)
        {
            Debug.LogError($"CSV for rank {rank} is null");
            return null;
        }

        using (StringReader reader = new StringReader(targetCsv.text))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read();
            csv.ReadHeader();

            IEnumerable<EquipmentStatLevelUpData> records = csv.GetRecords<EquipmentStatLevelUpData>();

            foreach (var record in records)
            {
                if (record.Level == level)
                    return record;
            }
        }

        Debug.LogWarning($"Level {level} not found in equipment stat table for rank {rank}");
        return null;
    }
    
    // 각 StatType별로 LevelUp에서 값을 찾아 반환
    public float GetLevelUpValue(EquipmentStatLevelUpData levelUp, UnimoStat statType)
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
    
     public EquipmentStatLevelUpData GetCurrentAndNextStat(EquipmentRank rank, int currentLevel)
    {
        TextAsset targetCsv = rank switch
        {
            EquipmentRank.N => nRankEquipmentStatLevelUpDataCsv,
            EquipmentRank.R => rRankEquipmentStatLevelUpDataCsv,
            EquipmentRank.SR => srRankEquipmentStatLevelUpDataCsv,
            _ => null
        };
        
        using (StringReader reader = new StringReader(targetCsv.text))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read();
            csv.ReadHeader();

            List<EquipmentStatLevelUpData> allData = csv.GetRecords<EquipmentStatLevelUpData>().ToList();

            var next = allData.FirstOrDefault(x => x.Level == currentLevel + 1);
            var current = allData.FirstOrDefault(x => x.Level == currentLevel);
            
            if (next == null)
            {
                Debug.LogWarning($"No next level data found for level {currentLevel + 1}");
                return null;
            }

            EquipmentStatLevelUpData nextStat = new EquipmentStatLevelUpData
            {
                Level = next.Level - current.Level,
                Hp = next.Hp - current.Hp,
                Def = next.Def - current.Def,
                Speed = next.Speed - current.Speed,
                BloomRange = next.BloomRange - current.BloomRange,
                BloomSpeed = next.BloomSpeed - current.BloomSpeed,
                FlowerRate = next.FlowerRate - current.FlowerRate,
                RareFlowerRate = next.RareFlowerRate - current.RareFlowerRate,
                Dodge = next.Dodge - current.Dodge,
                StunRecovery = next.StunRecovery - current.StunRecovery,
                HpRecovery = next.HpRecovery - current.HpRecovery,
                FlowerDropSpeed = next.FlowerDropSpeed - current.FlowerDropSpeed,
                FlowerDropAmount = next.FlowerDropAmount - current.FlowerDropAmount
            };

            return nextStat;
        }
    }
    
    // public EquipmentStatLevelUpData GetCurrentAndEngineNextStat(EquipmentRank rank, int currentLevel)
    // {
    //     TextAsset targetCsv = rank switch
    //     {
    //         EquipmentRank.N => nRankEquipmentStatLevelUpDataCsv,
    //         EquipmentRank.R => rRankEquipmentStatLevelUpDataCsv,
    //         EquipmentRank.SR => srRankEquipmentStatLevelUpDataCsv,
    //         _ => null
    //     };
    //
    //     if (targetCsv == null)
    //     {
    //         Debug.LogError($"CSV for rank {rank} is null");
    //         return null;
    //     }
    //
    //     using (StringReader reader = new StringReader(targetCsv.text))
    //     using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
    //     {
    //         csv.Read();
    //         csv.ReadHeader();
    //
    //         IEnumerable<EquipmentStatLevelUpData> records = csv.GetRecords<EquipmentStatLevelUpData>();
    //
    //         foreach (var record in records)
    //         {
    //             if (record.Level == currentLevel + 1)
    //                 return record;
    //         }
    //     }
    //
    //     Debug.LogWarning($"Next Level {currentLevel + 1} not found in equipment stat table for rank {rank}");
    //     return null;
    // }
}