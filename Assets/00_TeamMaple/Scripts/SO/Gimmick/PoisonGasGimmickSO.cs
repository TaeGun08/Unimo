using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "PoisonGasGimmickSO", menuName = "StageGimmick/PoisonGas")]
public class PoisonGasGimmickSO : StageGimmickSO
{
    public GameObject gasPrefab;
    public float gasDuration = 20f;
    public float interval = 30f;
    public float spawnRadius = 8f;

    public override GameObject Execute(Vector3 origin)
    {
        Debug.Log("[PoisonGas] 기믹 실행됨");

        PoisonGasRunner runner = new GameObject("PoisonGasRunner").AddComponent<PoisonGasRunner>();
        runner.Init(this, origin);
        return runner.gameObject;
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
        float elapsed = 0f;
        while (true)
        {
            Vector3 randomPos = center + new Vector3(
                Random.Range(-data.spawnRadius, data.spawnRadius),
                0,
                Random.Range(-data.spawnRadius, data.spawnRadius)
            );

            GameObject gas = Instantiate(data.gasPrefab, randomPos, Quaternion.identity);
            Destroy(gas, data.gasDuration);

            yield return new WaitForSeconds(data.interval);
        }
    }

    private void OnDestroy()
    {
        if (routine != null) StopCoroutine(routine);
    }
}