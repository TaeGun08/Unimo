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

        redTrade = double.Parse(texts[0].text) *
                   (StringMethod.ToCurrencyDouble(stageManager.StageRewardData.Star2R) / 66d);

        yellowTrade = StringMethod.ToCurrencyDouble(texts[1].text) *
                      (StringMethod.ToCurrencyDouble(stageManager.StageRewardData.Star1Y) / 133d);

        ADSButton.SetActive(true);

        Base_Mng.Data.data.Red += redTrade;
        Base_Mng.Data.data.Yellow += yellowTrade;

        YellowText.text = StringMethod.ToCurrencyString(yellowTrade);
        RedText.text = StringMethod.ToCurrencyString(redTrade);

        if (starBar.value < 0.01f ||
            JsonDataLoader.LoadServerData().HighStage > JsonDataLoader.LoadServerData().CurrentStage) return;

        int getStar = stageManager.GetStars(JsonDataLoader.LoadServerData().CurrentStage + 1000);

        starText.text = "x0";
        
        if (starBar.value >= 1f
            && getStar != 3)
        {
            stageManager.UpdateStageStars(JsonDataLoader.LoadServerData().CurrentStage + 1000, 3);
            starText.text = "x3";
        }
        else if (starBar.value < 1f && starBar.value >= 0.75f
                                    && getStar != 2)
        {
            stageManager.UpdateStageStars(JsonDataLoader.LoadServerData().CurrentStage + 1000, 2);
            starText.text = "x2";
        }
        else if (starBar.value < 0.75f && starBar.value >= 0.5f
                                       && getStar != 1)
        {
            stageManager.UpdateStageStars(JsonDataLoader.LoadServerData().CurrentStage + 1000, 1);
            starText.text = "x1";
        }
        stageManager.UpdateStageStars(JsonDataLoader.LoadServerData().CurrentStage + 1000, 3);
        starText.text = "x3";
        Base_Mng.Data.data.HighStage++;
    }

    public void GetRewardDoubleScore()
    {
        Base_Mng.ADS.ShowRewardedAds(() =>
        {
            Base_Mng.Data.data.Red += redTrade;
            Base_Mng.Data.data.RePlay++;
            var yellow = yellowTrade;
            Base_Mng.Data.data.Yellow += yellow;

            RedText.text = redTrade.ToString();
            YellowText.text = StringMethod.ToCurrencyString(yellow * 2);

            ADSButton.SetActive(false);
        });
    }
}