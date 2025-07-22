using UnityEngine;

[CreateAssetMenu(fileName = "FogDamageGimmickSO", menuName = "StageGimmick/FogDamage")]
public class FogDamageGimmickSO : StageGimmickSO
{
    public GameObject fogVisualPrefab;

    public float duration = 20f;
    public float damageInterval = 1f;

    public float startRadius = 20f;
    public float endRadius = 2f;

    public override GameObject Execute(Vector3 origin)
    {
        var obj = new GameObject("FogDamageRunner");
        var runner = obj.AddComponent<FogDamageRunner>();
        runner.Init(this, origin);
        return obj;
    }
}

public class FogDamageRunner : MonoBehaviour
{
    private FogDamageGimmickSO data;
    private float timer;
    private float damageTimer;
    private float currentRadius;

    private GameObject fogFX;
    private Vector3 center;

    public void Init(FogDamageGimmickSO so, Vector3 origin)
    {
        data = so;
        center = origin;
        currentRadius = data.startRadius;

        if (data.fogVisualPrefab != null)
        {
            fogFX = Instantiate(data.fogVisualPrefab, center, Quaternion.identity);
            fogFX.transform.localScale = Vector3.one * currentRadius;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        damageTimer += Time.deltaTime;

        // ✅ 수축 반지름 계산
        float t = Mathf.Clamp01(timer / data.duration);
        currentRadius = Mathf.Lerp(data.startRadius, data.endRadius, t);

        if (fogFX != null)
            fogFX.transform.localScale = Vector3.one * currentRadius;

        // ✅ 범위 내 대상에 데미지
        if (damageTimer >= data.damageInterval)
        {
            Collider[] hits = Physics.OverlapSphere(center, currentRadius);
            foreach (var hit in hits)
            {
                var damageable = hit.GetComponent<IDamageAble>();
                if (damageable != null)
                    damageable.TakeDamage(center);
            }

            damageTimer = 0f;
        }

        // ✅ 기믹 종료
        if (timer >= data.duration)
        {
            if (fogFX != null) Destroy(fogFX);
            Destroy(gameObject);
        }
    }
}