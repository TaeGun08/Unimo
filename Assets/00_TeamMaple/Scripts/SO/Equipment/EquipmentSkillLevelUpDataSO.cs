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
    public int Cooldown { get; set; }
    public int Duration { get; set; }
    public float Param { get; set; }   // ���� ��ų �Ķ����
    public string Description { get; set; }
}

[CreateAssetMenu(fileName = "EquipmentSkillLevelUpDataSO", menuName = "Scriptable Object/EquipmentSkillLevelUpDataSO")]
public class EquipmentSkillLevelUpDataSO : ScriptableObject
{
    [Header("EquipmentSkillLevelUpCsv")]
    [SerializeField] private TextAsset equipmentSkillLevelUpCsv;

    /// <summary>
    /// �Է� ���� ���̵� ���� ������ ��ȯ
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
            csv.ReadHeader();   // ��� ���
            
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