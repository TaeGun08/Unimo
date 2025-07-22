using System.IO;
using UnityEngine;

public static class StageLoader
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "stage_save.json");
    
    public static bool HighStageChecker(int currentStageId)
    {
        return (JsonDataLoader.LoadServerData().HighStage < currentStageId);
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
        return index % 10 == 0;
    }
}
