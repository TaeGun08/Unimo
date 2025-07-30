using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpritesTable", menuName = "Scriptable Object/SpritesTable")]

public class SpriteTable : ScriptableObject
{
    [Serializable]
    public class SpriteEntry
    {
        public int spriteKey;
        public Sprite sprite;
    }
    
    public SpriteEntry[] spriteEntries;
    
    private Dictionary<int, Sprite> spritesMap;
    
    private void OnEnable()
    {
        spritesMap = new Dictionary<int, Sprite>();
        foreach (var t in spriteEntries)
        {
            spritesMap.Add(t.spriteKey, t.sprite);
        }
    }

    public Sprite GetSpriteByKey(int key)
    {
        return spritesMap.GetValueOrDefault(key);
    }
    
    public Sprite GetSpriteByIndex(int index)
    {
        return spriteEntries[index].sprite;
    }
}
