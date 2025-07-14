using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
}