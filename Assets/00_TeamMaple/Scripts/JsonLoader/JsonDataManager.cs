using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class JsonDataManager : SingletonBehaviour<JsonDataManager>
{
    public void SaveServerData(Server_Data data)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        PlayerPrefs.SetString("PlayerData", jsonData);
        string filePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
        File.WriteAllText(filePath, jsonData);
        Base_Mng.Firebase.WriteData(PlayerPrefs.GetString("PlayerData"), jsonData);
    }

    public Server_Data LoadServerData()
    {
        string readPath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
        string rawJson = File.ReadAllText(readPath);

        //역직렬화: JSON → Server_Data 객체
        Server_Data data = JsonConvert.DeserializeObject<Server_Data>(rawJson);
        
        return data;
    }
}
