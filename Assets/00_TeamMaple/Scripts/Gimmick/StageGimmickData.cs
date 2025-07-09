using Unity.VisualScripting;
using UnityEngine;

public enum StageGimmickType
{
    None, //넓은 별
    LightningStrike, //낙뢰의 별
    PoisonGas,  //초록가스 별
    SlipperyFloor, //얼어붙은 별
    MeteorFall, //분출하는 별
    Darkness, //검은 별
    WindPush, //질풍의 별
    FogDamage, //안개 구름 별
    TimeSlow, //시간의 별
    Earthquake //불안정한 별
}

public class StageGimmickInfo
{
    public int StageNum; //현재 스테이지
    public StageGimmickType GimmickType; //현재 스테이지의 기믹타입
    public bool IsBonus; //현재 스테이지가 보너스 스테이지인지 확인
    public StageGimmickSO gimmickLogic;
    
    public StageGimmickInfo(int stageNum, StageGimmickData data)
    {
        this.StageNum = stageNum; //스테이지 넘버 참조
        this.GimmickType = data.gimmickType; //기믹 타입 참조
        this.IsBonus = data.isBonus; //보너스 스테이지 유무 참조
        this.gimmickLogic = data.gimmickLogic;
    }
}

[CreateAssetMenu(fileName = "StageGimmikData", menuName = "Scriptable Objects/StageGimmikData")]
public class StageGimmickData : ScriptableObject
{
    public int stageStart; //시작 스테이지
    public int stageEnd; //끝 스테이지
    public StageGimmickType gimmickType; //기믹 타입
    public bool isBonus; //보너스 스테이지인지 확인용(10스테이지마다 true체크)
    public StageGimmickSO gimmickLogic; // 기믹 로직관련 SO
}
