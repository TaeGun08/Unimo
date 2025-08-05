using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using UnityEngine;

[System.Serializable]
public class EquipmentStatLevelUpCost : IUpgradeData
{
    public int level { get; set; }
    public float need_yel { get; set; }
    public float need_yel_expo { get; set; }
    public float need_yel_sum { get; set; }
    public float need_yel_sum_expo { get; set; }
    public float need_org { get; set; }
    public float need_org_sum { get; set; }
    public float need_grn { get; set; }
    public float need_grn_sum { get; set; }
}

[CreateAssetMenu(fileName = "EquipmentStatLevelUpCostDataSO", menuName = "Scriptable Object/EquipmentStatLevelUpCostDataSO")]
public class EquipmentStatLevelUpCostDataSO : ScriptableObject
{
    [Header("CSV 파일 (N, R, SR 순서)")]
    [SerializeField] private TextAsset nCsv;
    [SerializeField] private TextAsset rCsv;
    [SerializeField] private TextAsset srCsv;

    private Dictionary<EquipmentRank, Dictionary<int, EquipmentStatLevelUpCost>> dataDic;

    private void OnEnable()
    {
        LoadAllCsv();
    }

    private void LoadAllCsv()
    {
        dataDic = new Dictionary<EquipmentRank, Dictionary<int, EquipmentStatLevelUpCost>>
        {
            { EquipmentRank.N, LoadCsv(nCsv) },
            { EquipmentRank.R, LoadCsv(rCsv) },
            { EquipmentRank.SR, LoadCsv(srCsv) },
        };
    }

    private Dictionary<int, EquipmentStatLevelUpCost> LoadCsv(TextAsset csvFile)
    {
        var dic = new Dictionary<int, EquipmentStatLevelUpCost>();

        if (csvFile == null) return dic;

        using (var reader = new StringReader(csvFile.text))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords<EquipmentStatLevelUpCost>().ToList();
            foreach (var record in records)
            {
                dic[record.level] = record;
            }
        }

        return dic;
    }

    public EquipmentStatLevelUpCost GetData(EquipmentRank grade, int level)
    {
        if (dataDic.TryGetValue(grade, out var gradeDic) && gradeDic.TryGetValue(level, out var data))
            return data;

        Debug.LogWarning($"{grade} 등급의 {level} 레벨 데이터 없음");
        return null;
    }
}


