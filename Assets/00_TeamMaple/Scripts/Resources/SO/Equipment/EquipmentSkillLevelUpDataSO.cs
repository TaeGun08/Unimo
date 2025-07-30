using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;

[System.Serializable]
public class EquipmentSkillLevelUpData
{
    public int Id { get; set; }
    public int Level { get; set; }
    public float Cooldown { get; set; }
    public float Duration { get; set; }
    public float Param { get; set; }   // 엔진 스킬 파라미터
    public string Description { get; set; }
}

[CreateAssetMenu(fileName = "EquipmentSkillLevelUpDataSO", menuName = "Scriptable Object/EquipmentSkillLevelUpDataSO")]
public class EquipmentSkillLevelUpDataSO : ScriptableObject
{
    [Header("EquipmentSkillLevelUpDataCsv")]
    [SerializeField] private TextAsset equipmentSkillLevelUpDataCsv;

    /// <summary>
    /// 입력 받은 아이디에 따라 데이터 반환
    /// </summary>
    public EquipmentSkillLevelUpData GetEquipmentSkillLevelUpData(int id, int level)
    {
        if (equipmentSkillLevelUpDataCsv == null)
        {
            Debug.LogError("EquipmentSkillLevelUpDataCsv is null");
            return null;
        }

        using (StringReader reader = new StringReader(equipmentSkillLevelUpDataCsv.text))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read(); // 첫 줄 무시
            csv.ReadHeader(); // 헤더 등록

            IEnumerable<EquipmentSkillLevelUpData> records = csv.GetRecords<EquipmentSkillLevelUpData>();

            foreach (EquipmentSkillLevelUpData record in records)
            {
                if (record.Id == id && record.Level == level)
                    return record;
            }
        }

        Debug.LogWarning($"EquipmentSkillLevelUpData not found. ID: {id}, Level: {level}");
        return null;
    }
}