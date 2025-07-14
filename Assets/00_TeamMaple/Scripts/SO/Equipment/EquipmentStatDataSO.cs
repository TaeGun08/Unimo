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
    public int Id { get; set; }    // ���� ���̵�
    public int Level { get; set; }    // ���� ����
    public string Name { get; set; }    // ���� �̸�
    public EquipmentRank Rank { get; set; }    // ���� ���
    public UnimoStat StatType1 { get; set; }    // ��ȭ�� ���ϸ� ���� 1
    public float StatValue1 { get; set; }    // ��ȭ�� �� 1
    public UnimoStat StatType2 { get; set; }    // ��ȭ�� ���ϸ� ���� 2
    public float StatValue2 { get; set; }    // ��ȭ�� �� 2
    public UnimoStat StatType3 { get; set; }    // ��ȭ�� ���ϸ� ���� 3
    public float StatValue3 { get; set; }    // ��ȭ�� �� 3
    public UnimoStat StatType4 { get; set; }    // ��ȭ�� ���ϸ� ���� 4
    public float StatValue4 { get; set; }    // ��ȭ�� �� 4
    public int Skill1 { get; set; }    // ���� ��ų ���̵� 1
    public int Skill2 { get; set; }    // ���� ��ų ���̵� 2
}

[CreateAssetMenu(fileName = "EquipmentStatDataSO", menuName = "Scriptable Object/EquipmentStatDataSO")]
public class EquipmentStatDataSO : ScriptableObject
{
    [Header("EquipmentStatDataCsv")]
    [SerializeField] private TextAsset equipmentStatDataCsv;

    /// <summary>
    /// �Է� ���� ���̵� ���� ������ ��ȯ
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
            csv.ReadHeader();   // ��� ���
            
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
