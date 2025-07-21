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

[System.Serializable]
public class UnimoNextStatData
{
    public int Level { get; set; }    // ���ϸ� ����
    public int PlusHp { get; set; }    // ���ϸ� ü��
    public int PlusDef { get; set; }    // ���ϸ� ����
    public float PlusSpeed { get; set; }    // ���ϸ� �ӵ�
    public int PlusBloomRange { get; set; }    // ��ȭ ����
    public float PlusBloomSpeed { get; set; }    // ��ȭ �ӵ�
    public float PlusFlowerRate { get; set; }    // �� ���� �ֱ�
    public float PlusRareFlowerRate { get; set; }    // ��� �� ���� �ֱ� 
    public float PlusDodge { get; set; }    // ȸ����
    public float PlusStunRecovery { get; set; }    // ���� ȸ����
    public float PlusHpRecovery { get; set; }    // ü�� ȸ����
    public float PlusFlowerDropSpeed { get; set; }    // �� ���� �ӵ�
    public float PlusFlowerDropAmount { get; set; }    // �� ���Ϸ�
}

[CreateAssetMenu(fileName = "UnimoStatDataSO", menuName = "Scriptable Object/UnimoStatDataSO")]
public class UnimoStatDataSO : ScriptableObject
{
    [Header("UnimoStatDataCsv")]
    [SerializeField] private TextAsset unimoStatDataCsv;
    [SerializeField] private TextAsset unimoLevelDataCsv;
    
    [Header("UnimoStatLevelUpDataSO")]
    [SerializeField] private UnimoStatLevelUpDataSO unimoStatLevelUpDataSO;

    // UnimoStatData + ������ ������ ���� ������ ��ȯ
    public UnimoStatData GetFinalUnimoStatData(int unimoID)
    {
        UnimoStatData baseData = GetUnimoStatData(unimoID);
        if (baseData == null)
            return null;

        UnimoStatLevelUpData levelUpData = null;
        if (unimoStatLevelUpDataSO != null)
            levelUpData = unimoStatLevelUpDataSO.GetUnimoStatLevelUpData(Base_Mng.Data.data.CharLevel[Base_Mng.Data.data.CharCount - 1]);

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
            // ������ ������ �ݿ� (null�� ��� ����)
            merged.Hp += levelUpData.PlusHp;
            merged.Def += levelUpData.PlusDef;
            merged.Speed += levelUpData.PlusSpeed;
            merged.BloomRange += levelUpData.PlusBloomRange;
            merged.BloomSpeed += levelUpData.PlusBloomSpeed;
            merged.FlowerRate += levelUpData.PlusFlowerRate;
            merged.RareFlowerRate += levelUpData.PlusRareFlowerRate;
            merged.Dodge += levelUpData.PlusDodge;
            merged.StunRecovery += levelUpData.PlusStunRecovery;
            merged.HpRecovery += levelUpData.PlusHpRecovery;
            merged.FlowerDropSpeed += levelUpData.PlusFlowerDropSpeed;
            merged.FlowerDropAmount += levelUpData.PlusFlowerDropAmount;
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
    
    public UnimoNextStatData GetCurrentAndNextStat(int currentLevel)
    {
        if (unimoLevelDataCsv == null)
        {
            Debug.LogError("unimoLevelDataCsv is null");
            return null;
        }

        using (StringReader reader = new StringReader(unimoLevelDataCsv.text))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read();
            csv.ReadHeader();

            List<UnimoNextStatData> allData = csv.GetRecords<UnimoNextStatData>().ToList();

            var next = allData.FirstOrDefault(x => x.Level == currentLevel + 1);

            if (next == null)
            {
                Debug.LogWarning($"No next level data found for level {currentLevel + 1}");
                return null;
            }

            UnimoNextStatData nextStat = new UnimoNextStatData
            {
                Level = next.Level,
                PlusHp = next.PlusHp,
                PlusDef = next.PlusDef,
                PlusSpeed = next.PlusSpeed,
                PlusBloomRange = next.PlusBloomRange,
                PlusBloomSpeed = next.PlusBloomSpeed,
                PlusFlowerRate = next.PlusFlowerRate,
                PlusRareFlowerRate = next.PlusRareFlowerRate,
                PlusDodge = next.PlusDodge,
                PlusStunRecovery = next.PlusStunRecovery,
                PlusHpRecovery = next.PlusHpRecovery,
                PlusFlowerDropSpeed = next.PlusFlowerDropSpeed,
                PlusFlowerDropAmount = next.PlusFlowerDropAmount
            };

            return nextStat;
        }
    }

}
