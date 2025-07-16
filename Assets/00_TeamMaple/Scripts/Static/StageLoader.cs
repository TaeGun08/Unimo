using UnityEngine;

public static class StageLoader
{
    public static bool HighStageChecker(int currentStageId)
    {
        return (JsonDataLoader.LoadServerData().HighStage < currentStageId);
    }

    public static StageData SetStageData(ProceduralMapGeneratorSO so)
    {
        return so.GetStageData(JsonDataLoader.LoadServerData().CurrentStage + 1000);
    }
}
