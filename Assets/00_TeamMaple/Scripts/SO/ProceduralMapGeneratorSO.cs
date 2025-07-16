using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;
using UnityEngine.Serialization;

public class StageData
{
    public int Id { get; set; } //스테이지 아이디
    public string StageName { get; set; } //스테이지 이름
    public string PlanetName { get; set; } //행성 이름
    public int StageItem { get; set; } //스테이지 아이템
    public float YGeneration { get; set; } //노란꽃 생성 확률
    public float RGeneration { get; set; } //빨간꽃 생성 확률
    public float BGeneration { get; set; } //파란꽃 생성 확률
}

public class StageRewardData
{
    public int Id { get; set; }
    public string Star1Y { get; set; }
    public string Star2R { get; set; }
    public string Star3Y { get; set; }
    public string Star3R { get; set; }
}

[CreateAssetMenu(fileName = "ProceduralMapGeneratorSO", menuName = "Scriptable Object/ProceduralMapGeneratorSO")]
public class ProceduralMapGeneratorSO : ScriptableObject
{
    [Header("ProceduralMapCsv")] [SerializeField]
    private TextAsset proceduralMapCsv;

    [FormerlySerializedAs("stageRewardCSV")] [SerializeField]
    private TextAsset stageRewardCsv;

    private Dictionary<int, StageData> stageDataDic = new Dictionary<int, StageData>();

    private Dictionary<int, StageRewardData> stageRewardDataDic = new Dictionary<int, StageRewardData>();

    public void InitData()
    {
        LoadCsv();
        LoadRewardCsv();
    }

    private void LoadCsv()
    {
        if (proceduralMapCsv == null) return;

        stageDataDic.Clear(); // 기존 데이터 제거

        using (StringReader reader = new StringReader(proceduralMapCsv.text))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read(); // 주석 또는 설명 줄 스킵
            csv.ReadHeader(); // 헤더 읽기

            IEnumerable<StageData> records = csv.GetRecords<StageData>();

            foreach (StageData record in records)
            {
                if (!stageDataDic.ContainsKey(record.Id))
                {
                    stageDataDic.Add(record.Id, record);
                }
            }
        }
    }

    private void LoadRewardCsv()
    {
        if (stageRewardCsv == null) return;

        stageRewardDataDic.Clear(); // 기존 데이터 제거

        using (StringReader reader = new StringReader(stageRewardCsv.text))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read(); // 주석 또는 설명 줄 스킵
            csv.ReadHeader(); // 헤더 읽기

            IEnumerable<StageRewardData> records = csv.GetRecords<StageRewardData>();

            foreach (StageRewardData record in records)
            {
                if (!stageRewardDataDic.ContainsKey(record.Id))
                {
                    stageRewardDataDic.Add(record.Id, record);
                }
            }
        }
    }

    /// <summary>
    /// 입력 받은 아이디에 따라 데이터 반환
    /// </summary>
    public StageData GetStageData(int id)
    {
        if (stageDataDic.TryGetValue(id, out StageData data))
        {
            return data;
        }
        
        return null;
    }

    public StageRewardData GetStageRewardData(int id)
    {
        if (stageRewardDataDic.TryGetValue(id, out StageRewardData data))
        {
            return data;
        }

        return null;
    }
}