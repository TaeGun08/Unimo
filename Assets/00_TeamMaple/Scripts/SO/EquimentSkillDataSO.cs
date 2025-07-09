using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.IO;
using CsvHelper;

public enum EquipmentSkillType
{
    Passive,
    Active
}

[System.Serializable]
public class EquimentSkillData
{
    public int ID { get; set; }    // 엔진 스킬 아이디
    public string Name { get; set; }    // 엔진 스킬 이름
    public EquipmentSkillType Type { get; set; }    // 엔진 스킬 타입 (패시브 or 액티브)
    public int Cooldown { get; set; }   // 엔진 스킬 쿨타임
    public int Duration { get; set; }   // 엔진 스킬 지속시간
    public float Param1 { get; set; }   // 엔진 스킬 파라미터 1
    public float Param2 { get; set; }   // 엔진 스킬 파라미터 2
    public string Description { get; set; }    // 엔진 스킬 설명
}

[CreateAssetMenu(fileName = "EquimentSkillDataSO", menuName = "Scriptable Object/EquimentSkillDataSO")]
public class EquimentSkillDataSO : ScriptableObject
{
    [Header("EquimentSkillCsv")]
    [SerializeField] private TextAsset equimentSkillCsv;

    /// <summary>
    /// 입력 받은 아이디에 따라 데이터 반환
    /// </summary>
    public EquimentSkillData GetEquimentSkillData(int equimentSkillID)
    {
        if (equimentSkillCsv == null)
        {
            Debug.LogError("EquimentSkillCsv is null");
            return null;
        }

        using (StringReader reader = new StringReader(equimentSkillCsv.text))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read(); // unimo_id, name, hp, ...
            csv.ReadHeader();   // 헤더 등록
            
            IEnumerable<EquimentSkillData> records = csv.GetRecords<EquimentSkillData>();

            foreach (EquimentSkillData record in records)
            {
                if (record.ID == equimentSkillID)
                    return record;
            }
        }

        Debug.LogWarning($"EquimentSkillData with ID {equimentSkillID} not found.");
        return null;
    }
}