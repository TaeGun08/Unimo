using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Game : UI_Base
{
    public TextMeshProUGUI GameOneBest, GameTwoBest;
    private int stageCount;

    [Header("Stage Settings")]
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject[] starImg;
    [SerializeField] private GameObject bonusStageButton;
    [SerializeField] private GameObject selectStageUI;
    [SerializeField] private GameObject starRewardUI;
    [SerializeField] private TMP_Text getStarText;

    [Header("Equipped")] [SerializeField] private GetUnimoAndEngineImageSO so;
    [SerializeField] private Image[] images;

    [Header("SelectStage")] [SerializeField]
    private SelectStage selectStage;
    [SerializeField] private Image planetImage;
    
    private void OnEnable()
    {
        stageCount = Base_Mng.Data.data.HighStage;

        images[0].sprite = so.GetSprites.UnimoSprite[Base_Mng.Data.data.CharCount - 1];
        images[1].sprite = so.GetSprites.EngineSprite[Base_Mng.Data.data.EQCount - 1];

        stageCount = ((stageCount - 1) / 50) * 50 + 1;
        
        selectStage.Stage = stageCount;

        UpdatePlanetAndUI();

        GetStar();
    }

    public override void Start()
    {
        //Camera_Event.instance.GetCameraEvent(CameraMoveState.InGame);
        GameOneBest.text = "Best Score\n" + StringMethod.ToCurrencyString(Base_Mng.Data.data.BestScoreGameOne);
        GameTwoBest.text = "Best Score\n" + StringMethod.ToCurrencyString(Base_Mng.Data.data.BestScoreGameTwo);
        base.Start();

        stageCount = Base_Mng.Data.data.HighStage;

        stageCount = ((stageCount - 1) / 50) * 50 + 1;
        
        selectStage.Stage = stageCount;

        UpdatePlanetAndUI();

        GetStar();
    }

    public void GoGameScene()
    {
        int stageText = 0;

        int value = StageLoader.IsBonusStageByIndex(stageCount) ? 2 : 1;

        WholeSceneController.Instance.ReadyNextScene(value);
        Base_Mng.Data.data.GamePlay++;
        Pinous_Flower_Holder.FlowerHolder.Clear();
        if (!string.IsNullOrEmpty(inputField.text))
        {
            stageText = int.Parse(inputField.text);
            Base_Mng.Data.data.CurrentStage = stageText;
            Base_Mng.Data.data.HighStage = stageText;
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

    private void GetStar()
    {
        getStarText.text = $"0 / 135";

        if (StageManager.Instance == null) return;

        int starCount = 0;
        for (int i = 0; i < 50; i++)
        {
            if (StageManager.Instance.GetStars(i + 1) != 0) return;
            starCount += StageManager.Instance.GetStars(i + 1);
        }

        getStarText.text = $"{starCount} / 135";
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
            stageCount = Base_Mng.Data.data.HighStage;
            return;
        }

        if (StageLoader.IsBonusStageByIndex(stageCount) && !Base_Mng.Data.data.BonusStageOn)
        {
            stageCount++;
        }

        PlanetData planetData = StageManager.Instance.StageData.GetPlanetData(stageCount);
        planetImage.sprite = planetData.PlanetSprite;
        
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

        if (StageLoader.IsBonusStageByIndex(stageCount) && !Base_Mng.Data.data.BonusStageOn)
        {
            stageCount--;
        }

        PlanetData planetData = StageManager.Instance.StageData.GetPlanetData(stageCount);
        planetImage.sprite = planetData.PlanetSprite;
        
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

    public void SelectPlanetUp()
    {
        stageCount = ((stageCount - 1) / 50 + 1) * 50 + 1;
        if (stageCount > Base_Mng.Data.data.HighStage)
        {
            stageCount = Base_Mng.Data.data.HighStage;
        }

        UpdatePlanetAndUI();
    }

    public void SelectPlanetDown()
    {
        stageCount = ((stageCount - 1) / 50) * 50 + 1;
        stageCount -= 50;
        if (stageCount < 1)
        {
            stageCount = 1;
        }

        UpdatePlanetAndUI();
    }

    private void UpdatePlanetAndUI()
    {
        PlanetData planetData = StageManager.Instance.StageData.GetPlanetData(stageCount);
        planetImage.sprite = planetData.PlanetSprite;
        selectStage.Stage = stageCount;

        BonusStageOn();
        SetStageText();
        UpdateStar();
    }

    public void ActiveTrueStage()
    {
        selectStageUI.SetActive(true);
    }

    public void ActiveFalseStage()
    {
        selectStageUI.SetActive(false);
    }

    public void ActiveTrueReward()
    {
        starRewardUI.SetActive(true);
    }
}