using UnityEngine;

public static class StageLoader
{
    public static bool HighStageChecker(int currentStageId)
    {
        return (JsonDataManager.Instance.LoadServerData().HighStage < currentStageId);
    }

    public static StageData SetStageData(ProceduralMapGeneratorSO so)
    {
        return so.GetStageData(JsonDataManager.Instance.LoadServerData().CurrentStage);
    }
}
