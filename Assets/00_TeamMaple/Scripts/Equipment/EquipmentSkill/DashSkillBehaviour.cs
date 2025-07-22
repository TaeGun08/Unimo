using System.Collections;
using UnityEngine;

// 대쉬 스킬
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
                Debug.Log("[Active] 대쉬 액티브 발동");
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

        // 1. 대시 목표 위치가 맵 밖이면 가장 가까운 맵 안 위치로 조정
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
        caster.transform.position = end; // 마지막 위치 정확히 맞추기
    }
}