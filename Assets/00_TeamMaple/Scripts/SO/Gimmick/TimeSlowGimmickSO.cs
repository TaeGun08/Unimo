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
}

public class TimeSlowRunner : MonoBehaviour
{
    private float timer;
    private float duration;
    private float originalTimeScale;

    public void Init(TimeSlowGimmickSO so)
    {
        duration = so.duration;
        originalTimeScale = Time.timeScale;
        Time.timeScale = so.slowTimeScale;
    }

    private void Update()
    {
        timer += Time.unscaledDeltaTime;
        if (timer > duration)
        {
            Time.timeScale = originalTimeScale;
            Destroy(gameObject);
        }
    }
}

