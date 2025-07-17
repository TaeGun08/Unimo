using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "WildWindGimmickSO", menuName = "StageGimmick/WildWind")]
public class WildWindGimmickSO : StageGimmickSO
{
    public float delayBeforeStart = 30f; //기믹 시작 시간
    public float windDuration = 20f; // 기믹 지속 시간
    public Vector3 windDirection = Vector3.left; // 바람 방향
    public AnimationCurve decayCurve = AnimationCurve.Linear(0, 1f, 1, 0.6f); // 효과 점차 감소

    public override GameObject Execute(Vector3 origin)
    {
        var runnerObj = new GameObject("WildWindRunner");
        var runner = runnerObj.AddComponent<WildWindRunner>();
        runner.Init(this);
        return runnerObj;
    }
    
    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.WildWind, this);
    }
}

public class WildWindRunner : MonoBehaviour
{
    private WildWindGimmickSO data;
    public static Vector3 CurrentWindDirection { get; private set; } = Vector3.zero;

    public void Init(WildWindGimmickSO so)
    {
        data = so;
        StartCoroutine(WindRoutine());
    }

    private IEnumerator WindRoutine()
    {
        yield return new WaitForSeconds(data.delayBeforeStart);
        
        CurrentWindDirection = data.windDirection.normalized;
        Debug.Log($"[WildWind] 바람 시작: {CurrentWindDirection}");

        float timer = 0f;

        while (timer < data.windDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        CurrentWindDirection = Vector3.zero;
        Debug.Log("[WildWind] 바람 종료");

        Destroy(gameObject);
    }
}