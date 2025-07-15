using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

public class StageGimmickManager : MonoBehaviour
{
    [SerializeField] private TextAsset csvFile; // CSV 파일 드래그
    private Dictionary<int, StageGimmickData> stageToGimmickMap = new();
    private GameObject currentGimmickInstance;

    private void Awake()
    {
        LoadGimmickDataFromCSV();
    }

    public void TryApplyGimmick(int stageNum)
    {
        ClearCurrentGimmick();

        if (!stageToGimmickMap.TryGetValue(stageNum, out var data) || data.gimmickType == StageGimmickType.None)
        {
            Debug.Log($"[기믹] 스테이지 {stageNum}: 기믹 없음");
            return;
        }

        var gimmickLogic = GimmickRegistry.GetLogic(data.gimmickType);
        if (gimmickLogic != null)
        {
            GameObject gimmickObj = gimmickLogic.Execute(transform.position);
            if (gimmickObj != null) currentGimmickInstance = gimmickObj;
            Debug.Log($"[기믹] 스테이지 {stageNum}: {data.gimmickType} 실행");
        }

        if (data.isBonus)
        {
            Debug.Log($"[보너스] 스테이지 {stageNum}은 보너스 스테이지입니다.");
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
                if (firstLine) { firstLine = false; continue; } // Skip header

                string[] tokens = line.Split(',');
                if (tokens.Length < 4) continue;

                int start = int.Parse(tokens[0]);
                int end = int.Parse(tokens[1]);
                bool isBonus = bool.Parse(tokens[3]);
                StageGimmickType type = Enum.TryParse(tokens[2], out StageGimmickType parsed) ? parsed : StageGimmickType.None;

                for (int i = start; i <= end; i++)
                {
                    stageToGimmickMap[i] = new StageGimmickData(i, type, isBonus);
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

public class StageGimmickData
{
    public int stageNum;
    public StageGimmickType gimmickType;
    public bool isBonus;

    public StageGimmickData(int num, StageGimmickType type, bool bonus)
    {
        stageNum = num;
        gimmickType = type;
        isBonus = bonus;
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
    None,            // 넓은 별
    LightningStrike, // 낙뢰의 별
    PoisonGas,       // 초록가스 별
    SlipperyFloor,   // 얼어붙은 별
    MeteorFall,      // 분출하는 별
    Darkness,        // 검은 별
    WildWind,        // 질풍의 별
    FogDamage,       // 안개 구름 별
    TimeSlow,        // 시간의 별
    Earthquake       // 불안정한 별
}

