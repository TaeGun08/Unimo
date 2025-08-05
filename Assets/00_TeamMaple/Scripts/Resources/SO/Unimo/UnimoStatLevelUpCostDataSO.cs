using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using UnityEngine;


[System.Serializable]
public class UnimoStatLevelUpCost: IUpgradeData
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

[CreateAssetMenu(fileName = "UnimoStatLevelUpCostDataSO", menuName = "Scriptable Object/UnimoStatLevelUpCostDataSO")]
public class UnimoStatLevelUpCostDataSO : ParsingUpgradeData<UnimoStatLevelUpCost> { }
