using UnityEngine;

public class FogDamageZone : MonoBehaviour
{
    private Transform player;
    private FogDamageGimmickSO config;
    private float dotTimer;

    public void Init(Transform target, FogDamageGimmickSO so)
    {
        player = target;
        config = so;
    }

    private void Update()
    {
        if (player == null || config == null) return;

        float radius = transform.localScale.x / 2f;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < radius)
        {
            dotTimer += Time.deltaTime;
            if (dotTimer >= config.tickInterval)
            {
                dotTimer = 0f;
                ApplyFogDamage(); // ✅ 데미지 적용
            }
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

        Vector3 knockbackDir = (LocalPlayer.Instance.transform.position - transform.position).normalized;

        var combat = new CombatEvent
        {
            Damage = damage,
            Position = transform.position,
            KnockbackDir = knockbackDir
        };

        LocalPlayer.Instance.CombatEvent = combat;
        LocalPlayer.Instance.playerController.ChangeState(IPlayerState.EState.Hit);

        Debug.Log($"[FogZone] {damage} 피해 및 넉백 적용됨");
    }
}