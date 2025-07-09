using System.Collections.Generic;
using UnityEngine;

public class StageGimmickManager : MonoBehaviour
{
    [SerializeField] private List<StageGimmickData> gimmickDataList;

    private Dictionary<int, StageGimmickInfo> gimmickInfoByStage = new Dictionary<int, StageGimmickInfo>();

    private void Awake()
    {
        InitializeGimmickInfos();

        // 임시 테스트: 현재 스테이지를 51로 설정해서 실행
        TryApplyGimmick(51); 
    }

    private void InitializeGimmickInfos()
    {
        foreach (var data in gimmickDataList)
        {
            for (int stage = data.stageStart; stage <= data.stageEnd; stage++)
            {
                if (!gimmickInfoByStage.ContainsKey(stage))
                {
                    gimmickInfoByStage.Add(stage, new StageGimmickInfo(stage, data));
                }
                else
                {
                    Debug.LogWarning($"Stage {stage} already has gimmick info. Skipping.");
                }
            }
        }
    }

    public StageGimmickInfo GetGimmickInfo(int stageNum)
    {
        if (gimmickInfoByStage.TryGetValue(stageNum, out var info))
        {
            return info;
        }

        return new StageGimmickInfo(stageNum, new StageGimmickData { gimmickType = StageGimmickType.None, isBonus = false });
    }

    public void TryApplyGimmick(int stageNum)
    {
        var gimmick = GetGimmickInfo(stageNum);
        Debug.Log($"[기믹 적용] 스테이지 {stageNum} → {gimmick.GimmickType}");

        // StageGimmickSO 기반 기믹 실행
        gimmick.gimmickLogic?.Execute(transform.position);
    }
}