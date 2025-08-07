using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Mission_Base : MonoBehaviour
{
    public Mission_State state;
    public TextMeshProUGUI m_Title, m_Count, m_RewardText;
    public Slider m_Slider;
    public Image m_Reward, m_TitleImage;
    public Button button;
    public GameObject InProgress;
    public CharCostumer m_CharCostumer;
    public int index;

    public void Init(string style, int count, int reward, Asset_State rewardType)
    {
        m_Slider.value = (float)valueCount(style) / (float)count;
        m_Count.text = valueCount(style).ToString() + "/" + count.ToString();

        button.onClick.RemoveAllListeners();
        
        if (state == Mission_State.Daily)
        {
            m_RewardText.text = reward.ToString();
            m_Reward.sprite = Data_Mng.atlas.GetSprite(rewardType.ToString());
            GetCheck(style);

            button.onClick.AddListener(() => GetButtonCheck(style, count, reward, rewardType));
        }
        else
        {
            button.onClick.AddListener(() => GetButtonTrophy(style, count, reward));
            GetCheckAchieve(reward);
        }
    }

    private void GetCheckAchieve(int value)
    {
        bool Base = Base_Mng.Data.data.GetArchivements[value];
        button.gameObject.SetActive(Base ? false : true);
        InProgress.SetActive(Base ? true : false);
    }

    private void GetCheck(string style)
    {
        switch(style)
        {
            case "DailyAccount":
                button.gameObject.SetActive(Base_Mng.Data.data.GetDaily ? false : true);
                InProgress.SetActive(Base_Mng.Data.data.GetDaily ? true : false);
                break;
            case "GamePlay": 
                button.gameObject.SetActive(Base_Mng.Data.data.GetGamePlay ? false : true);
                InProgress.SetActive(Base_Mng.Data.data.GetGamePlay ? true : false);
                break;
            case "ADS":
                button.gameObject.SetActive(Base_Mng.Data.data.GetADS ? false : true);
                InProgress.SetActive(Base_Mng.Data.data.GetADS ? true : false);
                break;
            case "UnimoEnforce":
                button.gameObject.SetActive(Base_Mng.Data.data.GetUnimoEnforce ? false : true);
                InProgress.SetActive(Base_Mng.Data.data.GetUnimoEnforce ? true : false);
                break;
            case "EngineEnforce":
                button.gameObject.SetActive(Base_Mng.Data.data.GetEngineEnforce ? false : true);
                InProgress.SetActive(Base_Mng.Data.data.GetEngineEnforce ? true : false);
                break;
            case "TreeLevelUp":
                button.gameObject.SetActive(Base_Mng.Data.data.GetTreeLevelUp ? false : true);
                InProgress.SetActive(Base_Mng.Data.data.GetTreeLevelUp ? true : false);
                break;
        }
    }

    private void GetButtonTrophy(string style, int count, int reward)
    {
        if (valueCount(style) < count) return;

        Base_Mng.m_Analytics.RecordCustomEventWithParameters("Trophy Mission Completed", reward);

        Base_Mng.Data.data.GetArchivements[reward] = true;
        Canvas_Holder.instance.NoneClose = true;
        Canvas_Holder.instance.GetUI("##Reward");
        Canvas_Holder.UI_Holder.Peek().transform.parent = Canvas_Holder.instance.transform;
        if (m_CharCostumer == CharCostumer.Charcater)
            Canvas_Holder.UI_Holder.Peek().GetComponent<UI_Reward>().GetRewardInit(RewardState.Character, CharCostumer.Charcater, 0, index, 1);
        else Canvas_Holder.UI_Holder.Peek().GetComponent<UI_Reward>().GetRewardInit(RewardState.Character, CharCostumer.EQ, 0, 1, index);

        GetCheckAchieve(reward);
    }
    private void GetButtonCheck(string style, int count, int reward, Asset_State rewardType)
    {
        if (valueCount(style) < count) return;
        Base_Mng.m_Analytics.RecordCustomEventWithParameters("Trophy Mission Completed", reward);

        switch (style)
        {
            case "DailyAccount": Base_Mng.Data.data.GetDaily = true; break;
            case "GamePlay": Base_Mng.Data.data.GetGamePlay = true; break;
            case "ADS": Base_Mng.Data.data.GetADS = true; break;
            case "UnimoEnforce": Base_Mng.Data.data.GetUnimoEnforce = true; break;
            case "EngineEnforce": Base_Mng.Data.data.GetEngineEnforce = true; break;
            case "TreeLevelUp": Base_Mng.Data.data.GetTreeLevelUp = true; break;
        }

        if (Base_Mng.Data.data.DailyQuestCount < 84)
        {
            Base_Mng.Data.data.DailyQuestCount++;
        }
        Canvas_Holder.instance.NoneClose = true;
        Canvas_Holder.instance.GetUI("##Reward");
        Canvas_Holder.UI_Holder.Peek().transform.parent = Canvas_Holder.instance.transform;
        Canvas_Holder.UI_Holder.Peek().GetComponent<UI_Reward>().GetRewardInit(RewardState.Other, CharCostumer.Charcater, reward);

        GetCheck(style);

    }

    public int valueCount(string style)
    {
        switch(style)
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
            //case "IAP": return Base_Mng.Data.data.IAP;
            // case "Collection":
            //     int a = 0;
            //     for(int i = 0; i < Base_Mng.Data.data.GetCharacterData.Length; i++)
            //     {
            //         if (Base_Mng.Data.data.GetCharacterData[i] == true)
            //         {
            //             a++;
            //         }
            //     }
            //     return a;
            // case "Collection_EQ":
            //     int b = 0;
            //     for(int i = 0; i < Base_Mng.Data.data.GetEQData.Length; i++)
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

    public void ClaimButton()
    {

    }
}
