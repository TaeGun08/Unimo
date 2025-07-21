using System.Collections;
using UnityEngine;

// 대쉬 스킬
public class DashSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        // 대시 효과 코루틴 실행
        StartCoroutine(DashRoutine(caster, skillData.Param, 0.15f));
    }

    private IEnumerator DashRoutine(GameObject caster, float dashPower, float duration)
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