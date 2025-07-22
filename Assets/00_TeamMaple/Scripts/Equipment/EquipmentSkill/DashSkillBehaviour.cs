using System.Collections;
using UnityEngine;

// �뽬 ��ų
public class DashSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private Coroutine activeCoroutine;
    
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] �뽬 ��Ƽ�� �ߵ�");
                if (activeCoroutine != null)
                {
                    StopCoroutine(activeCoroutine);
                }
                activeCoroutine = StartCoroutine(ActiveDash(caster, skillData.Param, 0.1f));
                break;
        }
    }

    private IEnumerator ActiveDash(GameObject caster, float dashPower, float duration)
    {
        var mapSetter = PlaySystemRefStorage.mapSetter;
        Vector3 start = caster.transform.position;
        Vector3 end = start + caster.transform.forward * dashPower;

        // 1. ��� ��ǥ ��ġ�� �� ���̸� ���� ����� �� �� ��ġ�� ����
        if (mapSetter != null && !mapSetter.IsInMap(end))
        {
            end = mapSetter.FindNearestPoint(end);
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            caster.transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        caster.transform.position = end; // ������ ��ġ ��Ȯ�� ���߱�
    }
}