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

[System.Serializable]
public class PlanetData
{
    public int ReturnId;
    public Sprite PlanetSprite;
    public Material BG;
    public Material Land;
    public Material Rock;
}

[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Object/StageData")]
public class StageData : ParsingStageData<StageDataRecord>
{
    [Header("Planet Image")] [SerializeField]
    private PlanetData[] planetData;

    public PlanetData GetPlanetData(int index)
    {
        if (StageLoader.IsBonusStageByIndex(index)) return null;
        switch (GetData(index + 1000).PlanetName)
        {
            case "넓은별":
            case "넓은별_블랙홀":
                return planetData[0];
            case "낙뢰의별":
            case "낙뢰의 별_블랙홀":
                return planetData[1];
            case "초록가스 별":
            case "초록가스별_블랙홀":
                return planetData[2];
            case "얼어붙은별":
            case "얼어붙은별_블랙홀":
                return planetData[3];
            case "분출하는별":
            case "분출하는별_블랙홀":
                return planetData[4];
            case "검은별":
            case "검은별_블랙홀":
                return planetData[5];
            case "질풍의 별":
            case "질풍의별_블랙홀":
                return planetData[6];
            case "안개구름 별":
            case "안개구름별_블랙홀":
                return planetData[7];
            case "불안정한 별":
            case "불안정한별_블랙홀":
                return planetData[8];
            case "시간의 별":
            case "시간의별_블랙홀":
                return planetData[9];
        }
        
        return planetData[0];
    }
}