using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "LightningStrikeGimmickSO", menuName = "StageGimmick/LightningStrike")]
public class LightningStrikeGimmickSO : StageGimmickSO
{
    public GameObject lightningPrefab;
    public float interval = 5f;

    public override GameObject Execute(Vector3 origin)
    {
        var runnerObj = new GameObject("LightningStrikeRunner");
        var runner = runnerObj.AddComponent<LightningStrikeRunner>();
        runner.Init(this, origin);
        return runnerObj;
    }
}

public class LightningStrikeRunner : MonoBehaviour
{
    private LightningStrikeGimmickSO data;
    private Coroutine routine;

    public void Init(LightningStrikeGimmickSO so, Vector3 origin)
    {
        data = so;
        routine = StartCoroutine(StrikeRoutine(origin));
    }

    private IEnumerator StrikeRoutine(Vector3 center)
    {
        while (true)
        {
            Vector3 pos = center + Random.insideUnitSphere * 5f;
            pos.y = 0f;
            Instantiate(data.lightningPrefab, pos, Quaternion.identity);
            yield return new WaitForSeconds(data.interval);
        }
    }

    private void OnDestroy()
    {
        if (routine != null) StopCoroutine(routine);
    }
}
