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

    void OnEnable()
    {
        exitButton.onClick.AddListener(CloseSelf);
        collectButton.onClick.AddListener(OnClick_Collect);

        SetInfoText();
        UpdateUI();
        UpdateTimer();
    }

    void OnDisable()
    {
        exitButton.onClick.RemoveListener(CloseSelf);
        collectButton.onClick.RemoveListener(OnClick_Collect);
    }

    void Update() => UpdateTimer();

    void SetInfoText()
    {
        string text = Localization_Mng.LocalAccess switch
        {
            "kr" => "유니모가 사랑하는 달콤한 초록 별꿀을\n차곡차곡 만들어주는 부드러운 케이크 공방!",
            "en" => "A soft cake workshop\nthat steadily produces green Star Honey for Unimos!",
            "es" => "Un taller de pasteles\nque produce miel estelar verde para los Unimos.",
            _ => ""
        };
        infoText.text = text;
    }

    void UpdateUI()
    {
        if (CakeFacility.Instance == null) return;

        int current = CakeFacility.Instance.GetPendingReward();
        int max = CakeFacility.Instance.GetMaxPending();
        storageText.text = $"{current} / {max}";
        collectButton.interactable = current > 0;

        for (int i = 0; i < greenFlowers.Count; i++)
        {
            var go = greenFlowers[i];
            var img = go.GetComponent<Image>();

            if (i < current)            { go.SetActive(true);  if (img) img.fillAmount = 1f; }
            else if (i == current)      { go.SetActive(true);  if (img) img.fillAmount = 0f; } // 진행 중은 타이머에서 채움
            else                         { go.SetActive(false); }
        }
    }

    void UpdateTimer()
    {
        if (CakeFacility.Instance == null) return;

        int current = CakeFacility.Instance.GetPendingReward();
        float interval = CakeFacility.Instance.GetProductionInterval();
        float t = CakeFacility.Instance.GetCurrentTimer();

        if (current >= greenFlowers.Count)
        {
            intervalText.text = "생산 완료";
            return;
        }

        float remain = Mathf.Clamp(interval - t, 0f, interval);
        var ts = TimeSpan.FromSeconds(remain);
        intervalText.text = $"{ts.Hours}:{ts.Minutes:D2}:{ts.Seconds:D2}";

        // 진행 중인 꽃의 fill만 업데이트
        var img = greenFlowers[current].GetComponent<Image>();
        if (img) img.fillAmount = Mathf.Clamp01(t / interval);
    }

    void OnClick_Collect()
    {
        if (CakeFacility.Instance == null) return;

        CakeFacility.Instance.CollectReward();

        // ❌ 전부 끄지 말 것(끄면 진행바 0으로 보임)
        // foreach (var f in greenFlowers) f.SetActive(false);

        UpdateUI();   // pending만 0으로 줄고, timer는 유지되므로 다음 꽃 진행바 유지됨
        UpdateTimer();
    }

    void CloseSelf() => Canvas_Holder.CloseAllPopupUI();
}
