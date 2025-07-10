using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum Mission_State { Daily, Achievements }

public class UI_Mission : UI_Base
{
    public Mission_State m_State;
    public Mission_Base[] m_Mission;
    public Mission_Base[] m_TrophyBase;
    public Transform Content;

    public GameObject DailyObject, TrophyObject;
    public Image DailyImage, TrophyImage;
    public Outline DailyBar, TrophyBar;

    public List<Dictionary<string, object>> Data = new List<Dictionary<string, object>>();
    public override void Start()
    {
        Data = CSVReader.Read("Mission");

        GetMission(Mission_State.Daily);
        base.Start();
    }

    public void GetButton(bool Daily)
    {
        DailyObject.SetActive(Daily);
        TrophyObject.SetActive(!Daily);
        DailyBar.enabled = Daily;
        TrophyBar.enabled = !Daily;
        GetMission(Daily ? Mission_State.Daily : Mission_State.Achievements);
    }

    // ���� GetMission �޼���
    // public void GetMission(Mission_State state)
    // {
    //     int a = 0;
    //     for (int i = 0; i < Data.Count; i++)
    //     {
    //         if (Data[i]["Type"].ToString() == state.ToString())
    //         {
    //             if (state == Mission_State.Daily)
    //             {
    //                 var reward = Data[i]["Reward"].ToString().Split("_");
    //                 m_Mission[i].Init(Data[i]["Style"].ToString(), int.Parse(Data[i]["Count"].ToString()), int.Parse(reward[1]), stateCheck(reward[0]));
    //             }
    //             else if (state == Mission_State.Achievements)
    //             {
    //                 m_TrophyBase[a].Init(Data[i]["Style"].ToString(), int.Parse(Data[i]["Count"].ToString()), a, Asset_State.Blue);
    //                 a++;
    //             }
    //         }
    //     }
    // }

    // ���� ���� GetMission �޼���
    public void GetMission(Mission_State state)
    {
        if (state == Mission_State.Daily)
        {
            // 1. ���� �̼Ǹ� ���� ����
            var dailyMissions = Data
                .Where(dict => dict["Type"].ToString() == Mission_State.Daily.ToString())
                .ToList();

            // 2. ���� 2�� ���� (��: Index 0, 1 ����)
            var fixedMissions = dailyMissions.Take(2).ToList();

            // 3. ���������� ���� 1�� �̱�
            var candidates = dailyMissions.Skip(2).ToList();

            var selectedMissions = new List<Dictionary<string, object>>(fixedMissions);

            if (candidates.Count > 0)
            {
                int randIdx = Random.Range(0, candidates.Count);
                selectedMissions.Add(candidates[randIdx]);
            }

            // 4. 3�� �̼��� UI�� ��ġ
            for (int i = 0; i < m_Mission.Length; i++)
            {
                if (i < selectedMissions.Count)
                {
                    var data = selectedMissions[i];
                    var reward = data["Reward"].ToString().Split("_");
                    m_Mission[i].Init(data["Style"].ToString(), int.Parse(data["Count"].ToString()), int.Parse(reward[1]), stateCheck(reward[0]));
                }
                else
                {
                    m_Mission[i].gameObject.SetActive(false); // ���� ������ ��Ȱ��ȭ
                }
            }
        }
        else if (state == Mission_State.Achievements)
        {
            int a = 0;
            for (int i = 0; i < Data.Count; i++)
            {
                if (Data[i]["Type"].ToString() == state.ToString())
                {
                    m_TrophyBase[a].Init(Data[i]["Style"].ToString(), int.Parse(Data[i]["Count"].ToString()), a, Asset_State.Blue);
                    a++;
                }
            }
        }
    }
    
    private Asset_State stateCheck(string temp)
    {
        switch(temp)
        {
            case "Y": return Asset_State.Yellow;
            case "O": return Asset_State.Red;
            case "B": return Asset_State.Blue;
        }
        return Asset_State.Yellow;
    }

}
