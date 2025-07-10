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
    public int ID { get; set; }    // 유니모 아이디
    public int Level { get; set; }    // 유니모 레벨
    public string Name { get; set; }    // 유니모 이름
    public UnimoRank Rank { get; set; }    // 유니모 등급
    public UnimoStat SpecialStat1 { get; set; }    // 유니모 특화 스탯 1
    public UnimoStat SpecialStat2 { get; set; }    // 유니모 특화 스탯 2
    public UnimoStat SpecialStat3 { get; set; }    // 유니모 특화 스탯 3
    public int Hp { get; set; }    // 유니모 체력
    public int Def { get; set; }    // 유니모 방어력
    public int Speed { get; set; }    // 유니모 속도
    public int BloomRange { get; set; }    // 개화 범위
    public float BloomSpeed { get; set; }    // 개화 속도
    public float FlowerRate { get; set; }    // 꽃 생성 주기
    public float RareFlowerRate { get; set; }    // 희귀 꽃 생성 주기 
    public float Dodge { get; set; }    // 회피율
    public float StunRecovery { get; set; }    // 스턴 회복력
    public float HpRecovery { get; set; }    // 체력 회복력
    public float FlowerDropSpeed { get; set; }    // 꽃 낙하 속도
    public float FlowerDropAmount { get; set; }    // 꽃 낙하량
    
    // 회피율 계산 
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
    
    // 스턴 지속시간 계산
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
    /// 입력 받은 아이디에 따라 데이터 반환
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
            csv.ReadHeader();   // 헤더 등록
            
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
    /// 임시 유니모 데이터 
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