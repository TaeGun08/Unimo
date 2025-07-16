using System;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("StageSO")] 
    [SerializeField] private ProceduralMapGeneratorSO so;
    public StageData StageData { get; private set; }
    public StageRewardData StageRewardData { get; private set; }
    
    private void Awake()
    {
        Instance = this;
        
        StageData = new StageData();
        StageRewardData = new StageRewardData();
        
        so.InitData();
        
        StageData = StageLoader.SetStageData(so);
        StageRewardData = so.GetStageRewardData(JsonDataLoader.LoadServerData().CurrentStage + 1000);
    }
}
