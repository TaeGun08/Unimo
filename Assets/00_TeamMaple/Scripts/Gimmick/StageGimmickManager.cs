using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using UnityEngine.Purchasing.MiniJSON;

public class StageGimmickManager : MonoBehaviour
{
    [SerializeField] private TextAsset csvFile;
    private Dictionary<int, List<StageGimmickType>> stageToGimmickMap = new();
    private GameObject currentGimmickInstance;

    private void Awake()
    {
        LoadGimmickDataFromCSV();
        JsonDataLoader.LoadServerData();
    }

    public void TryApplyGimmick(int stageNum)
    {
        ClearCurrentGimmick();

        if (!stageToGimmickMap.TryGetValue(stageNum, out var gimmickTypes) || gimmickTypes.Count == 0)
        {
            Debug.Log($"[기믹] 스테이지 {stageNum}: 기믹 없음");
            return;
        }

        foreach (var gimmickType in gimmickTypes)
        {
            var gimmickLogic = GimmickRegistry.GetLogic(gimmickType);
            if (gimmickLogic != null)
            {
                // 💡 비주얼 적용
                var visualApplier = FindObjectOfType<StageGimmickVisualApplier>();
                if (visualApplier != null)
                    visualApplier.ApplyVisuals(gimmickLogic);

                // 💥 기믹 실행
                GameObject gimmickObj = gimmickLogic.Execute(transform.position);
                if (gimmickObj != null) currentGimmickInstance = gimmickObj;

                Debug.Log($"[기믹] 스테이지 {stageNum}: {gimmickType} 실행");
            }
        }
    }

    private void LoadGimmickDataFromCSV()
    {
        stageToGimmickMap.Clear();

        using (StringReader reader = new StringReader(csvFile.text))
        {
            string line;
            bool firstLine = true;

            while ((line = reader.ReadLine()) != null)
            {
                if (firstLine) { firstLine = false; continue; }

                string[] tokens = line.Split(',');
                if (tokens.Length < 3) continue;

                int start = int.Parse(tokens[0]);
                int end = int.Parse(tokens[1]);
                StageGimmickType type = Enum.TryParse(tokens[2], out StageGimmickType parsed) ? parsed : StageGimmickType.None;

                for (int i = start; i <= end; i++)
                {
                    if (!stageToGimmickMap.ContainsKey(i))
                        stageToGimmickMap[i] = new List<StageGimmickType>();

                    if (type != StageGimmickType.None)
                        stageToGimmickMap[i].Add(type);
                }
            }
        }

        Debug.Log($"[기믹 매핑] {stageToGimmickMap.Count}개의 스테이지 기믹 로드 완료");
    }

    private void ClearCurrentGimmick()
    {
        if (currentGimmickInstance != null)
        {
            Destroy(currentGimmickInstance);
            currentGimmickInstance = null;
        }
    }
}

public static class GimmickRegistry
{
    private static Dictionary<StageGimmickType, StageGimmickSO> registry = new();

    public static void Register(StageGimmickType type, StageGimmickSO logic)
    {
        registry[type] = logic;
    }
    
    public static StageGimmickSO GetLogic(StageGimmickType type)
    {
        return registry.TryGetValue(type, out var logic) ? logic : null;
    }
}

public enum StageGimmickType
{
    None,
    LightningStrike,
    PoisonGas,
    SlipperyFloor,
    MeteorFall,
    Darkness,
    WildWind,
    FogDamage,
    Earthquake,
    TimeSlow,
    BlackHole
}
