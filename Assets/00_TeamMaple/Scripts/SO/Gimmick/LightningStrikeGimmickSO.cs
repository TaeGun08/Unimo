using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "LightningStrikeGimmickSO", menuName = "StageGimmick/LightningStrike")]
public class LightningStrikeGimmickSO : StageGimmickSO
{
    public GameObject markerPrefab;
    public GameObject lightningPrefab;
    public float markerGrowTime = 1.5f;
    public float interval = 5f;
    public float radius = 6f;

    public override GameObject Execute(Vector3 origin)
    {
        var runnerObj = new GameObject("LightningStrikeRunner");
        var runner = runnerObj.AddComponent<LightningStrikeRunner>();
        runner.Init(this, origin);
        return runnerObj;
    }
    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.LightningStrike, this);
    }

}

public class LightningStrikeRunner : MonoBehaviour
{
    private LightningStrikeGimmickSO data;
    private Vector3 center;
    private Coroutine routine;
    private bool strikeInProgress = false;

    public void Init(LightningStrikeGimmickSO so, Vector3 origin)
    {
        data = so;
        center = origin;
        routine = StartCoroutine(StrikeLoop());
    }

    private IEnumerator StrikeLoop()
    {
        while (true)
        {
            if (!strikeInProgress)
            {
                Vector3 pos = GetRandomGroundPosition();
                if (pos != Vector3.zero)
                {
                    StartCoroutine(ExecuteStrike(pos));
                }
            }
            yield return new WaitForSeconds(data.interval);
        }
    }

    private IEnumerator ExecuteStrike(Vector3 pos)
    {
        strikeInProgress = true;

        GameObject marker = Instantiate(data.markerPrefab, pos, Quaternion.identity);
        LightningStrikeMarker markerScript = marker.GetComponent<LightningStrikeMarker>();
        if (markerScript != null)
        {
            markerScript.Init(() =>
            {
                Instantiate(data.lightningPrefab, pos, Quaternion.identity);
                ApplyStun(pos);
            }, data.markerGrowTime);
        }

        yield return new WaitForSeconds(data.markerGrowTime + 0.5f);
        strikeInProgress = false;
    }

    private void ApplyStun(Vector3 center)
    {
        Collider[] hits = Physics.OverlapSphere(center, 1.5f);
        foreach (var hit in hits)
        {
            var stunnable = hit.GetComponent<IStunnable>();
            if (stunnable != null)
            {
                stunnable.Stun(2f);
            }
        }
    }

    private Vector3 GetRandomGroundPosition()
    {
        Vector3 random = center + new Vector3(
            UnityEngine.Random.Range(-data.radius, data.radius),
            0,
            UnityEngine.Random.Range(-data.radius, data.radius)
        );

        if (Physics.Raycast(random + Vector3.up * 20f, Vector3.down, out RaycastHit hit, 40f))
        {
            if (hit.collider.CompareTag("Ground"))
                return hit.point;
        }
        return Vector3.zero;
    }

    private void OnDestroy()
    {
        if (routine != null) StopCoroutine(routine);
    }

}