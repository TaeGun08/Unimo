using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Vane : UI_Base
{
    public Slider[] sliders;
    public TextMeshProUGUI[] Text_Timers;
    public GameObject[] Locks;

    // 버프 시간 관련 상수
    private const float ADD_TIME = 1800f;     // 30분
    private const float MAX_TIME = 43200f;    // 720분 = 12시간

    // 광고 쿨타임 관련
    private float lastAdTime = -9999f; // 마지막 광고 시청 시각(Time.time)
    private const float AD_COOLDOWN = 300f; // 5분 = 300초

    public override void Start()
    {
        Camera_Event.instance.GetCameraEvent(CameraMoveState.Vane);
        base.Start();
    }

    public override void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            if (Base_Mng.Data.data.BuffFloating[i] > 0f)
            {
                Base_Mng.Data.data.BuffFloating[i] -= Time.deltaTime;
                if (Base_Mng.Data.data.BuffFloating[i] < 0f)
                    Base_Mng.Data.data.BuffFloating[i] = 0f;

                // 남은 시간 텍스트
                Text_Timers[i].text = ShowTimer(Base_Mng.Data.data.BuffFloating[i]);

                // 720분 기준 비율
                sliders[i].value = Base_Mng.Data.data.BuffFloating[i] / MAX_TIME;
            }
            else
            {
                ReturnTimer(i);
            }
        }
        base.Update();
    }

    public override void DisableOBJ()
    {
        base.DisableOBJ();
    }

    // 광고 버튼 클릭
    public void RewardVane(int value)
    {
        // 쿨타임 체크
        float elapsed = Time.time - lastAdTime;
        if (elapsed < AD_COOLDOWN)
        {
            float remain = AD_COOLDOWN - elapsed;
            var ts = TimeSpan.FromSeconds(remain);
            Canvas_Holder.instance.Get_Toast(
                $"광고는 {ts.Minutes:D2}:{ts.Seconds:D2} 뒤에 시청 가능"
            );
            return;
        }

        // 광고 시청
        Base_Mng.ADS.ShowRewardedAds(() =>
        {
            GetTimer(value);
            lastAdTime = Time.time; // 시청 시각 기록
        });
    }

    // 버프 시간 추가
    public void GetTimer(int value)
    {
        Canvas_Holder.instance.Get_Toast("ADS");
        Locks[value].SetActive(true);

        float currentTime = Base_Mng.Data.data.BuffFloating[value];
        currentTime += ADD_TIME;

        // 720분(43200초) 초과 방지
        if (currentTime > MAX_TIME)
            currentTime = MAX_TIME;

        Base_Mng.Data.data.BuffFloating[value] = currentTime;

        sliders[value].gameObject.SetActive(true);
        Base_Mng.Data.Save();
    }

    public void ReturnTimer(int value)
    {
        Locks[value].SetActive(false);
        Base_Mng.Data.data.BuffFloating[value] = 0.0f;
        sliders[value].gameObject.SetActive(false);
        Base_Mng.Data.Save();
    }

    // 시간 포맷
    public static string ShowTimer(double timer)
    {
        TimeSpan t = TimeSpan.FromSeconds(Convert.ToDouble(timer));
        // 시간:분:초
        return $"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";
    }
}
