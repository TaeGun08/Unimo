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

            if (dist < data.innerRadius)
            {
                if (!innerHit.Contains(target.gameObject))
                {
                    innerHit.Add(target.gameObject);
                    // HP 10% 감소 + 튕겨내기
                    var damageable = target.GetComponent<IDamageAble>();
                    damageable?.TakeDamage(center);

                    Vector3 bounceDir = (target.position - center).normalized;
                    target.AddForce(bounceDir * 5f, ForceMode.Impulse);
                }
            }
            else if (dist < data.outerRadius)
            {
                float force = Mathf.Lerp(data.innerForce, data.outerForce, dist / data.outerRadius);
                target.AddForce(dir.normalized * force, ForceMode.Acceleration);
            }
        }
    }
}
