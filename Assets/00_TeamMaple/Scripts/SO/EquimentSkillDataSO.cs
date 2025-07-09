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
    public int ID { get; set; }    // ���� ��ų ���̵�
    public string Name { get; set; }    // ���� ��ų �̸�
    public EquipmentSkillType Type { get; set; }    // ���� ��ų Ÿ�� (�нú� or ��Ƽ��)
    public int Cooldown { get; set; }   // ���� ��ų ��Ÿ��
    public int Duration { get; set; }   // ���� ��ų ���ӽð�
    public float Param1 { get; set; }   // ���� ��ų �Ķ���� 1
    public float Param2 { get; set; }   // ���� ��ų �Ķ���� 2
    public string Description { get; set; }    // ���� ��ų ����
}

[CreateAssetMenu(fileName = "EquimentSkillDataSO", menuName = "Scriptable Object/EquimentSkillDataSO")]
public class EquimentSkillDataSO : ScriptableObject
{
    [Header("EquimentSkillCsv")]
    [SerializeField] private TextAsset equimentSkillCsv;

    /// <summary>
    /// �Է� ���� ���̵� ���� ������ ��ȯ
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
            csv.ReadHeader();   // ��� ���
            
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