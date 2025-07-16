using UnityEngine;

public class StageAutoGimmickStarter : MonoBehaviour
{
    public StageGimmickManager gimmickManager;
    public int stageNum = 1; // 여기만 변경해서 테스트 가능

    private void Start()
    {
        if (gimmickManager != null)
        {
            gimmickManager.TryApplyGimmick(stageNum);
        }
    }
}