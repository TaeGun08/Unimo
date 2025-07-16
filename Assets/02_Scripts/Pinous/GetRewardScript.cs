using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetRewardScript : MonoBehaviour
{
    public TextMeshProUGUI[] texts;
    public TextMeshProUGUI YellowText, RedText;
    public TextMeshProUGUI ScoreText;

    public GameObject ADSButton;

    [SerializeField] private Slider starBar;

    private double redTrade;
    private double yellowTrade;

    private void Start()
    {
        redTrade = double.Parse(texts[0].text) *
                   (StringMethod.ToCurrencyDouble(StageManager.Instance.StageRewardData.Star2R) / 66d);

        yellowTrade = StringMethod.ToCurrencyDouble(texts[1].text) *
                      (StringMethod.ToCurrencyDouble(StageManager.Instance.StageRewardData.Star1Y) / 133d);

        ADSButton.SetActive(true);

        Base_Mng.Data.data.Red += redTrade;
        Base_Mng.Data.data.Yellow += yellowTrade;

        YellowText.text = StringMethod.ToCurrencyString(yellowTrade);
        RedText.text = StringMethod.ToCurrencyString(redTrade);

        if (starBar.value < 0.5f) return;
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