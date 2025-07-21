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
    public int Id { get; set; }    // 유니모 아이디
    public int Level { get; set; }    // 유니모 레벨
    public string Name { get; set; }    // 유니모 이름
    public UnimoRank Rank { get; set; }    // 유니모 등급
    public UnimoStat SpecialStat1 { get; set; }    // 유니모 특화 스탯 1
    public UnimoStat SpecialStat2 { get; set; }    // 유니모 특화 스탯 2
    public UnimoStat SpecialStat3 { get; set; }    // 유니모 특화 스탯 3
    public int Hp { get; set; }    // 유니모 체력
    public int Def { get; set; }    // 유니모 방어력
    public float Speed { get; set; }    // 유니모 속도
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

[System.Serializable]
public class UnimoNextStatData
{
    public int Level { get; set; }    // 유니모 레벨
    public int PlusHp { get; set; }    // 유니모 체력
    public int PlusDef { get; set; }    // 유니모 방어력
    public float PlusSpeed { get; set; }    // 유니모 속도
    public int PlusBloomRange { get; set; }    // 개화 범위
    public float PlusBloomSpeed { get; set; }    // 개화 속도
    public float PlusFlowerRate { get; set; }    // 꽃 생성 주기
    public float PlusRareFlowerRate { get; set; }    // 희귀 꽃 생성 주기 
    public float PlusDodge { get; set; }    // 회피율
    public float PlusStunRecovery { get; set; }    // 스턴 회복력
    public float PlusHpRecovery { get; set; }    // 체력 회복력
    public float PlusFlowerDropSpeed { get; set; }    // 꽃 낙하 속도
    public float PlusFlowerDropAmount { get; set; }    // 꽃 낙하량
}

[CreateAssetMenu(fileName = "UnimoStatDataSO", menuName = "Scriptable Object/UnimoStatDataSO")]
public class UnimoStatDataSO : ScriptableObject
{
    [Header("UnimoStatDataCsv")]
    [SerializeField] private TextAsset unimoStatDataCsv;
    [SerializeField] private TextAsset unimoLevelDataCsv;
    
    [Header("UnimoStatLevelUpDataSO")]
    [SerializeField] private UnimoStatLevelUpDataSO unimoStatLevelUpDataSO;

    // UnimoStatData + 레벨업 데이터 적용 최종값 반환
    public UnimoStatData GetFinalUnimoStatData(int unimoID)
    {
        UnimoStatData baseData = GetUnimoStatData(unimoID);
        if (baseData == null)
            return null;

        UnimoStatLevelUpData levelUpData = null;
        if (unimoStatLevelUpDataSO != null)
            levelUpData = unimoStatLevelUpDataSO.GetUnimoStatLevelUpData(Base_Mng.Data.data.CharLevel[Base_Mng.Data.data.CharCount - 1]);

        // 복사본 생성 (기존 데이터 수정 방지)
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
            // 레벨업 데이터 반영 (null일 경우 무시)
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

    // 기존 데이터 로드 함수
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
