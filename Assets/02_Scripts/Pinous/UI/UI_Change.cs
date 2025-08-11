using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Change : UI_Base
{
    double blueCostTotal;   // 소모 블루 총합
    double orangeGain;      // 오렌지 획득량
    double yellowGain;      // 옐로우 획득량

    public Slider OrangeSlider, YellowSlider;
    public TextMeshProUGUI Blue, Orange, Yellow;

    double BlueOwned => Base_Mng.Data.data.Blue;
    double SecondBase => Base_Mng.Data.data.Second_Base;

    public override void Start()
    {
        SetupSlidersMax();
        OrangeSlider.onValueChanged.AddListener(ValueChangeSlider_Orange);
        YellowSlider.onValueChanged.AddListener(ValueChangeSlider_Yellow);
        RecalcAndRefresh();
        base.Start();
    }

    void SetupSlidersMax()
    {
        // 슬라이더는 float이므로 안전 클램프
        float max = Mathf.Max(0f, (float)Mathf.Min((float)BlueOwned, 1e9f));
        OrangeSlider.minValue = 0f;
        YellowSlider.minValue = 0f;
        OrangeSlider.maxValue = max;
        YellowSlider.maxValue = max;

        if (OrangeSlider.value > max) OrangeSlider.value = max;
        if (YellowSlider.value > max) YellowSlider.value = max;
    }

    void RecalcAndRefresh()
    {
        // 현재 슬라이더 값으로 계산
        orangeGain = (double)OrangeSlider.value * 500.0;
        yellowGain = (double)YellowSlider.value * SecondBase;
        blueCostTotal = (orangeGain / 500.0) + (yellowGain / SecondBase);

        // UI 표시 (A,B,C... 단위 변환)
        Blue.text   = FormatWithLetter(blueCostTotal);
        Blue.color  = (BlueOwned + 1e-9) >= blueCostTotal ? Color.green : Color.red;
        Orange.text = FormatWithLetter(orangeGain);
        Yellow.text = FormatWithLetter(yellowGain);
    }

    // 숫자를 1e3 = A, 1e6 = B ... 연속 알파벳 단위로 변환
    string FormatWithLetter(double value)
    {
        if (value < 1000)
            return value.ToString("0.##");

        int exponent = (int)(Math.Floor(Math.Log10(value) / 3)); // 3자리마다 단위
        double shortValue = value / Math.Pow(1000, exponent);

        string letter = GetLetterUnit(exponent - 1); // 1e3 → A
        return shortValue.ToString("0.##") + letter;
    }

    // 0→A, 1→B, ... 25→Z, 26→AA ...
    string GetLetterUnit(int index)
    {
        string result = "";
        while (index >= 0)
        {
            result = (char)('A' + (index % 26)) + result;
            index = index / 26 - 1;
        }
        return result;
    }

    void ValueChangeSlider_Orange(float value)
    {
        double alreadyAllocatedByYellow = yellowGain / SecondBase;
        double remainBlue = Math.Max(0.0, BlueOwned - alreadyAllocatedByYellow);

        // 두 인자 모두 float
        float maxCap = Mathf.Min((float)remainBlue, 1e9f);
        float clamped = Mathf.Clamp(value, 0f, maxCap);  // 네임드 인자 제거

        if (!Mathf.Approximately(clamped, value))
            OrangeSlider.value = clamped;

        RecalcAndRefresh();
    }

    void ValueChangeSlider_Yellow(float value)
    {
        double alreadyAllocatedByOrange = orangeGain / 500.0;
        double remainBlue = Math.Max(0.0, BlueOwned - alreadyAllocatedByOrange);

        float maxCap = Mathf.Min((float)remainBlue, 1e9f);
        float clamped = Mathf.Clamp(value, 0f, maxCap);

        if (!Mathf.Approximately(clamped, value))
            YellowSlider.value = clamped;

        RecalcAndRefresh();
    }


    public override void DisableOBJ()
    {
        base.DisableOBJ();
    }

    public void GetExchange()
    {
        if (blueCostTotal <= 0.0)
        {
            Canvas_Holder.instance.Get_Toast("NoneBlue");
            return;
        }

        if ((BlueOwned + 1e-9) >= blueCostTotal)
        {
            Base_Mng.Data.data.Blue   -= blueCostTotal;
            Base_Mng.Data.data.Red    += orangeGain;
            Base_Mng.Data.data.Yellow += yellowGain;

            Main_UI.instance.Text_Check();
            Canvas_Holder.instance.Get_Toast("SuccessChange");
            DisableOBJ();
        }
        else
        {
            Canvas_Holder.instance.Get_Toast("NM");
        }
    }
}
