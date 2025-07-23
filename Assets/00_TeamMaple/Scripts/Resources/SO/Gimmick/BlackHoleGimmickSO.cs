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

    public float teleportRadius = 10f;           // 텔레포트 범위
    public LayerMask groundLayer;                // 지면 판별용 레이어

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
    private Transform blackHole;
    private Vector3 center;
    private HashSet<GameObject> innerHit = new();

    public void Init(BlackHoleGimmickSO so, Vector3 origin)
    {
        data = so;
        center = origin;
        blackHole = Instantiate(data.blackHolePrefab, center, Quaternion.identity).transform;
    }

    private void FixedUpdate()
    {
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

                    var damageable = target.GetComponent<IDamageAble>();

                    int hpPercentDamage = 0;
                    if (damageable != null)
                    {
                        var health = target.GetComponent<IDamageAble>();
                        if (health != null)
                        {
                            //float currentHP = health.CurrentHP;
                            //hpPercentDamage = Mathf.CeilToInt(currentHP * 0.1f);
                        }
                    }

                    var combatEvent = new CombatEvent
                    {
                        Sender = null,
                        Receiver = damageable,
                        Damage = hpPercentDamage,
                        Position = center
                    };
                    CombatSystem.Instance.AddInGameEvent(combatEvent);

                    // ✅ 블랙홀 중심 진입 → 공중 안전 지역으로 텔레포트 (중력 적용)
                    Vector3 teleportPos = GetRandomTeleportPosition();

                    Rigidbody rb = target.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.linearVelocity = Vector3.zero;
                        rb.MovePosition(teleportPos);
                        rb.linearVelocity = new Vector3(0, -5f, 0); // 아래로 낙하 유도
                    }
                    else
                    {
                        target.position = teleportPos;
                    }

                    Debug.Log("[블랙홀] 중심 진입 → 텔레포트 위치로 이동 (체력 10% 감소)");
                }
            }
            else
            {
                // 흡입력 계산
                float force = Mathf.Lerp(data.innerForce, data.outerForce, dist / data.outerRadius);
                Vector3 pullDir = toCenter.normalized;
                target.AddForce(pullDir * force, ForceMode.Acceleration);

                // 속도 제한
                if (target.linearVelocity.magnitude > 10f)
                    target.linearVelocity = target.linearVelocity.normalized * 10f;
            }
        }
    }

    private Vector3 GetRandomTeleportPosition()
    {
        const int maxTries = 10;
        float safeDistance = data.innerRadius + 2f;

        for (int i = 0; i < maxTries; i++)
        {
            Vector2 rand = Random.insideUnitCircle.normalized * data.teleportRadius;
            Vector3 offset = new Vector3(rand.x, 0f, rand.y);

            if (offset.magnitude < safeDistance)
                continue;

            Vector3 tryPos = center + offset + Vector3.up * 10f;

            if (Physics.Raycast(tryPos, Vector3.down, out RaycastHit hit, 20f, data.groundLayer))
            {
                return hit.point + Vector3.up * 15f; // 공중 리스폰 연출
            }
        }

        return center + (Vector3.forward * safeDistance) + Vector3.up * 15f;
    }
}
