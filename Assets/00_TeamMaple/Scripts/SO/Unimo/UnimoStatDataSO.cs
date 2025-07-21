using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.IO;
using System.Linq;
using CsvHelper;

public enum UnimoRank
{
    N,
    P
}

public enum UnimoStat
{
    None,
    All,
    Hp,
    Def,
    Speed,
    BloomRange,
    BloomSpeed,
    FlowerRate,
    RareFlowerRate,
    Dodge,
    StunRecovery,
    HpRecovery,
    FlowerDropSpeed,
    FlowerDropAmount
}

[System.Serializable]
public class UnimoStatData
{
    public int Id { get; set; }    // ���ϸ� ���̵�
    public int Level { get; set; }    // ���ϸ� ����
    public string Name { get; set; }    // ���ϸ� �̸�
    public UnimoRank Rank { get; set; }    // ���ϸ� ���
    public UnimoStat SpecialStat1 { get; set; }    // ���ϸ� Ưȭ ���� 1
    public UnimoStat SpecialStat2 { get; set; }    // ���ϸ� Ưȭ ���� 2
    public UnimoStat SpecialStat3 { get; set; }    // ���ϸ� Ưȭ ���� 3
    public int Hp { get; set; }    // ���ϸ� ü��
    public int Def { get; set; }    // ���ϸ� ����
    public float Speed { get; set; }    // ���ϸ� �ӵ�
    public int BloomRange { get; set; }    // ��ȭ ����
    public float BloomSpeed { get; set; }    // ��ȭ �ӵ�
    public float FlowerRate { get; set; }    // �� ���� �ֱ�
    public float RareFlowerRate { get; set; }    // ��� �� ���� �ֱ� 
    public float Dodge { get; set; }    // ȸ����
    public float StunRecovery { get; set; }    // ���� ȸ����
    public float HpRecovery { get; set; }    // ü�� ȸ����
    public float FlowerDropSpeed { get; set; }    // �� ���� �ӵ�
    public float FlowerDropAmount { get; set; }    // �� ���Ϸ�
}

[CreateAssetMenu(fileName = "UnimoStatDataSO", menuName = "Scriptable Object/UnimoStatDataSO")]
public class UnimoStatDataSO : ScriptableObject
{
    [Header("UnimoStatDataCsv")]
    [SerializeField] private TextAsset unimoStatDataCsv;
    
    [Header("UnimoStatLevelUpDataSO")]
    [SerializeField] private UnimoStatLevelUpDataSO unimoStatLevelUpDataSO;

    // UnimoStatData + ������ ������ ���� ������ ��ȯ
    public UnimoStatData GetFinalUnimoStatData(int unimoID, int level)
    {
        UnimoStatData baseData = GetUnimoStatData(unimoID);
        if (baseData == null)
            return null;

        UnimoStatLevelUpData levelUpData = null;
        if (unimoStatLevelUpDataSO != null)
            levelUpData = unimoStatLevelUpDataSO.GetUnimoStatLevelUpData(level);

        // Ưȭ ���� ����
        HashSet<UnimoStat> specialStats = new HashSet<UnimoStat>
        {
            baseData.SpecialStat1,
            baseData.SpecialStat2,
            baseData.SpecialStat3
        };

        // "All"�� Ưȭ���ȿ� ������ �÷���
        bool isAll = specialStats.Contains(UnimoStat.All);
        
        // ����ġ
        float weight = 1f;
        switch (baseData.Rank)
        {
            case UnimoRank.N: weight = 1.1f; break;
            case UnimoRank.P: weight = 1.2f; break;
        }
        
        // ���纻 ���� (���� ������ ���� ����)
        UnimoStatData merged = new UnimoStatData
        {
            Id = baseData.Id,
            Level = baseData.Level,
            Name = baseData.Name,
            Rank = baseData.Rank,
            SpecialStat1 = baseData.SpecialStat1,
            SpecialStat2 = baseData.SpecialStat2,
            SpecialStat3 = baseData.SpecialStat3,
            Hp = baseData.Hp,
            Def = baseData.Def,
            Speed = baseData.Speed,
            BloomRange = baseData.BloomRange,
            BloomSpeed = baseData.BloomSpeed,
            FlowerRate = baseData.FlowerRate,
            RareFlowerRate = baseData.RareFlowerRate,
            Dodge = baseData.Dodge,
            StunRecovery = baseData.StunRecovery,
            HpRecovery = baseData.HpRecovery,
            FlowerDropSpeed = baseData.FlowerDropSpeed,
            FlowerDropAmount = baseData.FlowerDropAmount
        };

        if (levelUpData != null)
        {
            merged.Hp += Mathf.RoundToInt(GetLevelUpWeighted(levelUpData.PlusHp, UnimoStat.Hp, specialStats, isAll, weight));
            merged.Def += Mathf.RoundToInt(GetLevelUpWeighted(levelUpData.PlusDef, UnimoStat.Def, specialStats, isAll, weight));
            merged.Speed += GetLevelUpWeighted(levelUpData.PlusSpeed, UnimoStat.Speed, specialStats, isAll, weight);
            merged.BloomRange += Mathf.RoundToInt(GetLevelUpWeighted(levelUpData.PlusBloomRange, UnimoStat.BloomRange, specialStats, isAll, weight));
            merged.BloomSpeed += GetLevelUpWeighted(levelUpData.PlusBloomSpeed, UnimoStat.BloomSpeed, specialStats, isAll, weight);
            merged.FlowerRate += GetLevelUpWeighted(levelUpData.PlusFlowerRate, UnimoStat.FlowerRate, specialStats, isAll, weight);
            merged.RareFlowerRate += GetLevelUpWeighted(levelUpData.PlusRareFlowerRate, UnimoStat.RareFlowerRate, specialStats, isAll, weight);
            merged.Dodge += GetLevelUpWeighted(levelUpData.PlusDodge, UnimoStat.Dodge, specialStats, isAll, weight);
            merged.StunRecovery += GetLevelUpWeighted(levelUpData.PlusStunRecovery, UnimoStat.StunRecovery, specialStats, isAll, weight);
            merged.HpRecovery += GetLevelUpWeighted(levelUpData.PlusHpRecovery, UnimoStat.HpRecovery, specialStats, isAll, weight);
            merged.FlowerDropSpeed += GetLevelUpWeighted(levelUpData.PlusFlowerDropSpeed, UnimoStat.FlowerDropSpeed, specialStats, isAll, weight);
            merged.FlowerDropAmount += GetLevelUpWeighted(levelUpData.PlusFlowerDropAmount, UnimoStat.FlowerDropAmount, specialStats, isAll, weight);
        }

        return merged;
    }

    // ���� ������ �ε� �Լ�
    public UnimoStatData GetUnimoStatData(int unimoID)
    {
        if (unimoStatDataCsv == null)
        {
            Debug.LogError("UnimoStatDataCsv is null");
            return null;
        }

        using (StringReader reader = new StringReader(unimoStatDataCsv.text))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read();
            csv.ReadHeader();

            IEnumerable<UnimoStatData> records = csv.GetRecords<UnimoStatData>();

            foreach (UnimoStatData record in records)
            {
                if (record.Id == unimoID)
                    return record;
            }
        }

        Debug.LogWarning($"UnimoStatData with ID {unimoID} not found.");
        return null;
    }
    
    // Ưȭ����/All�� �� ����ġ ����
    private float GetLevelUpWeighted(float baseValue, UnimoStat stat, HashSet<UnimoStat> specialStats, bool isAll, float weight)
    {
        if (stat == UnimoStat.None) return baseValue; // None�� �׻� ����ġ ������
        return isAll || specialStats.Contains(stat) ? baseValue * weight : baseValue;
    }
}
