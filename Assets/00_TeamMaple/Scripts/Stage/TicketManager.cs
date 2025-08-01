using System;
using UnityEngine;

public class TicketManager : MonoBehaviour
{
    [Header("Ticket Settings")]
    [SerializeField] private float productionInterval = 7200f; // 2½Ã°£ = 7200ÃÊ
    [SerializeField] private int maxTicket = 8;

    private float timer;
    
    public int CurTicket { get; set; }

    private void Start()
    {
        CurTicket = PlayerPrefs.GetInt("CurrentTicket", maxTicket);
        
        string savedTimeStr = PlayerPrefs.GetString("LastExitTime", "");
        if (string.IsNullOrEmpty(savedTimeStr)) return;
        DateTime savedTime = DateTime.Parse(savedTimeStr);
        TimeSpan elapsed = DateTime.Now - savedTime;
        
        int recovered = Mathf.FloorToInt((float)elapsed.TotalSeconds / productionInterval);
        CurTicket = Mathf.Min(CurTicket + recovered, maxTicket);
    }

    private void Update()
    {
        timer += Time.unscaledDeltaTime;
        if (timer >= productionInterval)
        {
            if (CurTicket >= maxTicket) return;
            CurTicket++;
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
        
        PlayerPrefs.SetInt("CurrentTicket", CurTicket);

        PlayerPrefs.Save();
    }
}