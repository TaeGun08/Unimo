using UnityEngine;

public class GimmickItem : MonoBehaviour
{
    private StageGimmickType gimmickType;
    private MonoBehaviour runner;

    private GameObject pickupEffect;
    private GameObject durationEffect;

    private const float defaultDuration = 30f;
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
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;

        Debug.Log("[GimmickItem] 플레이어가 아이템을 획득함");

        Transform playerTf = LocalPlayer.Instance.transform;

        // ✅ 공통 이펙트 출력
        GimmickEffectHelper.PlayPickupEffect(pickupEffect, playerTf);

        if (gimmickType is StageGimmickType.LightningStrike or
            StageGimmickType.PoisonGas or
            StageGimmickType.SlipperyFloor or
            StageGimmickType.WildWind or
            StageGimmickType.TimeSlow)
        {
            GimmickEffectHelper.AttachDurationEffect(durationEffect, playerTf, effectDuration);
        }


        // ✅ 각 기믹 기능 호출
        switch (gimmickType)
        {
            case StageGimmickType.LightningStrike:
                LightningStrikeRunner.ApplyLightningImmune(10f);
                break;
            case StageGimmickType.PoisonGas:
                PoisonGasRunner.ApplyTemporaryGasClear(30f);
                break;
            case StageGimmickType.SlipperyFloor:
                SlipperyFloorRunner.Instance?.GrantSlipImmunity(30f,playerTf);
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
                TimeSlowCycleRunner.ApplyTimeSlowImmune(10f);
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
