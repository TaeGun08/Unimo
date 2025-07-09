using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabsTable", menuName = "Scriptable Object/PrefabsTable")]

public class PrefabsTable : ScriptableObject
{
    [Serializable]
    public class PrefabEntry
    {
        public int prefabKey;
        public GameObject prefab;
    }
    
    public PrefabEntry[] prefabEntries;
    
    private Dictionary<int, GameObject> prefabsMap;
    
    private void OnEnable()
    {
        prefabsMap = new Dictionary<int, GameObject>();
        foreach (var t in prefabEntries)
        {
            prefabsMap.Add(t.prefabKey, t.prefab);
        }
    }

    public GameObject GetPrefabByKey(int key)
    {
        return prefabsMap.GetValueOrDefault(key);
    }
    
    public GameObject GetPrefabByIndex(int index)
    {
        return prefabEntries[index].prefab;
    }
}
