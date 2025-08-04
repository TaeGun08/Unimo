using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class ScoreManager : SingletonBehaviour<ScoreManager>
{
    [SerializeField] private List<TextMeshProUGUI> specialResourceTxts;
    [SerializeField] private TextMeshProUGUI scoreTxt;
    [SerializeField] private TextMeshProUGUI resultTxt;
    [SerializeField] private TextMeshProUGUI blueTxt;
    public TextMeshProUGUI BlueTxt => blueTxt;
    
    private List<double> gatheredResources;
    private double score;
    private PlayHoneyGainCalculator honeyCalculator;
    private GameRecordManager recordManager;

    [SerializeField] private Slider starBar;
    [SerializeField] private StageFlowerConditionData stageFlowerConditions;

    private void Awake()
    {
        PlaySystemRefStorage.scoreManager = this;
        gatheredResources = new List<double> { 0, 0, 0 };
        score = 0;
    }

    private void Start()
    {
        recordManager = FindAnyObjectByType<GameRecordManager>();

        Debug.Log($"gatheredResources.Count: {gatheredResources.Count}");
        Debug.Log($"specialResourceTxts.Count: {specialResourceTxts.Count}");

        int loopCount = Mathf.Min(gatheredResources.Count - 1, specialResourceTxts.Count);
        for (int i = 1; i <= loopCount; i++)
        {
            specialResourceTxts[i - 1].text = gatheredResources[i].ToString();
        }

        PlaySystemRefStorage.playProcessController.SubscribeGameoverAction(() =>
        {
            for (int i = 1; i <= loopCount; i++)
            {
                specialResourceTxts[i - 1].text = gatheredResources[i].ToString();
            }

            scoreTxt.text = StringMethod.ToCurrencyString(gatheredResources[0]);
            resultTxt.text = StringMethod.ToCurrencyString(gatheredResources[0]);
            checkBest();
        });
    }

    public void AddBloomScore(int idx, float score)
    {
        gatheredResources[idx] += 1;
        this.score += 1;
        int id = 1000;
        if (StageLoader.IsBonusStageByIndex(Base_Mng.Data.data.CurrentStage))
        {
            id--;
        }
        
        float condition = float.Parse(stageFlowerConditions.GetData(Base_Mng.Data.data.CurrentStage + id).Star3Condition);
        if (starBar != null && starBar.value < 1)
        {
            starBar.value = (float)(this.score / condition);
        }
        
        if (idx != 0 && idx - 1 < specialResourceTxts.Count)
        {
            specialResourceTxts[idx - 1].text = gatheredResources[idx].ToString();
        }
        
        if (idx == 2 && blueTxt != null)
        {
            blueTxt.text = gatheredResources[idx].ToString();
        }

        scoreTxt.text = StringMethod.ToCurrencyString(gatheredResources[0]);
    }

    // int CalculateReward(int level)
    // {
    //     float normalizedLevel = Mathf.Log(level);
    //     if (level >= 1000) normalizedLevel = Mathf.Log(1000);
    //     float maxLogLevel = Mathf.Log(1000);
    //
    //     int reward = Mathf.FloorToInt(Mathf.Lerp(1, 50, normalizedLevel / maxLogLevel));
    //     Debug.Log($"Reward : {reward}");
    //     return reward;
    // }

    private void checkBest()
    {
        double best = recordManager.GetBestRecord(getGameIndex());
        
        switch (getGameIndex())
        {
            case 0:
                if (Base_Mng.Data.data.BestScoreGameOne <= score)
                {
                    Base_Mng.Data.data.BestScoreGameOne = best;
                }
        
                break;
            case 1:
                if (Base_Mng.Data.data.BestScoreGameTwo <= score)
                {
                    Base_Mng.Data.data.BestScoreGameTwo = best;
                }
                break;
            case 2:
                break;
        }
        
        if (score > best)
        {
            recordManager.SetBestRecord(score, getGameIndex());
        }
    }

    private int getGameIndex()
    {
        string scnName = SceneManager.GetActiveScene().name;
        string num = scnName.Substring(scnName.Length - 3);
        int.TryParse(num, out int idx);
        idx--;
        return idx;
    }

    public void ScoreUpButton()
    {
        score += 50;
        float condition = float.Parse(stageFlowerConditions.GetData(Base_Mng.Data.data.CurrentStage + 1000).Star3Condition);
        if (starBar != null && starBar.value < 1)
        {
            starBar.value = (float)(score / condition);
        }
    }
}