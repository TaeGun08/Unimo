using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;

// N��� ���� ��ȭ ������
[System.Serializable]
public class NRankEquipmentStatLevelUpData
{
    // �� �� �� ������ �ش��ϴ� ���ȸ� ��ȭ��
    public int Level { get; set; }
    public int Hp { get; set; }
    public int Def { get; set; }
    public int Speed { get; set; }
    public int BloomRange { get; set; }
    public float BloomSpeed { get; set; }
    public float FlowerRate { get; set; }
    public float RareFlowerRate { get; set; }
    public float Dodge { get; set; }
    public float StunRecovery { get; set; }
    public float HpRecovery { get; set; }
    public float FlowerDropSpeed { get; set; }
    public float FlowerDropAmount { get; set; }
}

[CreateAssetMenu(fileName = "EquipmentStatLevelUpDataSO", menuName = "Scriptable Object/NRankEquipmentStatLevelUpDataSO")]
public class EquipmentStatLevelUpDataSO : ScriptableObject
{
    [Header("NRankEquipmentStatLevelUpDataCsv")]
    [SerializeField] private TextAsset nRankEquipmentStatLevelUpDataCsv;

    /// <summary>
    /// �Է� ���� ���̵� ���� ������ ��ȯ
    /// </summary>
    public NRankEquipmentStatLevelUpData GetEquipmentStatLevelUpData(int equipmentSkillLevel)
    {
        if (nRankEquipmentStatLevelUpDataCsv == null)
        {
            Debug.LogError("NRankEquipmentStatLevelUpDataCsv is null");
            return null;
        }

        using (StringReader reader = new StringReader(nRankEquipmentStatLevelUpDataCsv.text))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read(); // unimo_id, name, hp, ...
            csv.ReadHeader();   // ��� ���
            
            IEnumerable<NRankEquipmentStatLevelUpData> records = csv.GetRecords<NRankEquipmentStatLevelUpData>();

            foreach (NRankEquipmentStatLevelUpData record in records)
            {
                if (record.Level == equipmentSkillLevel)
                    return record;
            }
        }

        Debug.LogWarning($"NRankEquipmentStatLevelUpData with ID {equipmentSkillLevel} not found.");
        return null;
    }
}