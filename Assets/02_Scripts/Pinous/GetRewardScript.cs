
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GetRewardScript : MonoBehaviour
{
    private StageManager stageManager;

    public TextMeshProUGUI[] texts;
    public TextMeshProUGUI YellowText, RedText, BlueText;
    public TextMeshProUGUI ScoreText;

    public GameObject ADSButton;

    [SerializeField] private Slider starBar;
    [SerializeField] private Transform[] starsTrs;

    private double redTrade;
    private double yellowTrade;
    private double blueTrade;

    private int currentStageId;

    private string starY1;
    private string starR2;
    private string starY3;
    private string starR3;

    [SerializeField] private TMP_Text[] flowerTexts;
    
    [SerializeField] private StageFlowerConditionData stageFlowerConditions;

    private void Start()
    {
        stageManager = StageManager.Instance;

        texts[2].text = ScoreManager.Instance.BlueTxt.text;

        flowerTexts[0].text = texts[0].text;
        flowerTexts[1].text = texts[1].text;
        flowerTexts[2].text = texts[2].text;
        
        if (Base_Mng.Data.data.BonusStageOn && StageLoader.IsBonusStageByIndex(Base_Mng.Data.data.CurrentStage))
        {
            currentStageId = Base_Mng.Data.data.CurrentStage + 999;
            starY1 = stageManager.StageRewardData.GetData(currentStageId).Star1Y;
            starR2 = stageManager.StageRewardData.GetData(currentStageId).Star2R;
            starY3 = stageManager.StageRewardData.GetData(currentStageId).Star3Y;
            starR3 = stageManager.StageRewardData.GetData(currentStageId).Star3R;

            GetBonusReward();
            Debug.Log("보너스 리워드");
        }
        else
        {
            currentStageId = Base_Mng.Data.data.CurrentStage + 1000;
            starY1 = stageManager.StageRewardData.GetData(currentStageId).Star1Y;
            starR2 = stageManager.StageRewardData.GetData(currentStageId).Star2R;
            starY3 = stageManager.StageRewardData.GetData(currentStageId).Star3Y;
            starR3 = stageManager.StageRewardData.GetData(currentStageId).Star3R;

            GetReward();
            Debug.Log("리워드");
        }
    }

    private void GetReward()
    {
        float condition = float.Parse(stageFlowerConditions.GetData(Base_Mng.Data.data.CurrentStage + 1000).Star3Condition);

        redTrade = StringMethod.ToCurrencyDouble(texts[0].text) * 
                   (StringMethod.ToCurrencyDouble(starR2) / (condition * 0.67d));
        
        yellowTrade = StringMethod.ToCurrencyDouble(texts[1].text) *
                      (StringMethod.ToCurrencyDouble(starY1) / (condition * 0.67d));

        blueTrade = StringMethod.ToCurrencyDouble(texts[2].text);

        ADSButton.SetActive(true);

        YellowText.text = StringMethod.ToCurrencyString(yellowTrade);
        RedText.text = StringMethod.ToCurrencyString(redTrade);
        BlueText.text = StringMethod.ToCurrencyString(blueTrade);

        if (!(starBar.value < 0.6f))
        {
            int getStar = stageManager.GetStars(Base_Mng.Data.data.CurrentStage + 1000);

            switch (starBar.value)
            {
                case >= 1f when getStar != 3:
                    stageManager.UpdateStageStars(Base_Mng.Data.data.CurrentStage + 1000, 3);

                    _= ActiveTrueStars(3);
                    if (getStar != 1 && getStar < 2)
                    {
                        yellowTrade += StringMethod.ToCurrencyDouble(starY1);
                    }

                    if (getStar != 2 && getStar < 3)
                    {
                        redTrade += StringMethod.ToCurrencyDouble(starR2);
                    }

                    yellowTrade += StringMethod.ToCurrencyDouble(starY3);
                    redTrade += StringMethod.ToCurrencyDouble(starR3);

                    break;
                case < 1f and >= 0.8f when getStar != 2:
                    stageManager.UpdateStageStars(Base_Mng.Data.data.CurrentStage + 1000, 2);
                    _= ActiveTrueStars(2);
                    if (getStar != 1 && getStar < 2)
                    {
                        yellowTrade += StringMethod.ToCurrencyDouble(starY1);
                    }

                    redTrade += StringMethod.ToCurrencyDouble(starR2);
                    
                    break;
                case < 0.8f and >= 0.6f when getStar != 1:
                    _= ActiveTrueStars(1);
                    stageManager.UpdateStageStars(Base_Mng.Data.data.CurrentStage + 1000, 1);
                    
                    yellowTrade += StringMethod.ToCurrencyDouble(starY1);
                    
                    break;
            }
            
            if (Base_Mng.Data.data.HighStage == Base_Mng.Data.data.CurrentStage)
            {
                Base_Mng.Data.data.HighStage++;
            }

            if (StageLoader.IsBonusStageByIndex(Base_Mng.Data.data.HighStage))
            {
                Base_Mng.Data.data.BonusStageOn = true;
            }
        }

        Base_Mng.Data.data.Red += redTrade;
        Base_Mng.Data.data.Yellow += yellowTrade;
        Base_Mng.Data.data.Blue += blueTrade;
        Base_Mng.Data.data.GetTicket--;
        Debug.Log("티켓 감소" + Base_Mng.Data.data.GetTicket);
    }

    private async Task ActiveTrueStars(int starCount)
    {
        int count = 0;
        foreach (var starTrs in starsTrs)
        {
            if (starCount == count) break;
            starTrs.gameObject.SetActive(true);
            await starTrs.DOScale(Vector3.one, 1f).AsyncWaitForCompletion();
            count++;
        }
    }

    private void GetBonusReward()
    {
        float condition = float.Parse(stageFlowerConditions.GetData(Base_Mng.Data.data.CurrentStage + 999).Star3Condition);

        redTrade = StringMethod.ToCurrencyDouble(texts[0].text) * 
                   (StringMethod.ToCurrencyDouble(starR2) / (condition * 0.67d)) * 3f;

        yellowTrade = StringMethod.ToCurrencyDouble(texts[1].text) *
                      (StringMethod.ToCurrencyDouble(starY1) / (condition * 0.67d)) * 3f;

        blueTrade = StringMethod.ToCurrencyDouble(texts[2].text) * 3f;

        ADSButton.SetActive(true);

        YellowText.text = StringMethod.ToCurrencyString(yellowTrade);
        RedText.text = StringMethod.ToCurrencyString(redTrade);
        BlueText.text = StringMethod.ToCurrencyString(blueTrade);

        Base_Mng.Data.data.Red += redTrade;
        Base_Mng.Data.data.Yellow += yellowTrade;
        Base_Mng.Data.data.Blue += blueTrade;

        Base_Mng.Data.data.HighStage++;
        Base_Mng.Data.data.CurrentStage = Base_Mng.Data.data.HighStage;

        Base_Mng.Data.data.GetTicket--;
        
        Base_Mng.Data.data.BonusStageOn = false;
    }

    private void ADSReward(float multiplication)
    {
        Base_Mng.Data.data.Red += redTrade * multiplication;
        Base_Mng.Data.data.RePlay++;
        var yellow = yellowTrade * multiplication;
        Base_Mng.Data.data.Yellow += yellow;
        Base_Mng.Data.data.Blue += blueTrade;

        RedText.text = StringMethod.ToCurrencyString(redTrade);
        YellowText.text = StringMethod.ToCurrencyString(yellow * 2);
        BlueText.text = StringMethod.ToCurrencyString(blueTrade);

        ADSButton.SetActive(false);
    }

    public void GetRewardDoubleScore()
    {
        Base_Mng.ADS.ShowRewardedAds(() =>
        {
            if (StageLoader.IsBonusStageByIndex(Base_Mng.Data.data.HighStage))
            {
                ADSReward(3f);
            }
            else
            {
                ADSReward(1f);
            }
        });
    }
}