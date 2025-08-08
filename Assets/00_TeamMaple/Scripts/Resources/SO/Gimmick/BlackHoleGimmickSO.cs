using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlackHoleGimmickSO", menuName = "StageGimmick/BlackHole")]
public class BlackHoleGimmickSO : StageGimmickSO
{
    public GameObject blackHolePrefab;
    public float outerRadius = 6f;
    public float innerRadius = 1.5f;
    public float teleportRadius = 10f;
    public LayerMask groundLayer;
    public float spawnDelay = 10f;

    public override GameObject Execute(Vector3 origin)
    {
        var runnerObj = new GameObject("BlackHoleRunner");
        var runner = runnerObj.AddComponent<BlackHoleRunner>();
        runner.Init(this, origin);
        return runnerObj;
    }

    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.BlackHole, this);
    }
}

public class BlackHoleRunner : MonoBehaviour
{
    private BlackHoleGimmickSO data;
    private Vector3 center;
    private bool activated = false;
    private readonly HashSet<GameObject> innerHit = new();

    public void Init(BlackHoleGimmickSO so, Vector3 origin)
    {
        data = so;
        center = origin;
        StartCoroutine(SpawnBlackHole());
    }

    private IEnumerator SpawnBlackHole()
    {
        yield return new WaitForSeconds(data.spawnDelay);
        Instantiate(data.blackHolePrefab, center, Quaternion.identity);
        activated = true;
    }

    private void FixedUpdate()
    {
        if (!activated) return;

        foreach (var target in FindObjectsOfType<Rigidbody>())
        {
            if (!target.CompareTag("Player")) continue;

            Vector3 toCenter = center - target.position;
            float dist = toCenter.magnitude;

            if (dist > data.outerRadius)
            {
                target.linearVelocity = Vector3.zero;
                continue;
            }

            if (dist < data.innerRadius)
            {
                if (!innerHit.Contains(target.gameObject))
                {
                    innerHit.Add(target.gameObject);
                    StartCoroutine(SafeTeleportPlayer(target));
                }
            }
            else
            {
                Vector3 pullDir = toCenter.normalized;
                target.linearVelocity = pullDir * 1f;
            }
        }
    }

    private IEnumerator SafeTeleportPlayer(Rigidbody rb)
    {
        yield return new WaitForSeconds(0.25f);

        Vector3 offset = new Vector3(
            Random.Range(-data.teleportRadius, data.teleportRadius),
            10f,
            Random.Range(-data.teleportRadius, data.teleportRadius)
        );

        Vector3 newPos = center + offset;

        if (Physics.Raycast(newPos, Vector3.down, out RaycastHit hit, 30f, data.groundLayer))
        {
            // ✅ 위치 이동
            rb.position = hit.point + Vector3.up * 0.1f;

            // ✅ 속도 제거
            rb.linearVelocity = Vector3.zero;
            rb.linearVelocity = Vector3.zero;

            // ✅ 중력 재활성화
            rb.useGravity = true;

            // ✅ 데미지 처리
            var stat = LocalPlayer.Instance?.PlayerStatHolder;

            if (stat != null && !stat.HasInvincible)
            {
                if (stat.HasOnceInvalid)
                {
                    stat.OnInvalidation();
                    Debug.Log("[블랙홀] 1회 무효화 소모됨");
                }
                else
                {
                    int maxHp = stat.Hp.MaxValue;
                    int damage = Mathf.CeilToInt(maxHp * 0.1f);
                    stat.Hp.Subtract(damage);
                    Debug.Log($"[블랙홀] HP 10% 피해 적용됨: -{damage} (남은 HP: {stat.Hp.Value})");
                }
            }
            else
            {
                Debug.Log("[블랙홀] 무적 상태 - 피해 없음");
            }

            // ✅ 중력 상태 복구 루틴
            rb.GetComponent<BlackHoleRecover>()?.TriggerFall();

            // ✅ 다시 진입 가능하도록 해제
            innerHit.Remove(rb.gameObject);
        }
    }

    public static void ResetHit(GameObject obj)
    {
        if (obj == null) return;

        if (obj.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
}
