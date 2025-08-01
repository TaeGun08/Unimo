using System;
using UnityEngine;

public class TicketGenerator : MonoBehaviour
{
    [Header("Ticket Settings")]
    [SerializeField] private float productionInterval = 7200f; // 2½Ã°£ = 7200ÃÊ
    [SerializeField] private int maxTicket = 8;

    private float timer;

    private void Start()
    {
        string savedTimeStr = PlayerPrefs.GetString("LastExitTime", "");
        if (string.IsNullOrEmpty(savedTimeStr)) return;
    
        DateTime savedTime = DateTime.Parse(savedTimeStr);
        TimeSpan elapsed = DateTime.Now - savedTime;

        int recovered = Mathf.FloorToInt((float)elapsed.TotalSeconds / productionInterval);
        Base_Mng.Data.data.GetTicket = Mathf.Min(Base_Mng.Data.data.GetTicket + recovered, maxTicket);
        
        float remainingTime = (float)(elapsed.TotalSeconds % productionInterval);
        timer = remainingTime;
    }

    private void Update()
    {
        if (Base_Mng.Data.data.GetTicket > 0) return;
        
        timer += Time.deltaTime;
        if (timer >= productionInterval)
        {
            if (Base_Mng.Data.data.GetTicket >= maxTicket) return;
            Base_Mng.Data.data.GetTicket++;
            timer = 0f;
        }
    }

    private void OnApplicationQuit()
    {
        SaveState();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveState();
    }

    private void SaveState()
    {
        PlayerPrefs.SetString("LastExitTime", DateTime.Now.ToString());

        PlayerPrefs.Save();
    }
}