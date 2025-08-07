using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class StageRewardProgress
{
    public int StageGroupId;
    public List<int> ReceivedThresholds = new();
}

[Serializable]
public class StarRewardSaveData
{
    public List<StageRewardProgress> stageRewards = new();
}

public class StarRewardUI : MonoBehaviour
{
    private string SavePath => Path.Combine(Application.persistentDataPath, "StarRewardSave.json");
    public StarRewardSaveData CurrentSaveData { get; private set; }

    [SerializeField] private Slider stageSlider;
    [SerializeField] private UI_Game uiGame;
    [SerializeField] private List<int> rewardThresholds = new() { 30, 60, 90, 115, 135 };

    private void OnEnable()
    {
        Load();
        UpdateSlider();
        ClaimAllAvailableRewards();
    }

    private void UpdateSlider()
    {
        if (StageManager.Instance == null)
        {
            stageSlider.value = 0f;
            return;
        }

        int starCount = GetCurrentGroupStarCount();
        stageSlider.value = Mathf.Clamp01(starCount / 135f);
    }

    private int GetCurrentGroupStarCount()
    {
        int startStage = ((uiGame.StageCount - 1) / 50) * 50 + 1;
        int starCount = 0;

        for (int i = 0; i < 50; i++)
        {
            int currentStage = startStage + i;
            starCount += StageManager.Instance.GetStars(currentStage + 1000);
        }

        return starCount;
    }

    private void ClaimAllAvailableRewards()
    {
        int groupId = ((uiGame.StageCount - 1) / 50) * 50 + 49;
        int starCount = GetCurrentGroupStarCount();
        var groupReward = GetOrCreateStageReward(groupId);

        bool anyClaimed = false;

        foreach (int threshold in rewardThresholds)
        {
            if (starCount >= threshold && !groupReward.ReceivedThresholds.Contains(threshold))
            {
                groupReward.ReceivedThresholds.Add(threshold);
                GiveReward(threshold);
                anyClaimed = true;
            }
        }

        if (anyClaimed)
        {
            Save();
        }
    }

    private StageRewardProgress GetOrCreateStageReward(int groupId)
    {
        var groupReward = CurrentSaveData.stageRewards.FirstOrDefault(r => r.StageGroupId == groupId);
        if (groupReward == null)
        {
            groupReward = new StageRewardProgress { StageGroupId = groupId };
            CurrentSaveData.stageRewards.Add(groupReward);
        }
        return groupReward;
    }

    private void GiveReward(int threshold)
    {
        int rewardValue = threshold switch
        {
            30 => 1,
            60 => 3,
            90 => 5,
            115 => 7,
            135 => 10,
            _ => 0
        };

        Base_Mng.Data.data.Blue += rewardValue;
    }

    public void Load()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            CurrentSaveData = JsonConvert.DeserializeObject<StarRewardSaveData>(json);
        }
        else
        {
            CurrentSaveData = new StarRewardSaveData();
        }
    }

    public void Save()
    {
        string json = JsonConvert.SerializeObject(CurrentSaveData, Formatting.Indented);
        File.WriteAllText(SavePath, json);
    }
}
