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
    [SerializeField] private GameObject bonusStageButton;

    private void OnEnable()
    {
        stageCount = JsonDataLoader.LoadServerData().CurrentStage;
        UpdateStar();
        SetStageText();
        BonusStageOn();
    }

    public override void Start()
    {
        Camera_Event.instance.GetCameraEvent(CameraMoveState.InGame);
        GameOneBest.text = "Best Score\n" + StringMethod.ToCurrencyString(Base_Mng.Data.data.BestScoreGameOne);
        GameTwoBest.text = "Best Score\n" + StringMethod.ToCurrencyString(Base_Mng.Data.data.BestScoreGameTwo);
        base.Start();

        stageCount = JsonDataLoader.LoadServerData().CurrentStage;
        UpdateStar();
        SetStageText();
        BonusStageOn();
    }

    public override void DisableOBJ()
    {
        base.DisableOBJ();
    }

    public void GoGameScene(int value)
    {
        int stageText = 0;

        WholeSceneController.Instance.ReadyNextScene(value);
        Base_Mng.Data.data.GamePlay++;
        Pinous_Flower_Holder.FlowerHolder.Clear();
        if (!string.IsNullOrEmpty(inputField.text))
        {
            stageText = int.Parse(inputField.text);
            Base_Mng.Data.data.CurrentStage = stageText;
        }
        else
        {
            Base_Mng.Data.data.CurrentStage = stageCount;
        }

        //Base_Mng.Data.data.CurrentStage = stageCount;
        JsonDataLoader.SaveServerData(Base_Mng.Data.data);

        //Base_Mng.ADS._interstitialCallback = () =>
        //{

        //};
        //Base_Mng.ADS.ShowInterstitialAds();
    }

    private void SetStageText()
    {
        if (StageLoader.IsBonusStageByIndex(stageCount) && Base_Mng.Data.data.BonusStageOn) return;
        stageText.text = $"Stage-{stageCount}";
    }

    public void StageCountUp()
    {
        stageCount++;
        if (StageLoader.HighStageChecker(stageCount))
        {
            stageCount = JsonDataLoader.LoadServerData().HighStage;
            return;
        }

        BonusStageOn();
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

        BonusStageOn();
        SetStageText();
        UpdateStar();
    }

    private void BonusStageOn()
    {
        if (StageLoader.IsBonusStageByIndex(stageCount) && Base_Mng.Data.data.BonusStageOn
            && Base_Mng.Data.data.HighStage <= stageCount)
        {
            stageText.text = $"Bonus Stage";
            bonusStageButton.SetActive(true);
        }
        else
        {
            bonusStageButton.SetActive(false);
        }
    }

    private void UpdateStar()
    {
        if (StageLoader.IsBonusStageByIndex(stageCount) && Base_Mng.Data.data.BonusStageOn
            && Base_Mng.Data.data.HighStage <= stageCount)
        {
            for (int i = 0; i < starImg.Length; i++)
            {
                starImg[i].SetActive(false);
            }
            return;
        }

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