using System;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UI_Game : UI_Base
{
    public TextMeshProUGUI GameOneBest, GameTwoBest;
    private int stageCount;
    
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject[] starImg;

    private void OnEnable()
    {
        UpdateStar();
        stageCount = JsonDataLoader.LoadServerData().CurrentStage;
    }

    public override void Start()
    {
        Camera_Event.instance.GetCameraEvent(CameraMoveState.InGame);
        GameOneBest.text = "Best Score\n" + StringMethod.ToCurrencyString(Base_Mng.Data.data.BestScoreGameOne);
        GameTwoBest.text = "Best Score\n" + StringMethod.ToCurrencyString(Base_Mng.Data.data.BestScoreGameTwo);
        base.Start();

        stageCount = JsonDataLoader.LoadServerData().CurrentStage;
        UpdateStar();
    }

    public override void DisableOBJ()
    {
        base.DisableOBJ();
    }

    public void GoGameScene(int value)
    {
        int stageText = 0;
        if (!string.IsNullOrEmpty(inputField.text))
        {
            stageText = int.Parse(inputField.text);
        }
        
        WholeSceneController.Instance.ReadyNextScene(value);
        Base_Mng.Data.data.GamePlay++;
        Pinous_Flower_Holder.FlowerHolder.Clear();
        Base_Mng.Data.data.CurrentStage = stageCount + stageText;
        
        //Base_Mng.ADS._interstitialCallback = () =>
        //{
           
        //};
        //Base_Mng.ADS.ShowInterstitialAds();
    }

    private void SetStageText()
    {
        stageText.text = $"Stage-{stageCount}";
    }

    public void StageCountUp()
    {
        stageCount++;
        if (StageLoader.HighStageChecker(stageCount))
        {
            stageCount--;
            return;
        }
        
        SetStageText();
        UpdateStar();
    }

    public void StageCountDown()
    {
        stageCount--;
        if (0 >= stageCount)
        {
            stageCount = 1;
            return;
        }
        
        SetStageText();
        UpdateStar();
    }

    private void UpdateStar()
    {
        if (StageManager.Instance.GetStars(stageCount + 1000) == 3)
        {
            for (int i = 0; i < starImg.Length; i++)
            {
                starImg[i].SetActive(true);
            }
        }
        else if (StageManager.Instance.GetStars(stageCount + 1000) == 2)
        {
            for (int i = 0; i < starImg.Length - 1; i++)
            {
                starImg[i].SetActive(true);
            }
        }
        else if (StageManager.Instance.GetStars(stageCount + 1000) == 1)
        {
            starImg[0].SetActive(true);
        }
        else
        {
            for (int i = 0; i < starImg.Length; i++)
            {
                starImg[i].SetActive(false);
            }
        }
    }
}
