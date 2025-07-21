using System.Collections;
using UnityEngine;

// �뽬 ��ų
public class DashSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        // ��� ȿ�� �ڷ�ƾ ����
        StartCoroutine(DashRoutine(caster, skillData.Param, 0.15f));
    }

    private IEnumerator DashRoutine(GameObject caster, float dashPower, float duration)
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