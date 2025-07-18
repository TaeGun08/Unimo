using System.Collections;
using UnityEngine;

// 피격 무효화 스킬
public class HitInvalidationSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public float passiveCooldown;
    private Coroutine passiveRegenRoutine;
    
    private void Start()
    {
        // 무효화 소진시마다 쿨타임 시작!
        LocalPlayer.Instance.PlayerStatHolder.OnOnceInvalidUsed += StartPassiveCooldown;
    }

    private void OnDestroy()
    {
        LocalPlayer.Instance.PlayerStatHolder.OnOnceInvalidUsed -= StartPassiveCooldown;
    }

    public void Excute(GameObject caster, EquipmentSkillType type, int duration, float param)
    {
        switch (type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 피격 무효화 패시브 발동");
                // 최초에는 1회 무효화 부여
                LocalPlayer.Instance.PlayerStatHolder.GiveOnceInvalid();
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] 무적 액티브 발동");
                break;
        }
    }
    
    private void StartPassiveCooldown()
    {
        if (passiveRegenRoutine != null)
        {
            StopCoroutine(passiveRegenRoutine);
        }
        passiveRegenRoutine = StartCoroutine(PassiveCooldownRoutine());
    }

    private IEnumerator PassiveCooldownRoutine()
    {
        yield return new WaitForSeconds(passiveCooldown);
        LocalPlayer.Instance.PlayerStatHolder.GiveOnceInvalid();
        passiveRegenRoutine = null;
    }
}
