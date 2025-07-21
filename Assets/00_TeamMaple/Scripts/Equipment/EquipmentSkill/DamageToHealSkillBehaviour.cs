using System;
using System.Collections;
using UnityEngine;

// �ǰ� �������� ü�� ȸ������ ��ȯ ��ų
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
                Debug.Log("[Active] �ǰ� �������� ü�� ȸ������ ��ȯ ��Ƽ�� �ߵ�");
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
        statHolder.SetDamageToHeal(true);    // IsDamageToHeal -> true�� ����
        yield return new WaitForSeconds(duration);    // duration�� ���
        statHolder.SetDamageToHeal(false);    // IsDamageToHeal -> false�� ����
        activeCoroutine = null;
    }
}
