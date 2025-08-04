using UnityEngine;

[System.Serializable]
public class StageFlowerConditionDataRecord : IStageData
{
    public int Id { get; set; }
    public string Star1Condition { get; set; }
    public string Star2Condition { get; set; }
    public string Star3Condition { get; set; }
}

[CreateAssetMenu(fileName = "StageFlowerConditionData", menuName = "Scriptable Object/StageFlowerConditionData")]
public class StageFlowerConditionData : ParsingStageData<StageFlowerConditionDataRecord>
{
    
}
