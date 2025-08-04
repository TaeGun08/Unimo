using UnityEngine;

[CreateAssetMenu(fileName = "FogDamageGimmickSO", menuName = "StageGimmick/FogDamage")]
public class FogDamageGimmickSO : StageGimmickSO
{
    public float duration = 15f;
    public float damageInterval = 2f;
    public float damageAmount = 5f;
    public Material fogMaterial;

    public override GameObject Execute(Vector3 origin)
    {
        var obj = new GameObject("FogDamageRunner");
        obj.transform.position = origin;

        var runner = obj.AddComponent<FogDamageRunner>();
        runner.fogMaterial = fogMaterial;
        runner.Init(this);
        return obj;
    }
    
    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.FogDamage, this);
    }
}

public class FogDamageRunner : MonoBehaviour
{
    private FogDamageGimmickSO data;
    private float timer;
    private float damageTimer;

    public float maxRadius = 30f;
    public float minRadius = 3f;
    private float currentRadius;

    private GameObject fogVisual;

    public Material fogMaterial; // ✅ 반투명한 Shader Graph 머티리얼 연결

    public void Init(FogDamageGimmickSO so)
    {
        data = so;
        timer = 0f;
        damageTimer = 0f;

        // ✅ 비주얼용 안개 Sphere 생성
        fogVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        fogVisual.transform.position = Vector3.zero;
        fogVisual.transform.localScale = Vector3.one * maxRadius * 2f;
        fogVisual.GetComponent<Collider>().enabled = false;
        fogVisual.GetComponent<MeshRenderer>().material = fogMaterial;

        currentRadius = maxRadius;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        damageTimer += Time.deltaTime;

        // ✅ 반지름 조이기
        float t = Mathf.Clamp01(timer / data.duration);
        currentRadius = Mathf.Lerp(maxRadius, minRadius, t);
        fogVisual.transform.localScale = Vector3.one * currentRadius * 2f;

        // ✅ 데미지 처리
        if (damageTimer >= data.damageInterval)
        {
            foreach (var player in FindObjectsOfType<LocalPlayer>())
            {
                float dist = Vector3.Distance(player.transform.position, transform.position);
                if (dist > currentRadius)
                {
                    var evt = new CombatEvent
                    {
                        Receiver = player,
                        Damage = (int)data.damageAmount
                    };
                    CombatSystem.Instance.AddInGameEvent(evt);
                }
            }
            damageTimer = 0f;
        }

        if (timer > data.duration)
        {
            Destroy(fogVisual);
            Destroy(gameObject);
        }
    }
}