using System.IO;
using UnityEngine;

public static class StageLoader
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "stage_save.json");
    
    public static bool HighStageChecker(int currentStageId)
    {
        return (JsonDataLoader.LoadServerData().HighStage < currentStageId);
    }

    public static StageData SetStageData(ProceduralMapGeneratorSO so)
    {
        return so.GetStageData(JsonDataLoader.LoadServerData().CurrentStage + 1000);
    }

    public static void StageDataReset()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }
    }
    
    public static bool IsBonusStageByIndex(int index)
    {
        return (index + 1) % 10 == 0;
    }
}
