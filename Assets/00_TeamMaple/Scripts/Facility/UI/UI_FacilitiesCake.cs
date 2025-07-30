using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_FacilitiesCake : UI_Base
{
    [SerializeField] private Button exitButton;
    [SerializeField] private Button collectButton;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI storageText;
    [SerializeField] private TextMeshProUGUI intervalText;
    [SerializeField] private Transform flowerRoot;

    private void OnEnable()
    {
        exitButton.onClick.AddListener(CloseSelf);
        collectButton.onClick.AddListener(OnClick_Collect);

        SetInfoText();
        UpdateUI();
    }

    private void Update()
    {
        UpdateTimer();
    }

    private void SetInfoText()
    {
        string text = Localization_Mng.LocalAccess switch
        {
            "kr" => "유니모가 사랑하는 달콤한 초록 별꿀을\n차곡차곡 만들어주는 부드러운 케이크 공방!",
            "en" => "A soft and sweet cake workshop\nthat gently produces green Star Honey for Unimos!",
            "es" => "Un taller de pasteles que produce miel estelar verde\nque encanta a los Unimos!",
            _ => ""
        };
        infoText.text = text;
    }

    private void UpdateUI()
    {
        if (CakeFacility.Instance == null) return;

        int current = CakeFacility.Instance.GetPendingReward();
        int max = CakeFacility.Instance.GetMaxPending();

        storageText.text = $"{current} / {max}";

        for (int i = 0; i < flowerRoot.childCount; i++)
        {
            var green = flowerRoot.GetChild(i).Find("Flower(green)").gameObject;
            green.SetActive(i < current);
        }
    }

    private void UpdateTimer()
    {
        if (CakeFacility.Instance == null) return;

        float interval = CakeFacility.Instance.GetProductionInterval();
        float currentTimer = CakeFacility.Instance.GetCurrentTimer();

        float remain = Mathf.Clamp(interval - currentTimer, 0f, interval);
        TimeSpan ts = TimeSpan.FromSeconds(remain);
        intervalText.text = $"{ts.Hours}:{ts.Minutes:D2}:{ts.Seconds:D2}";
    }

    private void OnClick_Collect()
    {
        if (CakeFacility.Instance == null) return;

        int current = CakeFacility.Instance.GetPendingReward();
        int max = CakeFacility.Instance.GetMaxPending();

        CakeFacility.Instance.CollectReward();

        for (int i = 0; i < flowerRoot.childCount; i++)
        {
            var green = flowerRoot.GetChild(i).Find("Flower(green)").gameObject;
            green.SetActive(false);
        }

        if (current > 0)
        {
            var green = flowerRoot.GetChild(0).Find("Flower(green)").gameObject;
            green.SetActive(true);
            storageText.text = $"1 / {max}";
        }
        else
        {
            storageText.text = $"0 / {max}";
        }
    }

    private void CloseSelf()
    {
        Canvas_Holder.CloseAllPopupUI();
    }
}