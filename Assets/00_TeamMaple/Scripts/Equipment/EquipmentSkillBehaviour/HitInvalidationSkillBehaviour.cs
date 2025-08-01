using System.Collections;
using UnityEngine;

// 피격 무효화 스킬
public class HitInvalidationSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private float passiveCooldown;
    
    private Coroutine passiveCoroutine;
    private Coroutine activeCoroutine;
    private PlayerStatHolder statHolder;
    private EquipmentSkillManager skillManager;
    
    private void Awake()
    {
        statHolder = LocalPlayer.Instance.PlayerStatHolder;
        skillManager = EquipmentSkillManager.Instance;
    }

    private void OnDestroy()
    {
        if (passiveCoroutine != null)
        {
            StopCoroutine(passiveCoroutine);
        }
        statHolder.OnOnceInvalidUsed -= StartPassiveCooldown;
    }

    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 피격 무효화 패시브 발동");
                // OnOnceInvalidUsed 이벤트에 패시브 쿨타임 코루틴 구독
                statHolder.OnOnceInvalidUsed += StartPassiveCooldown;
                passiveCooldown = skillData.Cooldown;
                statHolder.GiveOnceInvalid();    // 1회 피격 무효 부여
                skillManager.effectController.PlaySkillEffect(0);
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] 무적 액티브 발동");
                if (activeCoroutine != null)
                {
                    StopCoroutine(activeCoroutine);
                }
                activeCoroutine = StartCoroutine(ActiveInvincible(skillData.Duration));
                break;
        }
    }
    
    // 패시브 쿨타임 스타트 (1회 피격 무효 부여 상태에서 피격 시에 호출)
    private void StartPassiveCooldown()
    {
        skillManager.effectController.StopSkillEffect(0);
        
        if (passiveCoroutine != null)
        {
            StopCoroutine(passiveCoroutine);
        }
        passiveCoroutine = StartCoroutine(PassiveCooldown());
        skillManager.StartSkillCooldown(0, passiveCooldown);
    }

    private IEnumerator PassiveCooldown()
    {
        yield return new WaitForSeconds(passiveCooldown);    // 패시브 쿨타임 대기
        statHolder.GiveOnceInvalid();    // 다시 1회 피격 무효 부여
        skillManager.effectController.PlaySkillEffect(0);
        passiveCoroutine = null;
    }
    
    private IEnumerator ActiveInvincible(float duration)
    {
        statHolder.GiveInvincible();    // 무적 부여
        skillManager.effectController.PlaySkillEffect(1);
        yield return new WaitForSeconds(duration);    // 액티브 지속시간 대기
        statHolder.RemoveInvincible();    // 무적 삭제
        skillManager.effectController.StopSkillEffect(1);
    }
}
