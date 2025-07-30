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
    public float Param { get; set; }   // ���� ��ų �Ķ����
    public string Description { get; set; }
}

[CreateAssetMenu(fileName = "EquipmentSkillLevelUpDataSO", menuName = "Scriptable Object/EquipmentSkillLevelUpDataSO")]
public class EquipmentSkillLevelUpDataSO : ScriptableObject
{
    [Header("EquipmentSkillLevelUpDataCsv")]
    [SerializeField] private TextAsset equipmentSkillLevelUpDataCsv;

    /// <summary>
    /// �Է� ���� ���̵� ���� ������ ��ȯ
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
            csv.Read(); // ù �� ����
            csv.ReadHeader(); // ��� ���

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