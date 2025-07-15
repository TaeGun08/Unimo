#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class GimmickCSVGenerator
{
    [MenuItem("Tools/StageGimmick/Generate Sample CSV")]
    public static void GenerateSampleCSV()
    {
        string path = "Assets/DataTables/gimmicks.csv";

        List<string> lines = new List<string>
        {
            "stageStart,stageEnd,gimmickType,isBonus"
        };

        // 샘플 기믹 범위 설정
        lines.Add("1,50,None,false");
        lines.Add("51,100,LightningStrike,false");
        lines.Add("101,150,PoisonGas,false");
        lines.Add("151,200,SlipperyFloor,false");
        lines.Add("201,250,MeteorFall,false");
        lines.Add("251,300,Darkness,false");
        lines.Add("301,350,WildWind,false");
        lines.Add("351,400,FogDamage,false");
        lines.Add("401,450,TimeSlow,false");
        lines.Add("451,500,Earthquake,false");

        File.WriteAllLines(path, lines);
        AssetDatabase.Refresh();

        Debug.Log($"[CSV 생성] {path} 에 StageGimmick CSV가 생성되었습니다.");
    }
}
#endif