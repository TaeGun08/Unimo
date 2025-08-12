using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Game : UI_Base
{
    public TextMeshProUGUI GameOneBest, GameTwoBest;
    private int stageCount;
    public int StageCount => stageCount;

    [Header("Stage Settings")]
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject[] starImg;
    [SerializeField] private GameObject bonusStageButton;
    [SerializeField] private GameObject selectStageUI;
    [SerializeField] private GameObject starRewardUI;
    [SerializeField] private TMP_Text getStarText;

    [Header("Equipped")]
    [SerializeField] private GetUnimoAndEngineImageSO so;
    [SerializeField] private Image[] images;

    [Header("SelectStage")]
    [SerializeField] private SelectStage selectStage;
    [SerializeField] private Image planetImage;

    [Header("Ticket")]
    [SerializeField] private TMP_Text ticketText;

    public override void Start()
    {
        base.Start();
        
        GameOneBest.text = "Best Score\n" + StringMethod.ToCurrencyString(Base_Mng.Data.data.BestScoreGameOne);
        GameTwoBest.text = "Best Score\n" + StringMethod.ToCurrencyString(Base_Mng.Data.data.BestScoreGameTwo);

        images[0].sprite = so.GetSprites.UnimoSprite[Base_Mng.Data.data.CharCount - 1];
        images[1].sprite = so.GetSprites.EngineSprite[Base_Mng.Data.data.EQCount - 1];
        
        stageCount = Base_Mng.Data.data.HighStage;

        if (stageCount > 999)
        {
            stageCount = 999;
        }
        
        InitializeStage();
    }

    private void InitializeStage()
    {
        int highStage = Base_Mng.Data.data.HighStage;
        
        stageCount = ((highStage - 1) / 50 + 1) * 50 - 1;

        if (stageCount > highStage)
            stageCount = highStage;

        selectStage.Stage = stageCount;

        UpdatePlanetAndUI();
        GetStar();
        ticketText.text = $"{Base_Mng.Data.data.GetTicket} / 10";
    }

    public void GoGameScene()
    {
        if (Base_Mng.Data.data.GetTicket <= 0) return;

        int stageTextInput = 0;
        int value = StageLoader.IsBonusStageByIndex(stageCount) ? 2 : 1;

        WholeSceneController.Instance.ReadyNextScene(value);
        Base_Mng.Data.data.GamePlay++;
        Pinous_Flower_Holder.FlowerHolder.Clear();

        if (!string.IsNullOrEmpty(inputField.text) && int.TryParse(inputField.text, out stageTextInput))
        {
            Base_Mng.Data.data.CurrentStage = stageTextInput;
            Base_Mng.Data.data.HighStage = stageTextInput;
        }
        else
        {
            Base_Mng.Data.data.CurrentStage = stageCount;
        }
        
        Canvas_Holder.CloseAllPopupUI();

        JsonDataLoader.SaveServerData(Base_Mng.Data.data);
    }

    private void GetStar()
    {
        if (StageManager.Instance == null)
        {
            getStarText.text = "0 / 135";
            return;
        }

        int starCount = 0;
        int startStage = ((stageCount - 1) / 50) * 50 + 1;

        for (int i = 0; i < 50; i++)
        {
            int currentStage = startStage + i;
            starCount += StageManager.Instance.GetStars(currentStage + 1000);
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
        
        if (StageLoader.IsBonusStageByIndex(stageCount))
        {
            stageCount++;
            
            if (StageLoader.HighStageChecker(stageCount))
            {
                stageCount = Base_Mng.Data.data.HighStage;
            }
        }

        UpdatePlanetAndUI();
    }

    public void StageCountDown()
    {
        stageCount--;
        
        if (stageCount < 1)
        {
            stageCount = 1;
            return;
        }
        
        if (StageLoader.IsBonusStageByIndex(stageCount))
        {
            stageCount--;
        }

        UpdatePlanetAndUI();
    }

    private void BonusStageOn()
    {
        bool isBonus = StageLoader.IsBonusStageByIndex(stageCount) && Base_Mng.Data.data.BonusStageOn && Base_Mng.Data.data.HighStage <= stageCount;
        stageText.text = isBonus ? "Bonus Stage" : $"Stage-{stageCount}";
        bonusStageButton.SetActive(isBonus);
    }

    private void UpdateStar()
    {
        int stars = StageManager.Instance.GetStars(stageCount + 1000);

        for (int i = 0; i < starImg.Length; i++)
        {
            starImg[i].SetActive(i < stars);
        }
    }

    public void SelectPlanetUp()
    {
        int planetIndex = ((stageCount - 1) / 50) + 1;
        int nextStageCount = ((planetIndex + 1) * 50) - 1;

        if (nextStageCount > Base_Mng.Data.data.HighStage)
            nextStageCount = Base_Mng.Data.data.HighStage;

        stageCount = nextStageCount;
        UpdatePlanetAndUI();
    }

    public void SelectPlanetDown()
    {
        int planetIndex = ((stageCount - 1) / 50) - 1;

        if (planetIndex < 0)
            planetIndex = 0;

        int prevStageCount = ((planetIndex + 1) * 50) - 1;

        if (prevStageCount > Base_Mng.Data.data.HighStage)
            prevStageCount = Base_Mng.Data.data.HighStage;

        stageCount = prevStageCount;
        UpdatePlanetAndUI();
    }

    private void UpdatePlanetAndUI()
    {
        BonusStageOn();
        
        for (int i = 0; i < starImg.Length; i++)
        {
            starImg[i].SetActive(false);
        }
        
        if (StageLoader.IsBonusStageByIndex(stageCount)) return;
        PlanetData planetData = StageManager.Instance.StageData.GetPlanetData(stageCount);
        planetImage.sprite = planetData.PlanetSprite;
        selectStage.Stage = stageCount;
        
        UpdateStar();
        SetStageText();
        GetStar();
    }

    public void ActiveTrueStage() => selectStageUI.SetActive(true);
    public void ActiveFalseStage() => selectStageUI.SetActive(false);
    public void ActiveTrueReward() => starRewardUI.SetActive(true);
    public void ActiveFalseReward() => starRewardUI.SetActive(false);
    public void ActiveTrueCharacter() => Canvas_Holder.instance.GetUI("##Character");
}