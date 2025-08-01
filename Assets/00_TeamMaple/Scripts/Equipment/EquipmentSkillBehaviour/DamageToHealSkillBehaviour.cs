using System;
using System.Collections;
using UnityEngine;

// �ǰ� �������� ü�� ȸ������ ��ȯ ��ų
public class DamageToHealSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private Coroutine activeCoroutine;
    
    private PlayerStatHolder statHolder;
    private EquipmentSkillManager skillManager;

    private void Awake()
    {
        statHolder = LocalPlayer.Instance.PlayerStatHolder;
        skillManager = EquipmentSkillManager.Instance;
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
    
    private IEnumerator ActiveDamageToHeal(float duration)
    {
        statHolder.SetDamageToHeal(true);    // IsDamageToHeal -> true�� ����
        skillManager.effectController.PlaySkillEffect(1);
        yield return new WaitForSeconds(duration);    // duration�� ���
        statHolder.SetDamageToHeal(false);    // IsDamageToHeal -> false�� ����
        skillManager.effectController.StopSkillEffect(1);

        activeCoroutine = null;
    }
}
