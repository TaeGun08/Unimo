using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using UnityEngine;

[System.Serializable]
public class UnimoStatLevelUpData
{
    public int Level { get; set; }
    public int PlusHp { get; set; }
    public int PlusDef { get; set; }
    public float PlusSpeed { get; set; }
    public int PlusBloomRange { get; set; }
    public float PlusBloomSpeed { get; set; }
    public float PlusFlowerRate { get; set; }
    public float PlusRareFlowerRate { get; set; }
    public float PlusDodge { get; set; }
    public float PlusStunRecovery { get; set; }
    public float PlusHpRecovery { get; set; }
    public float PlusFlowerDropSpeed { get; set; }
    public float PlusFlowerDropAmount { get; set; }
}

[CreateAssetMenu(fileName = "UnimoStatLevelUpDataSO", menuName = "Scriptable Object/UnimoStatLevelUpDataSO")]
public class UnimoStatLevelUpDataSO : ScriptableObject
{
    [Header("UnimoStatLevelUpDataCsv")]
    [SerializeField] private TextAsset unimoStatLevelUpDataCsv;

    /// <summary>
    /// 입력 받은 아이디에 따라 데이터 반환
    /// </summary>
    public UnimoStatLevelUpData GetUnimoStatLevelUpData(int level)
    {
        if (unimoStatLevelUpDataCsv == null)
        {
            Debug.LogError("UnimoLevelUpDataCsv is null");
            return null;
        }

        using (StringReader reader = new StringReader(unimoStatLevelUpDataCsv.text))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read(); // unimo_id, name, hp, ...
            csv.ReadHeader(); // 헤더 등록

            IEnumerable<UnimoStatLevelUpData> records = csv.GetRecords<UnimoStatLevelUpData>();

            foreach (UnimoStatLevelUpData record in records)
            {
                if (record.Level == level)
                    return record;
            }
        }

        Debug.LogWarning($"UnimoStatLevelUpData with Level {level} not found.");
        return null;
    }
    
    public UnimoStatLevelUpData GetCurrentAndNextStat(int currentLevel)
    {
        if (unimoStatLevelUpDataCsv == null)
        {
            Debug.LogError("unimoLevelDataCsv is null");
            return null;
        }

        using (StringReader reader = new StringReader(unimoStatLevelUpDataCsv.text))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read();
            csv.ReadHeader();

            List<UnimoStatLevelUpData> allData = csv.GetRecords<UnimoStatLevelUpData>().ToList();

            var next = allData.FirstOrDefault(x => x.Level == currentLevel + 1);
            var current = allData.FirstOrDefault(x => x.Level == currentLevel);
            
            if (next == null)
            {
                Debug.LogWarning($"No next level data found for level {currentLevel + 1}");
                return null;
            }

            UnimoStatLevelUpData nextStat = new UnimoStatLevelUpData
            {
                Level = next.Level - current.Level,
                PlusHp = next.PlusHp - current.PlusHp,
                PlusDef = next.PlusDef - current.PlusDef,
                PlusSpeed = next.PlusSpeed - current.PlusSpeed,
                PlusBloomRange = next.PlusBloomRange - current.PlusBloomRange,
                PlusBloomSpeed = next.PlusBloomSpeed - current.PlusBloomSpeed,
                PlusFlowerRate = next.PlusFlowerRate - current.PlusFlowerRate,
                PlusRareFlowerRate = next.PlusRareFlowerRate - current.PlusRareFlowerRate,
                PlusDodge = next.PlusDodge - current.PlusDodge,
                PlusStunRecovery = next.PlusStunRecovery - current.PlusStunRecovery,
                PlusHpRecovery = next.PlusHpRecovery - current.PlusHpRecovery,
                PlusFlowerDropSpeed = next.PlusFlowerDropSpeed - current.PlusFlowerDropSpeed,
                PlusFlowerDropAmount = next.PlusFlowerDropAmount - current.PlusFlowerDropAmount
            };

            return nextStat;
        }
    }
}
