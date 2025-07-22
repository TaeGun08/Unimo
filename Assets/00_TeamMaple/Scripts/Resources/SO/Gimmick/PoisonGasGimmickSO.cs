// PoisonGasGimmickSO.cs
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "PoisonGasGimmickSO", menuName = "StageGimmick/PoisonGas")]
public class PoisonGasGimmickSO : StageGimmickSO
{
    public GameObject gasPrefab;
    public float gasDuration = 20f;
    public float interval = 30f;
    public float spawnRadius = 8f;
    public float scaleDuration = 2f;
    public float targetScale = 6f;

    public override GameObject Execute(Vector3 origin)
    {
        Debug.Log("[PoisonGas] 기믹 실행됨");

        PoisonGasRunner runner = new GameObject("PoisonGasRunner").AddComponent<PoisonGasRunner>();
        runner.Init(this, origin);
        return runner.gameObject;
    }
    
    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.PoisonGas, this);
    }
}

public class PoisonGasRunner : MonoBehaviour
{
    private PoisonGasGimmickSO data;
    private Vector3 center;
    private Coroutine routine;

    public void Init(PoisonGasGimmickSO so, Vector3 origin)
    {
        data = so;
        center = origin;
        routine = StartCoroutine(GasRoutine());
    }

    private IEnumerator GasRoutine()
    {
        while (true)
        {
            Vector3 randomPos = center + new Vector3(
                Random.Range(-data.spawnRadius, data.spawnRadius),
                0,
                Random.Range(-data.spawnRadius, data.spawnRadius)
            );

            GameObject gas = Instantiate(data.gasPrefab, randomPos, Quaternion.identity);
            gas.transform.localScale = Vector3.zero;

            PoisonArea area = gas.GetComponent<PoisonArea>();
            if (area != null)
            {
                area.scaleDuration = data.scaleDuration;
                area.targetScale = data.targetScale;
            }

            Destroy(gas, data.gasDuration);
            yield return new WaitForSeconds(data.interval);
        }
    }

    private void OnDestroy()
    {
        if (routine != null) StopCoroutine(routine);
    }
}