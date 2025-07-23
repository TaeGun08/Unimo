using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json; // Ãß°¡
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class StageClearData
{
    public int stageId;
    public int stars; // È¹µæÇÑ º° °³¼ö (0~3)
}

[System.Serializable]
public class StageSaveData
{
    public List<StageClearData> stages = new();
}

public class StageManager : SingletonBehaviour<StageManager>
{
    public static StageManager Instance { get; private set; }
    
    private string SavePath => Path.Combine(Application.persistentDataPath, "stage_save.json");
    public StageSaveData CurrentSaveData { get; private set; }
    
    [Header("StageSO")] 
    [SerializeField] private StageData stageData;
    [SerializeField] private StageRewardData stageRewardData;
    [SerializeField] private StageGenerateFlowerData stageGenerateFlowerData;
    
    public StageData StageData => stageData;
    public StageRewardData StageRewardData => stageRewardData;
    public StageGenerateFlowerData StageGenerateFlowerData => stageGenerateFlowerData;

    private void Awake()
    {
        Instance = this;

        

        Load();
    }

    public void Load()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            CurrentSaveData = JsonConvert.DeserializeObject<StageSaveData>(json);
        }
        else
        {
            CurrentSaveData = new StageSaveData();
        }
    }

    public void Save()
    {
        string json = JsonConvert.SerializeObject(CurrentSaveData, Formatting.Indented);
        File.WriteAllText(SavePath, json);
    }

    public void UpdateStageStars(int stageId, int newStars)
    {
        var stageData = CurrentSaveData.stages.FirstOrDefault(s => s.stageId == stageId);
        if (stageData != null)
        {
            if (newStars > stageData.stars)
                stageData.stars = newStars;
        }
        else
        {
            CurrentSaveData.stages.Add(new StageClearData { stageId = stageId, stars = newStars });
        }
        Save();
    }

    public int GetStars(int stageId)
    {
        return CurrentSaveData.stages.FirstOrDefault(s => s.stageId == stageId)?.stars ?? 0;
    }
}
