using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.IO;
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
    
    // ȸ���� ��� 
    public bool DodgeCalculation(float dodge)
    {
        float evadeRate =  dodge * 100f;
        int rand = Random.Range(0, 100);

        if (rand < evadeRate)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    
    // ���� ���ӽð� ���
    public float stunDuration(float recovery)
    {
        float stunRecovery =   recovery * 100f;
        return 1- stunRecovery;
    }
}

[CreateAssetMenu(fileName = "UnimoStatDataSO", menuName = "Scriptable Object/UnimoStatDataSO")]
public class UnimoStatDataSO : ScriptableObject
{
    [Header("UnimoStatDataCsv")]
    [SerializeField] private TextAsset unimoStatDataCsv;

    /// <summary>
    /// �Է� ���� ���̵� ���� ������ ��ȯ
    /// </summary>
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
            csv.Read(); // unimo_id, name, hp, ...
            csv.ReadHeader();   // ��� ���
            
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
    
    /// <summary>
    /// �ӽ� �ʱ� ���ϸ� ������ ����
    /// </summary>
    public UnimoStatData CreateDefaultUnimo()
    {
        return new UnimoStatData
        {
            Id = 0,
            Level = 1,
            Name = "Unimo",

            Rank = UnimoRank.N,

            SpecialStat1 = UnimoStat.None,
            SpecialStat2 = UnimoStat.None,
            SpecialStat3 = UnimoStat.None,

            Hp = 100,
            Def = 10,
            Speed = 1,

            BloomRange = 1,
            BloomSpeed = 1.0f,
            FlowerRate = 5.0f,
            RareFlowerRate = 20.0f,

            Dodge = 0.3f,
            StunRecovery = 1.0f,
            HpRecovery = 0.5f,
            FlowerDropSpeed = 1.0f,
            FlowerDropAmount = 1.0f
        };
    }

    /// <summary>
    /// �ӽ� ���ϸ� ������ ����
    /// </summary>
    public UnimoData SettingsUnimoData(int Id)
    {
        Debug.Log(GetUnimoData(Id));
        
        return GetUnimoData(Id);
    }
}