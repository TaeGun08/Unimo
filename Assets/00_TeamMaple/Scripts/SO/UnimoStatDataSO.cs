using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.IO;
using CsvHelper;

public enum UnimoRank
{
    Normal,
    Premium
}

public enum UnimoStat
{
    None,
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
public class UnimoData
{
    public int ID { get; set; }    // ���ϸ� ���̵�
    public int Level { get; set; }    // ���ϸ� ����
    public string Name { get; set; }    // ���ϸ� �̸�
    public UnimoRank Rank { get; set; }    // ���ϸ� ���
    public UnimoStat SpecialStat1 { get; set; }    // ���ϸ� Ưȭ ���� 1
    public UnimoStat SpecialStat2 { get; set; }    // ���ϸ� Ưȭ ���� 2
    public UnimoStat SpecialStat3 { get; set; }    // ���ϸ� Ưȭ ���� 3
    public int Hp { get; set; }    // ���ϸ� ü��
    public int Def { get; set; }    // ���ϸ� ����
    public int Speed { get; set; }    // ���ϸ� �ӵ�
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
    [Header("UnimoDataCsv")]
    [SerializeField] private TextAsset unimoDataCsv;

    /// <summary>
    /// �Է� ���� ���̵� ���� ������ ��ȯ
    /// </summary>
    public UnimoData GetUnimoData(int unimoID)
    {
        if (unimoDataCsv == null)
        {
            Debug.LogError("UnimoDataCsv is null");
            return null;
        }

        using (StringReader reader = new StringReader(unimoDataCsv.text))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read(); // unimo_id, name, hp, ...
            csv.ReadHeader();   // ��� ���
            
            IEnumerable<UnimoData> records = csv.GetRecords<UnimoData>();

            foreach (UnimoData record in records)
            {
                if (record.ID == unimoID)
                    return record;
            }
        }

        Debug.LogWarning($"UnimoData with ID {unimoID} not found.");
        return null;
    }
    
    /// <summary>
    /// �ӽ� ���ϸ� ������ 
    /// </summary>
    public UnimoData CreateDefaultUnimo()
    {
        return new UnimoData
        {
            ID = 0,
            Level = 1,
            Name = "Unimo",

            Rank = UnimoRank.Normal,

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
}