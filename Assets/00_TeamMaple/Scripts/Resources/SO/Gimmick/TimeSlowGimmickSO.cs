using UnityEngine;

[CreateAssetMenu(fileName = "TimeSlowGimmickSO", menuName = "StageGimmick/TimeSlow")]
public class TimeSlowGimmickSO : StageGimmickSO
{
    public float slowTimeScale = 0.5f;
    public float fastTimeScale = 1.5f;

    public float slowDuration = 5f;
    public float fastDuration = 5f;
    public float normalDuration = 3f;

    public override GameObject Execute(Vector3 origin)
    {
        var obj = new GameObject("TimeSlowCycleRunner");
        var runner = obj.AddComponent<TimeSlowCycleRunner>();
        runner.so = this;
        runner.Init();
        return obj;
    }

    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.TimeSlow, this);
    }
}

public class TimeSlowCycleRunner : MonoBehaviour
{
    public TimeSlowGimmickSO so;

    private float timer = 0f;
    private int stateIndex = 0;

    private float[] timeScales;
    private float[] durations;

    private float originalTimeScale;
    private float originalFixedDelta;

    public void Init()
    {
        originalTimeScale = Time.timeScale;
        originalFixedDelta = Time.fixedDeltaTime;

        timeScales = new float[]
        {
            so.slowTimeScale,    // 느림
            1f,                  // 정상
            so.fastTimeScale,    // 빠름
            1f                   // 정상
        };

        durations = new float[]
        {
            so.slowDuration,
            so.normalDuration,
            so.fastDuration,
            so.normalDuration
        };

        ApplyState(0);
    }

    private void Update()
    {
        timer += Time.unscaledDeltaTime;

        if (timer >= durations[stateIndex])
        {
            timer = 0f;
            stateIndex = (stateIndex + 1) % timeScales.Length;
            ApplyState(stateIndex);
        }
    }

    private void ApplyState(int index)
    {
        float scale = timeScales[index];
        Time.timeScale = scale;
        Time.fixedDeltaTime = 0.02f * scale;

        Debug.Log($"[Time] 상태 {index}: 배속 {scale}");
    }

    private void OnDestroy()
    {
        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = originalFixedDelta;
    }
}
