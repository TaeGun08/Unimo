using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> specialResourceTxts;
    [SerializeField] private TextMeshProUGUI scoreTxt;
    [SerializeField] private TextMeshProUGUI resultTxt;
    private List<double> gatheredResources;
    private double score;
    private PlayHoneyGainCalculator honeyCalculator;
    private GameRecordManager recordManager;

    private StageManager stageManager;
    
    [SerializeField] private Slider starBar;
    
    private void Awake()
    {
        PlaySystemRefStorage.scoreManager = this;
        gatheredResources = new List<double> { 0, 0 };
        score = 0;
    }
    private void Start()
    {
        recordManager = FindAnyObjectByType<GameRecordManager>();
        honeyCalculator = GetComponent<PlayHoneyGainCalculator>();
        stageManager = StageManager.Instance;
        for (int i = 1; i < gatheredResources.Count; i++)
        {
            specialResourceTxts[i-1].text = gatheredResources[i].ToString();
        }
        PlaySystemRefStorage.playProcessController.SubscribeGameoverAction(() => 
        {
            for (int i = 1; i < gatheredResources.Count; i++)
            {
                specialResourceTxts[i-1].text = gatheredResources[i].ToString();
            }

            scoreTxt.text = StringMethod.ToCurrencyString(gatheredResources[0]);
            resultTxt.text = StringMethod.ToCurrencyString(gatheredResources[0]);
            checkBest();
        });
        Debug.Log("??");
    }
    public void AddBloomScore(int idx, float score)
    {
        if (idx == 1)
        {
            gatheredResources[idx] += StringMethod.ToCurrencyDouble(stageManager.StageRewardData.Star2R) / 66d;
        }
        else
        {
            //gatheredResources[idx] += StringMethod.ToCurrencyDouble(stageManager.StageRewardData.Star1Y) / 133d;
            gatheredResources[idx] += 1000000;
        }
        
        this.score += 1000000;
        
        if (starBar.value < 1)
        {
            starBar.value = (float)(gatheredResources[0] / 200);
        }
        
        if (idx != 0) { specialResourceTxts[idx-1].text = gatheredResources[idx].ToString(); }
        scoreTxt.text = StringMethod.ToCurrencyString(gatheredResources[0]);
    }

    int CalculateReward(int level)
    {
        // �α� ������ ����
        float normalizedLevel = Mathf.Log(level);  // �α� �������� �ʹ� �޻��
        if (level >= 1000) normalizedLevel = Mathf.Log(1000);
        float maxLogLevel = Mathf.Log(1000);   // �ִ� ���� 1000�� �α� ��

        // ������ ���� ��� �� int�� ��ȯ
        int reward = Mathf.FloorToInt(Mathf.Lerp(1, 50, normalizedLevel / maxLogLevel));
        Debug.Log($"Reward : {reward}");
        return reward;
    }
    private void checkBest()
    {
        double best = recordManager.GetBestRecord(getGameIndex());
        switch(getGameIndex())
        {
            case 0:
                if(Base_Mng.Data.data.BestScoreGameOne <= score)
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
        return  idx;
    }
}
