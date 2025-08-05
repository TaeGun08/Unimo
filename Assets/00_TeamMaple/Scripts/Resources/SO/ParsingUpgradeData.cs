using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;

public abstract class ParsingUpgradeData<TRecord> : ScriptableObject where TRecord : class, IUpgradeData
{
    protected Dictionary<int, TRecord> dataDic = new();

    [SerializeField] private TextAsset levelCsv;

    protected void OnEnable()
    {
        LoadCsv();
    }

    protected void LoadCsv()
    {
        if (levelCsv == null) return;

        dataDic.Clear();

        var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null // 에러 방지용
        };

        using var reader = new StringReader(levelCsv.text);
        using var csv = new CsvReader(reader, config);

        csv.Read();
        csv.ReadHeader();

        var records = csv.GetRecords<TRecord>();
        foreach (var record in records)
        {
            if (!dataDic.ContainsKey(record.level))
            {
                dataDic.Add(record.level, record);
            }
        }
    }

    public TRecord GetData(int level)
    {
        dataDic.TryGetValue(level, out var data);

        return data;
    }
}