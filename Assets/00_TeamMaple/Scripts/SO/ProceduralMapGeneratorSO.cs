using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;
using UnityEngine.Serialization;

public class StageData
{
    public int Id { get; set; } //�������� ���̵�
    public string StageName { get; set; } //�������� �̸�
    public string PlanetName { get; set; } //�༺ �̸�
    public int StageItem { get; set; } //�������� ������
    public float YGeneration { get; set; } //����� ���� Ȯ��
    public float RGeneration { get; set; } //������ ���� Ȯ��
    public float BGeneration { get; set; } //�Ķ��� ���� Ȯ��
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

        stageDataDic.Clear(); // ���� ������ ����

        using (StringReader reader = new StringReader(proceduralMapCsv.text))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read(); // �ּ� �Ǵ� ���� �� ��ŵ
            csv.ReadHeader(); // ��� �б�

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

        stageRewardDataDic.Clear(); // ���� ������ ����

        using (StringReader reader = new StringReader(stageRewardCsv.text))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read(); // �ּ� �Ǵ� ���� �� ��ŵ
            csv.ReadHeader(); // ��� �б�

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
    /// �Է� ���� ���̵� ���� ������ ��ȯ
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