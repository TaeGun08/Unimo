using UnityEngine;

public class GimmickItem : MonoBehaviour
{
    private StageGimmickType gimmickType;
    private MonoBehaviour runner;

    private GameObject pickupEffect;
    private GameObject durationEffect;

    private float effectDuration = 30f;

    public void Init(StageGimmickType type, MonoBehaviour runner = null, GameObject pickup = null, GameObject duration = null, float effectDuration = 30f)
    {
        this.gimmickType = type;
        this.runner = runner;
        this.pickupEffect = pickup;
        this.durationEffect = duration;
        this.effectDuration = effectDuration;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어 감지
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;

        Transform playerTf = LocalPlayer.Instance.transform;

        // 러너가 이펙트를 직접 관리하는 기믹 목록
        bool managedByRunner =
            gimmickType == StageGimmickType.PoisonGas ||
            gimmickType == StageGimmickType.SlipperyFloor ||
            gimmickType == StageGimmickType.TimeSlow;

        // ✅ 공통 이펙트는 러너 "비관리형"에만 출력 (중복 생성 방지)
        if (!managedByRunner)
        {
            GimmickEffectHelper.PlayPickupEffect(pickupEffect, playerTf);

            // 필요 타입만 지속 이펙트 부착 (러너가 안 하는 것들만)
            if (gimmickType is StageGimmickType.LightningStrike or StageGimmickType.WildWind)
            {
                GimmickEffectHelper.AttachDurationEffect(durationEffect, playerTf, effectDuration);
            }
        }

        // ✅ 각 기믹 기능 호출
        switch (gimmickType)
        {
            case StageGimmickType.LightningStrike:
                LightningStrikeRunner.ApplyLightningImmune(10f);
                break;

            case StageGimmickType.PoisonGas:
                // 러너가 픽업/지속 이펙트, 타이머, 제거까지 “한 개만” 관리
                PoisonGasRunner.ApplyTemporaryGasClear(effectDuration);
                break;

            case StageGimmickType.SlipperyFloor:
                // 러너가 이펙트(1개), 면역, 재적용까지 관리
                SlipperyFloorRunner.Instance?.GrantSlipImmunity(effectDuration, playerTf);
                break;

            case StageGimmickType.MeteorFall:
                MeteorFallRunner.RemoveBurning();
                break;

            case StageGimmickType.Darkness:
                DarknessRunner.Instance?.Suppress();
                break;

            case StageGimmickType.WildWind:
                WildWindRunner.ApplyWindResist(20f);
                break;

            case StageGimmickType.FogDamage:
                FogDamageRunner.Instance?.TriggerSafeZoneExpansion();
                break;

            case StageGimmickType.Earthquake:
                EarthquakeRunner.RemoveSlow();
                break;

            case StageGimmickType.TimeSlow:
                // 러너가 즉시 해제 + 면역 + 이펙트(1개) 관리 (Earthquake 패턴)
                (runner as TimeSlowCycleRunner)?.GrantTimeSlowImmunity(effectDuration, playerTf);
                // 만약 runner 참조가 없는 경우라도 최소한 현재 슬로우는 해제
                if (runner == null) TimeSlowRunner.RemoveSlow();
                break;
        }

        Destroy(gameObject);
    }
}

public static class GimmickEffectHelper
{
    public static void PlayPickupEffect(GameObject prefab, Transform target)
    {
        if (prefab == null || target == null) return;

        GameObject vfx = Object.Instantiate(prefab, target.position + Vector3.up * 1.5f, Quaternion.identity);
        Object.Destroy(vfx, 2f);
    }

    public static void AttachDurationEffect(GameObject prefab, Transform target, float duration)
    {
        if (prefab == null || target == null) return;

        GameObject instance = Object.Instantiate(prefab, target);
        instance.transform.localPosition = new Vector3(0, 0.1f, 0);
        Object.Destroy(instance, duration);
    }
}
