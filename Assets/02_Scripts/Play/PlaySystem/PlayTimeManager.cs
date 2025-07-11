using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTimeManager : MonoBehaviour
{
    public float LapseTime { get; private set; } = 0f;
    [SerializeField] private bool isInfinite = false;
    [SerializeField] private float reduceIncTime = 120f;
    private float maxTime = 0f;
    private float remainTime = 0f;
    private float minReduce = 1f;
    private bool isPaused;
    private ItemGenerator itemGenerator;
    [SerializeField] private TimeGaugeController timerGauge;
    void Awake()
    {
        PlaySystemRefStorage.playTimeManager = this;
        itemGenerator = FindAnyObjectByType<ItemGenerator>();
    }

    private void Start()
    {
        maxTime = LocalPlayer.Instance.CurMaxHp;
        remainTime = maxTime;
        timerGauge.SetGauge(remainTime / maxTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (LocalPlayer.Instance == null) return;
        if (isPaused) return; 
        
        LapseTime += Time.deltaTime;
        float rate = calcReduceRate(LapseTime);
        itemGenerator.DecreaseTick(Time.deltaTime * rate);
        ChangeTimer(-Time.deltaTime * rate);
    }
    
    public void InitTimer()
    {
        remainTime = maxTime;
        isPaused = true;
        StartCoroutine(CoroutineExtensions.DelayedActionCall(() => { ToggleTimer(); }, PlayProcessController.InitTimeSTATIC));
    }
    public void ToggleTimer()
    {
        isPaused = !isPaused;
    }
    public void ChangeTimer(float tchange)
    {
        if (isInfinite) { return; }
        remainTime += tchange;
        if (remainTime > maxTime) 
        {
            remainTime = maxTime; 
        }
        if (remainTime < 0f) 
        { 
            remainTime = 0f;
            timeUp();
        }
        timerGauge.SetGauge(remainTime / maxTime);
    }
    public float GetMaxTime()
    {
        return maxTime;
    }
    public float GetRemainTimeRatio()
    {
        return remainTime / maxTime;
    }
    private float calcReduceRate(float lapse)
    {
        float ratio = lapse / reduceIncTime;
        float rate = minReduce * ((0.7f * ratio * ratio + 0.3f * Mathf.Pow(ratio,3.2f)) + (1+Mathf.Exp(0.78f*ratio))/2);
        return rate;
    }
    private void timeUp()
    {
        isPaused = true;
        PlaySystemRefStorage.playProcessController.TimeUp();
    }
}
