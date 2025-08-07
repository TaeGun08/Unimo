using System;
using TMPro;
using UnityEngine;

public class TicketGenerator : MonoBehaviour
{
    [Header("Ticket Settings")]
    [SerializeField] private float productionInterval = 6000f; // 100Ка = 6000УЪ
    [SerializeField] private int maxTicket = 10;

    [SerializeField] private TMP_Text time;
    [SerializeField] private TMP_Text ticket;
    private float timer;
    private int lastDisplayedHours = -1;
    private int lastDisplayedMinutes = -1;
    private int lastDisplayedSeconds = -1;

    private void Start()
    {
        string savedTimeStr = PlayerPrefs.GetString("LastExitTime", "");
        if (!string.IsNullOrEmpty(savedTimeStr))
        {
            DateTime savedTime = DateTime.Parse(savedTimeStr);
            TimeSpan elapsed = DateTime.Now - savedTime;

            int recovered = Mathf.FloorToInt((float)elapsed.TotalSeconds / productionInterval);
            Base_Mng.Data.data.GetTicket = Mathf.Min(Base_Mng.Data.data.GetTicket + recovered, maxTicket);

            float remainingTime = (float)(elapsed.TotalSeconds % productionInterval);
            timer = productionInterval - remainingTime;
        }
        else
        {
            timer = productionInterval;
        }

        ticket.text = $"{Base_Mng.Data.data.GetTicket} / {maxTicket}";

        if (Base_Mng.Data.data.GetTicket >= maxTicket)
        {
            time.text = FormatTime(productionInterval);
        }
        else
        {
            UpdateTimeDisplay();
        }
    }

    private void Update()
    {
        if (Base_Mng.Data.data.GetTicket >= maxTicket)
        {
            time.text = FormatTime(productionInterval);
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Base_Mng.Data.data.GetTicket++;
            ticket.text = $"{Base_Mng.Data.data.GetTicket} / {maxTicket}";

            if (Base_Mng.Data.data.GetTicket >= maxTicket)
            {
                time.text = FormatTime(productionInterval);
            }
            else
            {
                timer = productionInterval;
                lastDisplayedHours = -1;
                lastDisplayedMinutes = -1;
                lastDisplayedSeconds = -1;
            }
        }
        else
        {
            UpdateTimeDisplay();
        }
    }

    private void UpdateTimeDisplay()
    {
        int currentHours = (int)(timer / 3600);
        int currentMinutes = (int)((timer % 3600) / 60);
        int currentSeconds = (int)(timer % 60);

        if (currentHours != lastDisplayedHours || currentMinutes != lastDisplayedMinutes || currentSeconds != lastDisplayedSeconds)
        {
            time.text = $"Time - {currentHours:D2}:{currentMinutes:D2}:{currentSeconds:D2}";
            lastDisplayedHours = currentHours;
            lastDisplayedMinutes = currentMinutes;
            lastDisplayedSeconds = currentSeconds;
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        int h = (int)(timeInSeconds / 3600);
        int m = (int)((timeInSeconds % 3600) / 60);
        int s = (int)(timeInSeconds % 60);
        return $"Time - {h:D2}:{m:D2}:{s:D2}";
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
