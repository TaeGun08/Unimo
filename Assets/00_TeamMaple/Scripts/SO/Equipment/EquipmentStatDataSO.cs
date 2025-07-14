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

    /// <summary>
    /// 입력 받은 아이디에 따라 데이터 반환
    /// </summary>
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
}
