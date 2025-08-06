using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "FogDamageGimmickSO", menuName = "StageGimmick/FogDamage")]
public class FogDamageGimmickSO : StageGimmickSO
{
    [Header("Prefabs")]
    public GameObject fogPrefab;
    public GameObject safeZonePrefab;

    [Header("Damage")]
    public float tickInterval = 1f;
    public float damagePercent = 0.01f;

    [Header("Safe Zone")]
    public float initialSafeRadius = 8f;
    public float finalSafeRadius = 2f;
    public float shrinkDuration = 30f;

    public override GameObject Execute(Vector3 origin)
    {
        var runnerObj = new GameObject("FogDamageRunner");
        var runner = runnerObj.AddComponent<FogDamageRunner>();
        runner.Init(this, origin);
        return runnerObj;
    }

    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.FogDamage, this);
    }
}

public class FogDamageRunner : MonoBehaviour
{
    private FogDamageGimmickSO config;
    private Transform player;
    private Transform safeZone;

    private float shrinkTimer;
    private float dotTimer;

    public void Init(FogDamageGimmickSO so, Vector3 origin)
    {
        config = so;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (config.fogPrefab != null)
            Instantiate(config.fogPrefab, origin, Quaternion.identity);

        if (config.safeZonePrefab != null)
        {
            var zone = Instantiate(config.safeZonePrefab, origin, Quaternion.identity);
            safeZone = zone.transform;
            safeZone.localScale = Vector3.one * config.initialSafeRadius * 2f;
        }
    }

    private void Update()
    {
        if (player == null || safeZone == null) return;

        // SafeZone 축소
        shrinkTimer += Time.deltaTime;
        float t = Mathf.Clamp01(shrinkTimer / config.shrinkDuration);
        float radius = Mathf.Lerp(config.initialSafeRadius, config.finalSafeRadius, t);
        safeZone.localScale = Vector3.one * radius * 2f;

        // 플레이어 거리 확인
        float distance = Vector3.Distance(player.position, safeZone.position);
        if (distance > radius)
        {
            dotTimer += Time.deltaTime;
            if (dotTimer >= config.tickInterval)
            {
                ApplyFogDamage();
                dotTimer = 0f;
            }
        }
        else
        {
            dotTimer = 0f; // 안에 있으면 타이머 초기화
        }
    }

    private void ApplyFogDamage()
    {
        var statHolder = LocalPlayer.Instance.PlayerStatHolder;

        if (LocalPlayer.Instance.IsInvincible || statHolder.HasInvincible || statHolder.HasOnceInvalid)
        {
            Debug.Log("[FogZone] 무적 상태 → 피해 무효");

            if (statHolder.HasOnceInvalid)
            {
                statHolder.OnInvalidation();
                Debug.Log("[FogZone] 1회 무효화 소모");
            }

            return;
        }

        int maxHp = statHolder.Hp.MaxValue;
        int damage = Mathf.CeilToInt(maxHp * config.damagePercent);

        Vector3 knockbackDir = (player.position - safeZone.position).normalized;

        var combat = new CombatEvent
        {
            Damage = damage,
            Position = safeZone.position,
            KnockbackDir = knockbackDir
        };

        LocalPlayer.Instance.CombatEvent = combat;
        LocalPlayer.Instance.playerController.ChangeState(IPlayerState.EState.Hit);

        Debug.Log($"[FogZone] {damage} 피해 및 넉백 적용됨");
    }
}
