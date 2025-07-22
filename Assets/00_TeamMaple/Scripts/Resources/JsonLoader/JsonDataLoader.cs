using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class JsonDataLoader
{
    public static void SaveServerData(Server_Data data)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        PlayerPrefs.SetString("PlayerData", jsonData);
        string filePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
        File.WriteAllText(filePath, jsonData);
    }

    public static Server_Data LoadServerData()
    {
        string readPath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
        string rawJson = File.ReadAllText(readPath);

        //������ȭ: JSON �� Server_Data ��ü
        Server_Data data = JsonConvert.DeserializeObject<Server_Data>(rawJson);
        
        return data;
    }
}
