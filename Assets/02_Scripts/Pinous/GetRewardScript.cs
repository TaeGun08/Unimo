using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetRewardScript : MonoBehaviour
{
    private StageManager stageManager;

    public TextMeshProUGUI[] texts;
    public TextMeshProUGUI YellowText, RedText;
    public TextMeshProUGUI ScoreText;

    public GameObject ADSButton;

    [SerializeField] private Slider starBar;
    [SerializeField] private TMP_Text starText;

    private double redTrade;
    private double yellowTrade;

    private int currentStageId;

    private string starY1;
    private string starR2;
    private string starY3;
    private string starR3;

    private void Start()
    {
        stageManager = StageManager.Instance;
        
        if (Base_Mng.Data.data.BonusStageOn)
        {
            currentStageId = Base_Mng.Data.data.CurrentStage + 999;
            starY1 = stageManager.StageRewardData.GetData(currentStageId).Star1Y;
            starR2 =  stageManager.StageRewardData.GetData(currentStageId).Star2R;
            starY3 = stageManager.StageRewardData.GetData(currentStageId).Star3Y;
            starR3 = stageManager.StageRewardData.GetData(currentStageId).Star3R;
            
            GetBonusReward();
            Debug.Log("���ʽ� ������");
        }
        else
        {
            currentStageId = Base_Mng.Data.data.CurrentStage + 1000;
            starY1 = stageManager.StageRewardData.GetData(currentStageId).Star1Y;
            starR2 =  stageManager.StageRewardData.GetData(currentStageId).Star2R;
            starY3 = stageManager.StageRewardData.GetData(currentStageId).Star3Y;
            starR3 = stageManager.StageRewardData.GetData(currentStageId).Star3R;
            
            GetReward();
            Debug.Log("������");
        }
    }

    private void GetReward()
    {
        redTrade = double.Parse(texts[0].text) *
                   (StringMethod.ToCurrencyDouble(starR2) / 66d);

        yellowTrade = StringMethod.ToCurrencyDouble(texts[1].text) *
                      (StringMethod.ToCurrencyDouble(starY1) / 133d);

        ADSButton.SetActive(true);

        YellowText.text = StringMethod.ToCurrencyString(yellowTrade);
        RedText.text = StringMethod.ToCurrencyString(redTrade);

        if (!(starBar.value < 0.01f) &&
            Base_Mng.Data.data.HighStage <= Base_Mng.Data.data.CurrentStage)
        {
            int getStar = stageManager.GetStars(Base_Mng.Data.data.CurrentStage + 1000);

            starText.text = "x0";

            switch (starBar.value)
            {
                case >= 1f when getStar != 3:
                    stageManager.UpdateStageStars(Base_Mng.Data.data.CurrentStage + 1000, 3);
                    starText.text = "x3";

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
                case < 1f and >= 0.75f when getStar != 2:
                    stageManager.UpdateStageStars(Base_Mng.Data.data.CurrentStage + 1000, 2);
                    starText.text = "x2";

                    if (getStar != 1 && getStar < 2)
                    {
                        yellowTrade += StringMethod.ToCurrencyDouble(starY1);
                    }

                    redTrade += StringMethod.ToCurrencyDouble(starR2);
                    break;
                case < 0.75f and >= 0.5f when getStar != 1:
                    stageManager.UpdateStageStars(Base_Mng.Data.data.CurrentStage + 1000, 1);
                    starText.text = "x1";
                    yellowTrade += StringMethod.ToCurrencyDouble(starY1);
                    break;
            }
            
            stageManager.UpdateStageStars(Base_Mng.Data.data.CurrentStage + 1000, 3);
            starText.text = "x3";

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
            
            Base_Mng.Data.data.HighStage++;
            
            if (StageLoader.IsBonusStageByIndex(Base_Mng.Data.data.HighStage))
            {
                Base_Mng.Data.data.BonusStageOn = true;
            }
        }

        Base_Mng.Data.data.Red += redTrade;
        Base_Mng.Data.data.Yellow += yellowTrade;
    }

    private void GetBonusReward()
    {
        redTrade = (double.Parse(texts[0].text) *
                    (StringMethod.ToCurrencyDouble(starR2) / 66d)) * 3f;

        yellowTrade = (StringMethod.ToCurrencyDouble(texts[1].text) *
                       (StringMethod.ToCurrencyDouble(starY1) / 133d)) * 3f;

        ADSButton.SetActive(true);

        YellowText.text = StringMethod.ToCurrencyString(yellowTrade);
        RedText.text = StringMethod.ToCurrencyString(redTrade);

        Base_Mng.Data.data.Red += redTrade;
        Base_Mng.Data.data.Yellow += yellowTrade;

        Base_Mng.Data.data.HighStage++;
        Base_Mng.Data.data.CurrentStage = Base_Mng.Data.data.HighStage;
        
        Base_Mng.Data.data.BonusStageOn = false;
    }

    private void ADSReward(float multiplication)
    {
        Base_Mng.Data.data.Red += redTrade * multiplication;
        Base_Mng.Data.data.RePlay++;
        var yellow = yellowTrade * multiplication;
        Base_Mng.Data.data.Yellow += yellow;

        RedText.text = redTrade.ToString();
        YellowText.text = StringMethod.ToCurrencyString(yellow * 2);

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