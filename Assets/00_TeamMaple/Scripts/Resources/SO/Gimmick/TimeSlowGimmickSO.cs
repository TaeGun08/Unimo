using UnityEngine;

[CreateAssetMenu(fileName = "TimeSlowGimmickSO", menuName = "StageGimmick/TimeSlow")]
public class TimeSlowGimmickSO : StageGimmickSO
{
    public float slowTimeScale = 0.5f;
    public float duration = 5f;

    public override GameObject Execute(Vector3 origin)
    {
        var obj = new GameObject("TimeSlowRunner");
        var runner = obj.AddComponent<TimeSlowRunner>();
        runner.Init(this);
        return obj;
    }
    
    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.TimeSlow, this);
    }
}

public class TimeSlowRunner : MonoBehaviour
{
    private float timer;
    private float duration;
    private float originalTimeScale;
    private float originalFixedDelta;

    public void Init(TimeSlowGimmickSO so)
    {
        duration = so.duration;

        // ✅ 기존 값 저장
        originalTimeScale = Time.timeScale;
        originalFixedDelta = Time.fixedDeltaTime;

        // ✅ 느려짐 적용
        Time.timeScale = so.slowTimeScale;
        Time.fixedDeltaTime = 0.02f * so.slowTimeScale;

        Debug.Log($"[TimeSlow] 시간 느려짐: {Time.timeScale}");
    }

    private void Update()
    {
        // ⚠️ unscaledDeltaTime 사용해야 정확하게 유지됨
        timer += Time.unscaledDeltaTime;

        if (timer > duration)
        {
            // ✅ 원복
            Time.timeScale = originalTimeScale;
            Time.fixedDeltaTime = originalFixedDelta;

            Debug.Log("[TimeSlow] 시간 복구됨");
            Destroy(gameObject);
        }
    }
}

