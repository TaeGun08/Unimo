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
public class EquipmentSkillData
{
    public int Id { get; set; }    // ���� ��ų ���̵�
    public string Name { get; set; }    // ���� ��ų �̸�
    public EquipmentSkillType Type { get; set; }    // ���� ��ų Ÿ�� (�нú� or ��Ƽ��)
    public int Cooldown { get; set; }   // ���� ��ų ��Ÿ��
    public int Duration { get; set; }   // ���� ��ų ���ӽð�
    public float Param { get; set; }   // ���� ��ų �Ķ����
    public string Description { get; set; }    // ���� ��ų ����
}

[CreateAssetMenu(fileName = "EquimentSkillDataSO", menuName = "Scriptable Object/EquimentSkillDataSO")]
public class EquipmentSkillDataSO : ScriptableObject
{
    [Header("EquimentSkillCsv")]
    [SerializeField] private TextAsset equipmentSkillCsv;

    /// <summary>
    /// �Է� ���� ���̵� ���� ������ ��ȯ
    /// </summary>
    public EquipmentSkillData GetEquipmentSkillData(int equipmentSkillID)
    {
        if (equipmentSkillCsv == null)
        {
            Debug.LogError("EquimentSkillCsv is null");
            return null;
        }

        using (StringReader reader = new StringReader(equipmentSkillCsv.text))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read(); // unimo_id, name, hp, ...
            csv.ReadHeader();   // ��� ���
            
            IEnumerable<EquipmentSkillData> records = csv.GetRecords<EquipmentSkillData>();

            foreach (EquipmentSkillData record in records)
            {
                if (record.Id == equipmentSkillID)
                    return record;
            }
        }

        Debug.LogWarning($"EquipmentSkillData with ID {equipmentSkillID} not found.");
        return null;
    }
}