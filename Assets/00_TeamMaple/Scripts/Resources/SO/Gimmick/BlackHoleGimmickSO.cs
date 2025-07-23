using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlackHoleGimmickSO", menuName = "StageGimmick/BlackHole")]
public class BlackHoleGimmickSO : StageGimmickSO
{
    public GameObject blackHolePrefab;
    public float outerForce = 0.3f;
    public float innerForce = 0.4f;
    public float innerRadius = 1.5f;
    public float outerRadius = 6f;
    public float teleportRadius = 10f;
    public LayerMask groundLayer;
    public float spawnDelay = 10f;

    public override GameObject Execute(Vector3 origin)
    {
        var runnerObj = new GameObject("BlackHoleRunner");
        var runner = runnerObj.AddComponent<BlackHoleRunner>();
        runner.InitDelayed(this, origin, spawnDelay);
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
    private Transform blackHole;
    private Vector3 center;
    private HashSet<GameObject> innerHit = new();
    private bool activated = false;

    public void InitDelayed(BlackHoleGimmickSO so, Vector3 origin, float delay)
    {
        data = so;
        center = origin;
        StartCoroutine(DelayedSpawn(delay));
    }

    private IEnumerator DelayedSpawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        blackHole = Instantiate(data.blackHolePrefab, center, Quaternion.identity).transform;
        activated = true;
        Debug.Log("[기믹] 블랙홀 생성됨");
    }

    private void FixedUpdate()
    {
        if (!activated) return;

        foreach (var target in FindObjectsOfType<Rigidbody>())
        {
            if (!target.CompareTag("Player")) continue;

            Vector3 toCenter = center - target.position;
            float dist = toCenter.magnitude;

            if (dist > data.outerRadius) continue;

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
                float force = Mathf.Lerp(data.innerForce, data.outerForce, dist / data.outerRadius);
                Vector3 pullDir = toCenter.normalized;
                target.AddForce(pullDir * force, ForceMode.Acceleration);

                if (target.linearVelocity.magnitude > 10f)
                    target.linearVelocity = target.linearVelocity.normalized * 10f;
            }
        }
    }

    private IEnumerator SafeTeleportPlayer(Rigidbody target)
    {
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb == null) yield break;

        Debug.Log("[블랙홀] 텔레포트 시작");

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;

        Vector3 teleportPos = GetSafeTeleportPosition();
        Vector3 finalPos = teleportPos + Vector3.up * 2.0f;
        target.transform.position = finalPos;

        Physics.SyncTransforms();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        var recover = target.GetComponent<BlackHoleRecover>();
        if (recover != null)
            recover.TriggerFall();

        Debug.Log("[블랙홀] 안전 텔레포트 완료: " + finalPos);
    }

    public static void ResetHit(GameObject obj)
    {
        foreach (var runner in FindObjectsOfType<BlackHoleRunner>())
        {
            runner.innerHit.Remove(obj);
        }
    }

    private Vector3 GetSafeTeleportPosition()
    {
        const int maxTries = 20;
        float safeDistance = data.innerRadius + 5f;

        for (int i = 0; i < maxTries; i++)
        {
            Vector2 rand = Random.insideUnitCircle.normalized * data.teleportRadius;
            Vector3 offset = new Vector3(rand.x, 0f, rand.y);
            Vector3 rayStart = center + offset + Vector3.up * 20f;

            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 30f, data.groundLayer))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) > 20f) continue;
                if ((hit.point - center).magnitude < safeDistance) continue;

                Vector3 spawnPos = hit.point;
                if (!Physics.CheckSphere(spawnPos + Vector3.up * 0.5f, 0.4f, ~data.groundLayer))
                {
                    Debug.Log("[블랙홀] 안전한 텔레포트 위치 발견: " + spawnPos);
                    return spawnPos;
                }
            }
        }

        Vector3 fallbackPos = center + Vector3.forward * safeDistance;
        if (Physics.Raycast(fallbackPos + Vector3.up * 10f, Vector3.down, out RaycastHit fallbackHit, 20f, data.groundLayer))
        {
            fallbackPos = fallbackHit.point;
        }
        else
        {
            fallbackPos = center + Vector3.up * 3f;
        }

        Debug.LogWarning("[블랙홀] 기본 위치 사용: " + fallbackPos);
        return fallbackPos;
    }
}

public class BlackHoleRecover : MonoBehaviour
{
    private bool isFallingFromBlackhole = false;
    private Rigidbody rb;

    [SerializeField] private LayerMask groundMask;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void TriggerFall()
    {
        isFallingFromBlackhole = true;
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }
    }

    private void FixedUpdate()
    {
        if (!isFallingFromBlackhole) return;

        Vector3 rayStart = transform.position + Vector3.up * 0.5f;
        if (Physics.SphereCast(rayStart, 0.3f, Vector3.down, out RaycastHit hit, 1.0f, groundMask))
        {
            Debug.Log("[블랙홀] 착지 감지됨 (SphereCast)");

            isFallingFromBlackhole = false;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;

            Vector3 corrected = transform.position;
            corrected.y = hit.point.y + 0.05f;
            transform.position = corrected;

            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            BlackHoleRunner.ResetHit(gameObject);

            Debug.Log("[블랙홀] 착지 완료 및 재진입 가능 상태");
        }
    }
}

