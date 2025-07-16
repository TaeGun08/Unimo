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
        string path = "Assets/00_TeamMaple/Resources/CSV/gimmicks.csv";

        List<string> lines = new List<string>
        {
            "stageStart,stageEnd,gimmickType"
        };

        // 샘플 기믹 범위 설정
        lines.Add("1,50,None");
        lines.Add("501,550,None");
        lines.Add("51,100,LightningStrike");
        lines.Add("551,600,LightningStrike");
        lines.Add("101,150,PoisonGas");
        lines.Add("601,650,PoisonGas");
        lines.Add("151,200,SlipperyFloor");
        lines.Add("651,700,SlipperyFloor");
        lines.Add("201,250,MeteorFall");
        lines.Add("701,750,MeteorFall");
        lines.Add("251,300,Darkness");
        lines.Add("751,800,Darkness");
        lines.Add("301,350,WildWind");
        lines.Add("801,850,WildWind");
        lines.Add("351,400,FogDamage");
        lines.Add("851,900,FogDamage");
        lines.Add("401,450,Earthquake");
        lines.Add("901,950,Earthquake");
        lines.Add("451,500,TimeSlow");
        lines.Add("951,1000,TimeSlow");
        lines.Add("501,1000,BlackHole");

        File.WriteAllLines(path, lines);
        AssetDatabase.Refresh();

        Debug.Log($"[CSV 생성] {path} 에 StageGimmick CSV가 생성되었습니다.");
    }
}
#endif