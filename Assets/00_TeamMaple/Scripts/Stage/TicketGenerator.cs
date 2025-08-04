using System;
using TMPro;
using UnityEngine;

public class TicketGenerator : MonoBehaviour
{
    [Header("Ticket Settings")]
    [SerializeField] private float productionInterval = 7200f;
    [SerializeField] private int maxTicket = 10;

    [SerializeField] private TMP_Text time;
    [SerializeField] private TMP_Text ticket;
    private float timer;
    private int lastDisplayedHours = -1;
    private int lastDisplayedMinutes = -1;

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
            time.text = "Time - 02:00";
        }
        else
        {
            int startHours = (int)(timer / 3600);
            int startMinutes = (int)((timer % 3600) / 60);
            time.text = $"Time - {startHours}:{startMinutes:00}";

            lastDisplayedHours = startHours;
            lastDisplayedMinutes = startMinutes;
        }
    }

    private void Update()
    {
        if (Base_Mng.Data.data.GetTicket >= maxTicket)
        {
            time.text = "Time - 02:00";
            return;
        }

        timer -= Time.deltaTime;

        int currentHours = (int)(timer / 3600);
        int currentMinutes = (int)((timer % 3600) / 60);

        if (currentHours != lastDisplayedHours || currentMinutes != lastDisplayedMinutes)
        {
            time.text = $"Time - {currentHours}:{currentMinutes:00}";
            lastDisplayedHours = currentHours;
            lastDisplayedMinutes = currentMinutes;
        }

        if (timer <= 0f)
        {
            Base_Mng.Data.data.GetTicket++;
            ticket.text = $"{Base_Mng.Data.data.GetTicket} / {maxTicket}";

            if (Base_Mng.Data.data.GetTicket >= maxTicket)
            {
                time.text = "Time - 02:00";
            }
            else
            {
                timer = productionInterval;
                lastDisplayedHours = -1;
                lastDisplayedMinutes = -1;
            }
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