using UnityEngine;

[CreateAssetMenu(fileName = "ProceduralMapGeneratorSO", menuName = "Scriptable Object/ProceduralMapGeneratorSO")]
public class ProceduralMapGeneratorSO : ScriptableObject
{
    [Header("ProceduralMapCsv")]
    [SerializeField] private TextAsset proceduralMapCsv;
}
