using System;
using UnityEngine;

public class CakeFacility : MonoBehaviour
{
    public static CakeFacility Instance { get; private set; }

    [SerializeField] int    maxPending      = 10;      // 저장 한도
    [SerializeField] float  secondsPerOne   = 11520f;  // 8시간/1개

    int     pending;        // 완료돼 수령 대기 중
    float   timer;          // 다음 1개까지 진행 시간
    DateTime lastUtc;       // 오프라인 기준

    void Awake()
    {
        if (Instance == null) Instance = this;
        Load();
        ApplyOfflineProduction();
    }

    void Update()
    {
        if (pending >= maxPending) return;

        timer += Time.deltaTime;
        while (timer >= secondsPerOne && pending < maxPending)
        {
            timer -= secondsPerOne;
            pending++;
        }
    }

    // === 외부 접근 ===
    public int   GetPendingReward()        => pending;
    public int   GetMaxPending()           => maxPending;
    public float GetProductionInterval()   => secondsPerOne;
    public float GetCurrentTimer()         => timer;

    // ★ 수령: pending만 0으로. timer는 건드리지 않음!
    public void CollectReward()
    {
        if (pending <= 0) return;

        Base_Mng.Data.data.Green += pending;
        pending = 0;

        Save();
        Main_UI.instance.Text_Check();
        Canvas_Holder.instance.Get_Toast("CollectSuccess");
    }

    // === 오프라인 생산 반영 ===
    void ApplyOfflineProduction()
    {
        var now = DateTime.UtcNow;
        var gap = Math.Max(0, (int)(now - lastUtc).TotalSeconds);

        if (gap > 0 && pending < maxPending)
        {
            double total = timer + gap;
            int add = Mathf.Min((int)(total / secondsPerOne), maxPending - pending);
            pending += add;
            timer = (float)(total - add * secondsPerOne);
        }

        lastUtc = now;
        Save();
    }

    void OnApplicationPause(bool pause) { if (pause) Save(); }
    void OnApplicationQuit()            { Save(); }

    void Save()
    {
        PlayerPrefs.SetInt("cake_pending", pending);
        PlayerPrefs.SetFloat("cake_timer", timer);
        PlayerPrefs.SetString("cake_lastUtc", DateTime.UtcNow.ToString("o"));
        PlayerPrefs.Save();
    }

    void Load()
    {
        pending = PlayerPrefs.GetInt("cake_pending", 0);
        timer   = PlayerPrefs.GetFloat("cake_timer", 0f);
        var s   = PlayerPrefs.GetString("cake_lastUtc", "");
        lastUtc = string.IsNullOrEmpty(s) ? DateTime.UtcNow :
                  DateTime.Parse(s, null, System.Globalization.DateTimeStyles.RoundtripKind);
    }
}
