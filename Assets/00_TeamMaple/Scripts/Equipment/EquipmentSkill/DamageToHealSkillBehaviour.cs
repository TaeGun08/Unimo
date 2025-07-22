using System;
using System.Collections;
using UnityEngine;

// 피격 데미지를 체력 회복으로 전환 스킬
public class DamageToHealSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private Coroutine activeCoroutine;
    
    private PlayerStatHolder statHolder;

    private void Awake()
    {
        statHolder = LocalPlayer.Instance.PlayerStatHolder;
    }

    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] 피격 데미지를 체력 회복으로 전환 액티브 발동");
                if (activeCoroutine != null)
                {
                    StopCoroutine(activeCoroutine);
                }
                activeCoroutine = StartCoroutine(ActiveDamageToHeal(skillData.Duration));
                break;
        }
    }
    
    private IEnumerator ActiveDamageToHeal(int duration)
    {
        statHolder.SetDamageToHeal(true);    // IsDamageToHeal -> true로 설정
        yield return new WaitForSeconds(duration);    // duration초 대기
        statHolder.SetDamageToHeal(false);    // IsDamageToHeal -> false로 설정
        activeCoroutine = null;
    }
}
