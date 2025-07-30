using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class UI_FacilitiesCake : UI_Base
{
    [SerializeField] private Button exitButton;
    [SerializeField] private Button collectButton;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI storageText;
    [SerializeField] private TextMeshProUGUI intervalText;
    [SerializeField] private List<GameObject> greenFlowers;

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

        for (int i = 0; i < greenFlowers.Count; i++)
        {
            var flower = greenFlowers[i];
            var image = flower.GetComponent<Image>();

            // 1) 현재까지 채워진 별꿀만 활성화하고 fillAmount 1로 고정
            if (i < current)
            {
                flower.SetActive(true);
                if (image != null) image.fillAmount = 1f;
            }
            // 2) 진행 중인 다음 꽃은 활성화만 시키고, fillAmount는 UpdateTimer에서 채움
            else if (i == current)
            {
                flower.SetActive(true);
                if (image != null) image.fillAmount = 0f; // 이후에 채워짐
            }
            // 3) 그 이후는 꺼두기
            else
            {
                flower.SetActive(false);
            }
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

        int current = CakeFacility.Instance.GetPendingReward();
        float fillRatio = Mathf.Clamp01(currentTimer / interval);

        if (current < greenFlowers.Count)
        {
            var image = greenFlowers[current].GetComponent<Image>();
            if (image != null) image.fillAmount = fillRatio;
        }
    }

    private void OnClick_Collect()
    {
        if (CakeFacility.Instance == null) return;

        int current = CakeFacility.Instance.GetPendingReward();
        int max = CakeFacility.Instance.GetMaxPending();

        CakeFacility.Instance.CollectReward();

        foreach (var flower in greenFlowers)
            flower.SetActive(false);

        if (current > 0 && greenFlowers.Count > 0)
        {
            greenFlowers[0].SetActive(true);
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