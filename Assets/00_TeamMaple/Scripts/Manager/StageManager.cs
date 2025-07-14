using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("StageSO")] 
    [SerializeField] private ProceduralMapGeneratorSO so;
    public StageData StageData { get; private set; }
    
    private void Start()
    {
        StageData = StageLoader.SetStageData(so);
    }
}
