using UnityEngine;

[System.Serializable]
public class StageRewardDataRecord : IStageData
{
    public int Id { get; set; } //스테이지 보상 아이디
    public string Star1Y { get; set; } //스테이지 1별 달성 보상
    public string Star2R { get; set; } //스테이지 2별 달성 보상
    public string Star3Y { get; set; } //스테이지 3별 달성 보상
    public string Star3R { get; set; } //스테이지 3별 달성 보상
}

[System.Serializable]
public class StageDataRecord : IStageData
{
    public int Id { get; set; }
    public string StageName { get; set; } //스테이지 이름
    public string PlanetName { get; set; } //행성 이름
}

[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Object/StageData")]
public class StageData : ParsingStageData<StageDataRecord> { }
