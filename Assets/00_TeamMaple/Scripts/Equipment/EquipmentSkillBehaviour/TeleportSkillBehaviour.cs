using System.Collections;
using UnityEngine;

// 텔레포트 스킬
public class TeleportSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        switch (skillData.Type)
        {
            case EquipmentSkillType.Passive:
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] 텔레포트 액티브 발동");
                Teleport(caster, skillData.Param);
                break;
        }
    }

    private void Teleport(GameObject caster, float distance)
    {
        var mapSetter = PlaySystemRefStorage.mapSetter;
        Vector3 start = caster.transform.position;
        Vector3 end = start + caster.transform.forward * distance;

        // 목표 위치가 맵 밖이면 보정
        if (mapSetter != null && !mapSetter.IsInMap(end))
        {
            end = mapSetter.FindNearestPoint(end);
        }

        caster.transform.position = end;
        // 이펙트나 사운드 등 추가 가능
    }
}