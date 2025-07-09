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
}