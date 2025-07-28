using UnityEngine;
using System;

public class CakeFacility : MonoBehaviour
{
    public static CakeFacility Instance { get; private set; }

    [SerializeField] private Data_Mng dataMng;
    [SerializeField] private float productionInterval = 11520f; // 8 hours
    [SerializeField] private int maxPending = 10;

    private float timer = 0f;
    private int pendingAmount = 0;
    private bool isProducing = false;

    private const string PendingKey = "Cake_PendingAmount";
    private const string LastExitKey = "Cake_LastExitTime";

    private void Awake()
    {
        Instance = this;
        pendingAmount = PlayerPrefs.GetInt(PendingKey, 0);
    }

    private void Start()
    {
        CalculateOfflineProduction();
    }

    private void Update()
    {
        if (!isProducing) return;

        timer += Time.deltaTime;

        if (timer >= productionInterval)
        {
            timer -= productionInterval;

            if (pendingAmount < maxPending)
            {
                pendingAmount++;
                PlayerPrefs.SetInt(PendingKey, pendingAmount);
                Debug.Log($"[케이크] 초록 별꿀 +1 누적 (대기: {pendingAmount})");
            }
            else
            {
                Debug.Log($"[케이크] 생산 중단 (대기량 {pendingAmount}개 ≥ 최대 {maxPending})");
            }
        }
    }

    private void OnApplicationQuit()
    {
        SaveExitTime();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveExitTime();
    }

    private void SaveExitTime()
    {
        PlayerPrefs.SetString(LastExitKey, DateTime.Now.ToString("O"));
        PlayerPrefs.SetInt(PendingKey, pendingAmount);
        PlayerPrefs.Save();
    }

    private void CalculateOfflineProduction()
    {
        if (!PlayerPrefs.HasKey(LastExitKey)) return;

        if (!DateTime.TryParse(PlayerPrefs.GetString(LastExitKey), out DateTime lastExit)) return;

        TimeSpan elapsed = DateTime.Now - lastExit;
        int produced = Mathf.FloorToInt((float)(elapsed.TotalSeconds / productionInterval));

        pendingAmount = Mathf.Min(pendingAmount + produced, maxPending);

        PlayerPrefs.SetInt(PendingKey, pendingAmount);
        PlayerPrefs.Save();

        Debug.Log($"[오프라인 보상] 초록 별꿀 +{produced}개 누적 (총 대기: {pendingAmount})");
    }

    public void StartProduction()
    {
        isProducing = true;
        timer = 0f;
    }

    public void StopProduction()
    {
        isProducing = false;
        SaveExitTime();
    }

    public int GetPendingReward() => pendingAmount;

    public void CollectReward()
    {
        if (pendingAmount <= 0) return;

        dataMng.AssetPlus(Asset_State.Green, pendingAmount);
        Debug.Log($"[케이크] 초록 별꿀 {pendingAmount}개 수령됨");

        pendingAmount = 0;
        PlayerPrefs.SetInt(PendingKey, 0);
        PlayerPrefs.Save();
    }
}
