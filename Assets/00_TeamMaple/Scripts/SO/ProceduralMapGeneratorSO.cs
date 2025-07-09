using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;

public class StageData
{
    public int Id { get; set; } //스테이지 아이디
    public string StageName { get; set; } //스테이지 이름
    public string PlanetName { get; set; } //행성 이름
    public int StageReward { get; set; } //스테이지 보상
    public int StageItem { get; set; } //스테이지 아이템
    public float YGeneration { get; set; } //노란꽃 생성 확률
    public float RGeneration { get; set; } //빨간꽃 생성 확률
    public float BGeneration { get; set; } //파란꽃 생성 확률
}

[CreateAssetMenu(fileName = "ProceduralMapGeneratorSO", menuName = "Scriptable Object/ProceduralMapGeneratorSO")]
public class ProceduralMapGeneratorSO : ScriptableObject
{
    [Header("ProceduralMapCsv")]
    [SerializeField] private TextAsset proceduralMapCsv;
    private Dictionary<int, StageData> stageDataDic = new Dictionary<int, StageData>();
    public Dictionary<int, StageData> StageDataDic => stageDataDic;

    private void Awake()
    {
        if (stageDataDic.Count == 0)
            LoadCsv();
    }
    
    private void LoadCsv()
    {
        if (proceduralMapCsv == null)
        {
            Debug.LogError("proceduralMapCsv is null");
            return;
        }

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
                else
                {
                    Debug.LogWarning($"Duplicate StageData ID found: {record.Id}");
                }
            }
        }

        Debug.Log($"Loaded {stageDataDic.Count} stage data entries.");
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

        Debug.LogWarning($"StageData with ID {id} not found.");
        return null;
    }
}
