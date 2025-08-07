using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionFinder : MonoBehaviour
{
    public List<Dictionary<string, object>> Data = new List<Dictionary<string, object>>();

    public bool GetCompletedMission = false;

    public GameObject MissionMark;

    private void Start()
    {
        Data = CSVReader.Read("Mission");

        StartCoroutine(InitCoroutine());
    }
    public void InitCheckCompleted()
    {
        GetCompletedMission = false;
        int a = 0;
        for (int i = 0; i < Data.Count; i++)
        {
            string style = Data[i]["Style"].ToString();
            int count = int.Parse(Data[i]["Count"].ToString());
            string type = Data[i]["Type"].ToString();
            bool Checking = false;
            if(type == "Daily")
            {
                Checking = GetCheck(style);
            }
            else if(type == "Achievements")
            {
                Checking = GetCheckAchieve(a);
                a++;
            }
            if (Checking == false)
            {
                GetCompletedMission = valueCount(style) >= count;
                if (GetCompletedMission == true) break;
            }
        }
        MissionMark.SetActive(GetCompletedMission);
    }
    private bool GetCheckAchieve(int value)
    {
        return Base_Mng.Data.data.GetArchivements[value];
    }


    private bool GetCheck(string style)
    {
        switch (style)
        {
            case "DailyAccount":
                return Base_Mng.Data.data.GetDaily;
            case "GamePlay": 
                return Base_Mng.Data.data.GetGamePlay;
            case "ADS":
                return Base_Mng.Data.data.GetADS;
            case "UnimoEnforce":
                return Base_Mng.Data.data.GetUnimoEnforce;
            case "EngineEnforce":
                return Base_Mng.Data.data.GetEngineEnforce;
            case "TreeLevelUp":
                return Base_Mng.Data.data.GetTreeLevelUp;
        }
        return false;
    }
    IEnumerator InitCoroutine()
    {
        InitCheckCompleted();
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(InitCoroutine());
    }

    public int valueCount(string style)
    {
        switch (style)
        {
            case "DailyAccount": return Base_Mng.Data.data.DailyAccount;
            case "GamePlay": return Base_Mng.Data.data.GamePlay;
            case "ADS": return Base_Mng.Data.data.ADS;
            case "UnimoEnforce": return Base_Mng.Data.data.UnimoEnforce;
            case "EngineEnforce":  return Base_Mng.Data.data.EngineEnforce;
            case "TreeLevelUp": return Base_Mng.Data.data.TreeLevelUp;
            case "DailyQuestCount": return Base_Mng.Data.data.DailyQuestCount;
            case "Stage3Star": return Base_Mng.Data.data.Stage3Star;
            case "StageClear50": return Base_Mng.Data.data.StageClear50;
            case "BlackHole": return Base_Mng.Data.data.BlackHole;
            case "ADSNONE": return Base_Mng.Data.data.ADSNoneReset;
            case "BonusStage": return Base_Mng.Data.data.BonusStage;
            case "Level": return Base_Mng.Data.data.Level + 1;
            // case "IAP": return Base_Mng.Data.data.IAP;
            // case "Collection":
            //     int a = 0;
            //     for (int i = 0; i < Base_Mng.Data.data.GetCharacterData.Length; i++)
            //     {
            //         if (Base_Mng.Data.data.GetCharacterData[i] == true)
            //         {
            //             a++;
            //         }
            //     }
            //     return a;
            // case "Collection_EQ":
            //     int b = 0;
            //     for (int i = 0; i < Base_Mng.Data.data.GetEQData.Length; i++)
            //     {
            //         if (Base_Mng.Data.data.GetEQData[i] == true)
            //         {
            //             b++;
            //         }
            //     }
            //     return b;
        }
        return -1;
    }
}
