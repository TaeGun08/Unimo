using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;

[System.Serializable]
public class StageDataRecord : IStageData
{
    public int Id { get; set; }
    public string StageName { get; set; } //스테이지 이름
    public string PlanetName { get; set; } //행성 이름
}

[System.Serializable]
public class StageRewardDataRecord : IStageData
{
    public int Id { get; set; } //스테이지 보상 아이디
    public string Star1Y { get; set; } //스테이지 1별 달성 보상
    public string Star2R { get; set; } //스테이지 2별 달성 보상
    public string Star3Y { get; set; } //스테이지 3별 달성 보상
    public string Star3R { get; set; } //스테이지 3별 달성 보상
}

[System.Serializable]
public class StageGenerateFlowerDataRecord : IStageData
{
    public int Id { get; set; }
    public float YGeneration { get; set; } //노란꽃 생성 확률
    public float RGeneration { get; set; } //빨간꽃 생성 확률
    public float BGeneration { get; set; } //파란꽃 생성 확률
}

public abstract class ParsingStageData<TRecord> : ScriptableObject where TRecord : class, IStageData
{
    protected Dictionary<int, TRecord> dataDic = new();

    [SerializeField] private TextAsset stageCsv;

    protected void OnEnable()
    {
        LoadCsv();
    }

    protected void LoadCsv()
    {
        if (stageCsv == null) return;

        dataDic.Clear();

        var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null // 에러 방지용
        };

        using var reader = new StringReader(stageCsv.text);
        using var csv = new CsvReader(reader, config);

        csv.Read();
        csv.ReadHeader();

        var records = csv.GetRecords<TRecord>();
        foreach (var record in records)
        {
            if (!dataDic.ContainsKey(record.Id))
            {
                dataDic.Add(record.Id, record);
            }
        }
    }

    public TRecord GetData(int id)
    {
        dataDic.TryGetValue(id, out var data);
        return data;
    }
}

[CreateAssetMenu(fileName = "StageDataSO", menuName = "Scriptable Object/StageDataSO")]
public class StageData : ParsingStageData<StageDataRecord> { }

[CreateAssetMenu(fileName = "StageRewardDataSO", menuName = "Scriptable Object/StageRewardDataSO")]
public class StageRewardData : ParsingStageData<StageRewardDataRecord> { }

[CreateAssetMenu(fileName = "StageGenerateFlowerDataSO", menuName = "Scriptable Object/StageGenerateFlowerDataSO")]
public class StageGenerateFlowerData : ParsingStageData<StageGenerateFlowerDataRecord> { }
