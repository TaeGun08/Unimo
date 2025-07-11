using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;

[System.Serializable]
public class EquipmentSkillLevelUpData
{
    public int Id { get; set; }    // 엔진 스킬 아이디
    public string Name { get; set; }    // 엔진 스킬 이름
    public EquipmentSkillType Type { get; set; }    // 엔진 스킬 타입 (패시브 or 액티브)
    public int Cooldown { get; set; }   // 엔진 스킬 쿨타임
    public int Duration { get; set; }   // 엔진 스킬 지속시간
    public float Param { get; set; }   // 엔진 스킬 파라미터
    public string Description { get; set; }    // 엔진 스킬 설명
}

[CreateAssetMenu(fileName = "EquipmentSkillLevelUpDataSO", menuName = "Scriptable Object/EquipmentSkillLevelUpDataSO")]
public class EquipmentSkillLevelUpDataSO : ScriptableObject
{
    [Header("EquipmentSkillLevelUpCsv")]
    [SerializeField] private TextAsset equipmentSkillLevelUpCsv;

    /// <summary>
    /// 입력 받은 아이디에 따라 데이터 반환
    /// </summary>
    public EquipmentSkillLevelUpData GetEquipmentSkillLevelUpData(int equipmentSkillID)
    {
        if (equipmentSkillLevelUpCsv == null)
        {
            Debug.LogError("EquipmentSkillLevelUpCsv is null");
            return null;
        }

        using (StringReader reader = new StringReader(equipmentSkillLevelUpCsv.text))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read(); // unimo_id, name, hp, ...
            csv.ReadHeader();   // 헤더 등록
            
            IEnumerable<EquipmentSkillLevelUpData> records = csv.GetRecords<EquipmentSkillLevelUpData>();

            foreach (EquipmentSkillLevelUpData record in records)
            {
                if (record.Id == equipmentSkillID)
                    return record;
            }
        }

        Debug.LogWarning($"EquipmentSkillLevelUpData with ID {equipmentSkillID} not found.");
        return null;
    }
}
