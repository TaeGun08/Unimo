using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpGaugeController : MonoBehaviour
{
    private readonly Color32 fullGaugeColor = new Color32(154, 228, 80, 255);
    private readonly Color32 midGaugeColor = new Color32(255, 216, 78, 255);
    private readonly Color32 lowGaugeColor = new Color32(255, 78, 78, 255);

    private readonly float midStandard = 0.75f;
    private readonly float lowStandard = 0.25f;
    private readonly float exLowStandard = 0.2f;

    private float maxHp;
    
    [SerializeField] private Image gaugeImg;
    [SerializeField] private RectTransform timerRect;
    private AudioSource cautionAudio;
    private bool canCaution = true;
    float maxWidth = 450;
    float height = 48;

    // Start is called before the first frame update
    void Start()
    {
        cautionAudio = GetComponent<AudioSource>();
        maxWidth = timerRect.transform.parent.GetComponent<RectTransform>().rect.width;
        height = timerRect.rect.height;

        maxHp = LocalPlayer.Instance.PlayerStatHolder.Hp.Value;
        
        SetGauge(1);
        
        PlaySystemRefStorage.playProcessController.SubscribePauseAction(stopCaution);
        PlaySystemRefStorage.playProcessController.SubscribeGameoverAction(stopCaution);
        PlaySystemRefStorage.playProcessController.SubscribeResumeAction(startCaution);
    }

    private void Update()
    {
        SetGauge(LocalPlayer.Instance.PlayerStatHolder.Hp.Value / maxHp);
    }

    public void SetGauge(float ratio)
    {
        ratio = Mathf.Clamp01(ratio);
        setImageColors(ratio);
        timerRect.sizeDelta = new Vector2(ratio * maxWidth, height);
    }

    private void stopCaution()
    {
        canCaution = false;
    }
    private void startCaution()
    {
        canCaution = true;
    }
    private void setImageColors(float ratio)
    {
        Color32 newcolor;
        if (ratio > midStandard)
        {
            float actratio = (ratio - midStandard) / (1f - midStandard);
            newcolor = midGaugeColor.HSVInterp(fullGaugeColor, actratio);
        }
        else if (ratio > lowStandard)
        {
            float actratio = (ratio - lowStandard) / (midStandard - lowStandard);
            newcolor = lowGaugeColor.HSVInterp(midGaugeColor, actratio);
        }
        else
        {
            newcolor = lowGaugeColor;
        }
        gaugeImg.color = newcolor;
    }
}
