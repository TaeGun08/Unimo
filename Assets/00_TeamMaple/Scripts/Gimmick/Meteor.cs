using System.Collections;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    [Header("Falling")]
    public float fallSpeed = 20f;
    public Vector3 fallDirection = new Vector3(0.5f, -1, 0);

    [Header("Damage Settings")]
    public float destroyDelay = 0.2f;
    public float damageRadius = 3f; // ✅ 충분히 넓게 조정
    public LayerMask playerLayer;

    [Header("DOT Settings")]
    public float dotDuration = 30f;
    public float dotInterval = 1f;
    public float dotPercent = 0.3f;

    [Header("Effect")]
    public GameObject explosionEffect;

    private bool hasExploded = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = fallDirection.normalized * fallSpeed;
        }

        int meteorLayer = LayerMask.NameToLayer("Meteor");
        if (gameObject.layer != meteorLayer)
        {
            gameObject.layer = meteorLayer;
        }

        Physics.IgnoreLayerCollision(meteorLayer, meteorLayer, true);

        if (explosionEffect != null)
            explosionEffect.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasExploded) return;
        hasExploded = true;

        // ✅ 폭발 이펙트 재생
        if (explosionEffect != null)
        {
            explosionEffect.SetActive(true);
            var ps = explosionEffect.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
        }

        // ✅ Rigidbody 멈춤
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // ✅ 모든 플레이어에 대해 범위 체크 후 처리
        // ✅ 모든 플레이어에 대해 범위 체크 후 처리
        Collider[] targets = Physics.OverlapSphere(transform.position, damageRadius, playerLayer);
        foreach (var target in targets)
        {
            if (!target.CompareTag("Player")) continue;

            var statHolder = LocalPlayer.Instance.PlayerStatHolder;

            // ✅ 무적 상태일 경우 DOT도 적용하지 않음
            if (LocalPlayer.Instance.IsInvincible || statHolder.HasInvincible || statHolder.HasOnceInvalid)
            {
                Debug.Log("[메테오] 무적 상태라 DOT/넉백 전부 무시");
                
                if (statHolder.HasOnceInvalid)
                {
                    statHolder.OnInvalidation(); // ✅ 1회 무효화 처리 소모
                    Debug.Log("[메테오] 1회 무적 효과 소모됨");
                }
                
                break;
            }

            // ✅ DOT 처리
            ApplyDamageOverTime();

            // ✅ 넉백 방향 계산 (Y 제거)
            Vector3 knockbackDir = LocalPlayer.Instance.transform.position - transform.position;
            knockbackDir.y = 0f;

            var combat = new CombatEvent
            {
                Damage = 1, // 혹은 0
                Position = transform.position,
                KnockbackDir = transform.position
            };

            LocalPlayer.Instance.CombatEvent = combat;
            LocalPlayer.Instance.playerController.ChangeState(IPlayerState.EState.Hit);

            Debug.Log("[메테오] DOT + 넉백 적용 완료");
            break; // 단일 플레이어 대상이므로 1회 처리
        }


        // ✅ Runner에 파괴 알림
        var runner = FindObjectOfType<MeteorFallRunner>();
        if (runner != null)
            runner.NotifyMeteorDestroyed(gameObject);

        Destroy(gameObject, destroyDelay);
    }

    private void ApplyDamageOverTime()
    {
        var statHolder = LocalPlayer.Instance.PlayerStatHolder;

        int maxHp = statHolder.Hp.MaxValue;
        float totalDamage = maxHp * dotPercent;
        int tickCount = Mathf.CeilToInt(dotDuration / dotInterval);
        float damagePerTick = totalDamage / tickCount;

        Debug.Log($"[메테오 DOT] {tickCount}회 동안 {damagePerTick}씩 피해");

        if (LocalPlayer.Instance.ActiveDotCoroutine != null)
            LocalPlayer.Instance.StopCoroutine(LocalPlayer.Instance.ActiveDotCoroutine);

        LocalPlayer.Instance.ActiveDotCoroutine = LocalPlayer.Instance.StartCoroutine(
            ApplyDOT(damagePerTick, tickCount, dotInterval)
        );
    }

    private IEnumerator ApplyDOT(float damagePerTick, int tickCount, float interval)
    {
        var statHolder = LocalPlayer.Instance.PlayerStatHolder;

        for (int i = 0; i < tickCount; i++)
        {
            if (LocalPlayer.Instance.IsInvincible || statHolder.HasInvincible || statHolder.HasOnceInvalid)
            {
                Debug.Log("[DOT] 무적 상태로 인해 데미지 무시됨");
            }
            else
            {
                statHolder.Hp.Subtract(Mathf.RoundToInt(damagePerTick));
                Debug.Log($"[DOT] 틱 {i + 1}/{tickCount} - {Mathf.RoundToInt(damagePerTick)} 피해 (남은 HP: {statHolder.Hp.Value})");

                if (statHolder.Hp.Value <= 0)
                {
                    LocalPlayer.Instance.playerController.ChangeState(IPlayerState.EState.Dead);
                    LocalPlayer.Instance.ActiveDotCoroutine = null;
                    yield break;
                }
            }

            yield return new WaitForSeconds(interval);
        }


        LocalPlayer.Instance.ActiveDotCoroutine = null;
    }

}
