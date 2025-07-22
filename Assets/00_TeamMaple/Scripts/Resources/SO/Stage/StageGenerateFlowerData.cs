using UnityEngine;

[System.Serializable]
public class StageGenerateFlowerDataRecord : IStageData
{
    public int Id { get; set; }
    public float YGeneration { get; set; } //畴鄂采 积己 犬伏
    public float RGeneration { get; set; } //弧埃采 积己 犬伏
    public float BGeneration { get; set; } //颇鄂采 积己 犬伏
}

[CreateAssetMenu(fileName = "StageGenerateFlowerData", menuName = "Scriptable Object/StageGenerateFlowerData")]
public class StageGenerateFlowerData : ParsingStageData<StageGenerateFlowerDataRecord> { }
