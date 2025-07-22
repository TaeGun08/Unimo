using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;

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