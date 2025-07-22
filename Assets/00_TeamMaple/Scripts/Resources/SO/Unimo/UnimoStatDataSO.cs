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

[CreateAssetMenu(fileName = "UnimoStatDataSO", menuName = "Scriptable Object/UnimoStatDataSO")]
public class UnimoStatDataSO : ScriptableObject
{
    [Header("UnimoStatDataCsv")]
    [SerializeField] private TextAsset unimoStatDataCsv;
    
    [Header("UnimoStatLevelUpDataSO")]
    [SerializeField] private UnimoStatLevelUpDataSO unimoStatLevelUpDataSO;

    // UnimoStatData + 레벨업 데이터 적용 최종값 반환
    public UnimoStatData GetFinalUnimoStatData(int unimoID, int level)
    {
        UnimoStatData baseData = GetUnimoStatData(unimoID);
        if (baseData == null)
            return null;

        UnimoStatLevelUpData levelUpData = null;
        if (unimoStatLevelUpDataSO != null)
            levelUpData = unimoStatLevelUpDataSO.GetUnimoStatLevelUpData(level);

        // 특화 스탯 모음
        HashSet<UnimoStat> specialStats = new HashSet<UnimoStat>
        {
            baseData.SpecialStat1,
            baseData.SpecialStat2,
            baseData.SpecialStat3
        };

        // "All"이 특화스탯에 있으면 플래그
        bool isAll = specialStats.Contains(UnimoStat.All);
        
        // 가중치
        float weight = 1f;
        switch (baseData.Rank)
        {
            case UnimoRank.N: weight = 1.1f; break;
            case UnimoRank.P: weight = 1.2f; break;
        }
        
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
    
    // 특화스탯/All일 때 가중치 적용
    private float GetLevelUpWeighted(float baseValue, UnimoStat stat, HashSet<UnimoStat> specialStats, bool isAll, float weight)
    {
        if (stat == UnimoStat.None) return baseValue; // None은 항상 가중치 미적용
        return isAll || specialStats.Contains(stat) ? baseValue * weight : baseValue;
    }
}
