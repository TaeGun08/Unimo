using UnityEngine;

public class StageAutoGimmickStarter : MonoBehaviour
{
    public StageGimmickManager gimmickManager;

    private void Start()
    {
        if (gimmickManager == null)
        {
            Debug.LogError("[오류] StageGimmickManager가 연결되지 않았습니다.");
            return;
        }

        var serverData = JsonDataLoader.LoadServerData();
        int currentStage = serverData.CurrentStage;

        Debug.Log($"[기믹 자동 적용] 현재 스테이지: {currentStage}");
        gimmickManager.TryApplyGimmick(currentStage);
    }
}