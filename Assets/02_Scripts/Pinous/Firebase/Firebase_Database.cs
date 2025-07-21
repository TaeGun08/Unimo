using Firebase.Database;
using Firebase.Extensions;
using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public partial class Firebase_Mng: MonoBehaviour
{
    // �����ͺ��̽��� ������ ����
    public void WriteData(string key, string value)
    {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            Canvas_Holder.instance.GetUI("##Network");
        }

        // DatabaseReference childReference = reference.Child("USER").Child(key);
        // childReference.SetRawJsonValueAsync(value).ContinueWithOnMainThread(task => {
        //     if (task.Exception != null)
        //     {
        //         Debug.LogError($"������ ���� ����: {task.Exception}");
        //     }
        //     else
        //     {
        //         Debug.Log("������ ���� ����!");
        //     }
        // });
    }

    public void ReadDataOnVersion()
    {
        DatabaseReference childReference = reference.Child("ADMIN").Child("Version");
        childReference.GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.Exception != null)
            {
                Debug.LogError($"������ �б� ����: {task.Exception}");
            }
            else if (task.Result.Exists)
            {
                AccountInitializer.instance.GetCheckVersionAndLogin();
                // string versionValue = task.Result.Value.ToString();
                // if(versionValue != Application.version)
                // {
                //     VersionCheck.instance.GetVersionPopUP();
                //     return;
                // }
                // else
                // {
                //     AccountInitializer.instance.GetCheckVersionAndLogin();
                // }
            }
            else
            {
             
            }
        });
    }
    
    public void ReadData(string key, System.Action<string> onDataReceived)
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(key)))
        {
            //역직렬화: JSON → Server_Data 객체
            Server_Data data = JsonDataLoader.LoadServerData();

            //다시 직렬화: Server_Data 객체 → JSON (정상)
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

            //json 확장자 파일로 저장
            string filePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
            File.WriteAllText(filePath, json);
            
            onDataReceived?.Invoke(json);
        }
        else
        {
            string newDataJson = NewData(); // JSON string 반환
            StageLoader.StageDataReset();
            PlayerPrefs.SetString(key, newDataJson);
            
            //그대로 저장해도 됨
            string filePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
            File.WriteAllText(filePath, newDataJson);

            onDataReceived?.Invoke(newDataJson);
        }
        
        isSetFirebase = true; // TODO : 이 변수가 로딩 시작 제어 함, 평범한 bool 값 바꿔서 쓰면 될듯
        
        // DatabaseReference childReference = reference.Child("USER").Child(key);
        // // TODO : 익명 로그인 초기화
        // // reference.Child("USER")
        // //     .Child(key)
        // //     .RemoveValueAsync();
        // childReference.GetValueAsync().ContinueWithOnMainThread(task => {
        //     if (task.Exception != null)
        //     {
        //         Debug.LogError($"������ �б� ����: {task.Exception}");
        //     }
        //     else if (task.Result.Exists)
        //     {
        //         string value = task.Result.GetRawJsonValue();
        //         if(value == "" || value == null)
        //         {
        //             value = NewData();
        //         }
        //         isSetFirebase = true;
        //     
        //         onDataReceived?.Invoke(value);
        //     }
        //     else
        //     {
        //         string data = NewData();
        //         
        //         Base_Mng.Firebase.WriteData(Base_Mng.Firebase.UserID, data);
        //         isSetFirebase = true;
        //     
        //         onDataReceived?.Invoke(data);
        //     }
        // });
    }

    string NewData()
    {
        PlayerPrefs.SetFloat("Volume", 1.0f);
        PlayerPrefs.SetFloat("FXVolume", 1.0f);

        Sound_Manager.instance.SoundCheck();

        Server_Data data = new Server_Data();
        data.UserName = string.IsNullOrEmpty(AccountInitializer.instance.UserName) ?  
               "null" : AccountInitializer.instance.UserName;
        data.EXP = 0;
        data.Level = 0;
        data.Second_Base = 5;
        data.Yellow = 0;
        data.Red = 0;
        data.Blue = 0;
        data.Green = 0;
        data.HighStage = 1;
        data.CurrentStage = 1;
        data.BonusStageOn = false;
        data.NextLevel_Base = 10;
        data.GetReview = false;
        data.GetOarkTong = false;
        data.GetGameTwo = false;
        data.GetInGame = false;
        data.GetVane = false;
        data.GetStarChange = false;
        data.BuffFloating[0] = 0.0f;
        data.BuffFloating[1] = 0.0f;
        data.BuffFloating[2] = 0.0f;
        
        data.BestScoreGameOne = 0;
        data.BestScoreGameTwo = 0;

        bool[] charDatas = { true, false, false, false, false, false, false, false, false, false, false, false, false };
        bool[] EqDatas = { true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };

        data.GetCharacterData = charDatas;
        data.GetEQData = EqDatas;

        data.DailyAccount = 1;
        data.GamePlay = 0;
        data.ADS = 0;
        data.Touch = 0;
        data.TimeItem = 0;
        data.RePlay = 0;

        bool[] archive = { false, false, false, false, false, false, false, false, false };
        data.GetArchivements = archive;

        data.IAP = 0;
        data.ADSBuy = false;

        data.GetDaily = false;
        data.GetGamePlay = false;
        data.GetADS = false;
        data.GetTouch = false;
        data.GetTimeItem = false;
        data.GetRePlay = false;
        data.ADSNoneReset = 0;
        data.S_DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        data.E_DateTime = "";

        data.CharCount = 1;
        data.EQCount = 1;

        data.BonusRewardCount = 1000.0f;

        string value = JsonUtility.ToJson(data);

        return value;
    }
}
