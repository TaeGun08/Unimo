using UnityEngine;

[CreateAssetMenu(fileName = "FogDamageGimmickSO", menuName = "StageGimmick/FogDamage")]
public class FogDamageGimmickSO : StageGimmickSO
{
    public float duration = 15f;
    public float damageInterval = 2f;
    public float damageAmount = 5f;

    public override GameObject Execute(Vector3 origin)
    {
        var obj = new GameObject("FogDamageRunner");
        var runner = obj.AddComponent<FogDamageRunner>();
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

    public void Init(FogDamageGimmickSO so)
    {
        data = so;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        damageTimer += Time.deltaTime;

        if (damageTimer >= data.damageInterval)
        {
            foreach (var player in FindObjectsOfType<LocalPlayer>()) // 가정: PlayerHealth 존재
            {
                Vector3 randomDirection = Random.onUnitSphere;
                player.TakeDamage(randomDirection);
            }
            damageTimer = 0f;
        }

        if (timer > data.duration)
        {
            Destroy(gameObject);
        }
    }
}

