using UnityEngine;

public class GimmickItem : MonoBehaviour
{
    private StageGimmickType gimmickType;
    private MonoBehaviour runner;

    public void Init(StageGimmickType type, MonoBehaviour runner = null)
    {
        this.gimmickType = type;
        this.runner = runner;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("플레이어가 아이템 먹음");
            var stat = LocalPlayer.Instance.PlayerStatHolder;

            switch (gimmickType)
            {
                case StageGimmickType.LightningStrike:
                    if (runner is LightningStrikeRunner lightning)
                        lightning.GrantLightningImmunity(10f);
                    break;

                case StageGimmickType.PoisonGas:
                    PoisonGasRunner.RemoveNearbyGas();
                    break;

                case StageGimmickType.SlipperyFloor:
                    SlipperyFloorRunner.ApplySlipImmunity(30f);
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
}
