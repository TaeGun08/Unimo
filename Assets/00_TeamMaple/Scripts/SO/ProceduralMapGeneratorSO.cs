using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;

public class StageData
{
    public int Id { get; set; } //�������� ���̵�
    public string StageName { get; set; } //�������� �̸�
    public string PlanetName { get; set; } //�༺ �̸�
    public int StageReward { get; set; } //�������� ����
    public int StageItem { get; set; } //�������� ������
    public float YGeneration { get; set; } //����� ���� Ȯ��
    public float RGeneration { get; set; } //������ ���� Ȯ��
    public float BGeneration { get; set; } //�Ķ��� ���� Ȯ��
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
                else
                {
                    Debug.LogWarning($"Duplicate StageData ID found: {record.Id}");
                }
            }
        }

        Debug.Log($"Loaded {stageDataDic.Count} stage data entries.");
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

        Debug.LogWarning($"StageData with ID {id} not found.");
        return null;
    }
}
