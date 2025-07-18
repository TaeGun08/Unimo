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

    private void Start()
    {
        stageManager = StageManager.Instance;

        if (Base_Mng.Data.data.BonusStageOn)
        {
            GetBonusReward();
            Debug.Log("보너스 리워드");
        }
        else
        {
            GetReward();
            Debug.Log("리워드");
        }
    }

    private void GetReward()
    {
        redTrade = double.Parse(texts[0].text) *
                   (StringMethod.ToCurrencyDouble(stageManager.StageRewardData.Star2R) / 66d);

        yellowTrade = StringMethod.ToCurrencyDouble(texts[1].text) *
                      (StringMethod.ToCurrencyDouble(stageManager.StageRewardData.Star1Y) / 133d);

        ADSButton.SetActive(true);

        YellowText.text = StringMethod.ToCurrencyString(yellowTrade);
        RedText.text = StringMethod.ToCurrencyString(redTrade);

        if (!(starBar.value < 0.01f) &&
            JsonDataLoader.LoadServerData().HighStage <= JsonDataLoader.LoadServerData().CurrentStage)
        {
            int getStar = stageManager.GetStars(JsonDataLoader.LoadServerData().CurrentStage + 1000);

            starText.text = "x0";

            switch (starBar.value)
            {
                case >= 1f when getStar != 3:
                    stageManager.UpdateStageStars(JsonDataLoader.LoadServerData().CurrentStage + 1000, 3);
                    starText.text = "x3";

                    if (getStar != 1 && getStar < 2)
                    {
                        redTrade += StringMethod.ToCurrencyDouble(stageManager.StageRewardData.Star1Y);
                    }

                    if (getStar != 2 && getStar < 3)
                    {
                        redTrade += StringMethod.ToCurrencyDouble(stageManager.StageRewardData.Star2R);
                    }

                    redTrade += StringMethod.ToCurrencyDouble(stageManager.StageRewardData.Star3Y);
                    redTrade += StringMethod.ToCurrencyDouble(stageManager.StageRewardData.Star3R);

                    break;
                case < 1f and >= 0.75f when getStar != 2:
                    stageManager.UpdateStageStars(JsonDataLoader.LoadServerData().CurrentStage + 1000, 2);
                    starText.text = "x2";

                    if (getStar != 1 && getStar < 2)
                    {
                        redTrade += StringMethod.ToCurrencyDouble(stageManager.StageRewardData.Star1Y);
                    }

                    redTrade += StringMethod.ToCurrencyDouble(stageManager.StageRewardData.Star2R);
                    break;
                case < 0.75f and >= 0.5f when getStar != 1:
                    stageManager.UpdateStageStars(JsonDataLoader.LoadServerData().CurrentStage + 1000, 1);
                    starText.text = "x1";
                    redTrade += StringMethod.ToCurrencyDouble(stageManager.StageRewardData.Star1Y);
                    break;
            }

            stageManager.UpdateStageStars(JsonDataLoader.LoadServerData().CurrentStage + 1000, 3);
            starText.text = "x3";

            Base_Mng.Data.data.HighStage++;
            Debug.Log("스테이지 업");
            if (StageLoader.IsBonusStageByIndex(JsonDataLoader.LoadServerData().HighStage))
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
                    (StringMethod.ToCurrencyDouble(stageManager.StageRewardData.Star2R) / 66d)) * 3f;

        yellowTrade = (StringMethod.ToCurrencyDouble(texts[1].text) *
                       (StringMethod.ToCurrencyDouble(stageManager.StageRewardData.Star1Y) / 133d)) * 3f;

        ADSButton.SetActive(true);

        YellowText.text = StringMethod.ToCurrencyString(yellowTrade);
        RedText.text = StringMethod.ToCurrencyString(redTrade);

        Base_Mng.Data.data.Red += redTrade;
        Base_Mng.Data.data.Yellow += yellowTrade;

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
            if (StageLoader.IsBonusStageByIndex(JsonDataLoader.LoadServerData().HighStage))
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