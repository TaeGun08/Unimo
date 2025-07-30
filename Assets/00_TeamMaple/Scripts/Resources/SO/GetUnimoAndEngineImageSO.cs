using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GetUnimoAndEngineImageSO", menuName = "Scriptable Object/GetUnimoAndEngineImageSO")]
public class GetUnimoAndEngineImageSO : ScriptableObject
{
    [System.Serializable]
    public class GetSprite
    {
        public Sprite[] UnimoSprite;
        public Sprite[] EngineSprite;
    }
    
    [Header("Sprites")]
    [SerializeField] private GetSprite getSprites;
    public GetSprite GetSprites => getSprites;
}
