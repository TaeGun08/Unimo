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

            Vector3 dir = (center - target.position);
            float dist = dir.magnitude;

            // ✅ 범위 밖이면 무시
            if (dist > data.outerRadius) continue;

            if (dist < data.innerRadius)
            {
                if (!innerHit.Contains(target.gameObject))
                {
                    innerHit.Add(target.gameObject);

                    // 데미지
                    var damageable = target.GetComponent<IDamageAble>();
                    CombatEvent combatEvent = new();
                    combatEvent.Sender = null;
                    combatEvent.Receiver = damageable;
                    combatEvent.Damage = 10;
                    combatEvent.Position = center;
                    CombatSystem.Instance.AddInGameEvent(combatEvent);

                    // 튕겨냄
                    Vector3 bounceDir = (target.position - center).normalized;
                    target.AddForce(bounceDir * 5f, ForceMode.Impulse);

                    Debug.Log("[블랙홀] 내부 도달 → 튕김 + 데미지");
                }
            }
            else
            {
                // ✅ 흡입 force 적용
                float force = Mathf.Lerp(data.innerForce, data.outerForce, dist / data.outerRadius);
                Vector3 pullDir = dir.normalized;

                target.AddForce(pullDir * force, ForceMode.Acceleration);

                // ✅ 속도 제한
                if (target.linearVelocity.magnitude > 10f)
                    target.linearVelocity = target.linearVelocity.normalized * 10f;
            }
        }
    }

}
