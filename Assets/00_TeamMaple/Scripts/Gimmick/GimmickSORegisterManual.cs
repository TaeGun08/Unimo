using UnityEngine;

public class GimmickSORegisterManual : MonoBehaviour
{
    [Header("직접 등록할 StageGimmickSO 리스트")]
    public StageGimmickSO[] gimmickSOs;

    private void Awake()
    {
        foreach (var so in gimmickSOs)
        {
            if (so != null)
            {
                GimmickRegistry.Register(so.gimmickType, so);
                Debug.Log($"[기믹 수동 등록] {so.gimmickType}");
            }
        }
    }
}