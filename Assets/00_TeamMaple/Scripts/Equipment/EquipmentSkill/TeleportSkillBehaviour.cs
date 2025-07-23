using System.Collections;
using UnityEngine;

// �ڷ���Ʈ ��ų
public class TeleportSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        switch (skillData.Type)
        {
            case EquipmentSkillType.Passive:
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] �ڷ���Ʈ ��Ƽ�� �ߵ�");
                Teleport(caster, skillData.Param);
                break;
        }
    }

    private void Teleport(GameObject caster, float distance)
    {
        var mapSetter = PlaySystemRefStorage.mapSetter;
        Vector3 start = caster.transform.position;
        Vector3 end = start + caster.transform.forward * distance;

        // ��ǥ ��ġ�� �� ���̸� ����
        if (mapSetter != null && !mapSetter.IsInMap(end))
        {
            end = mapSetter.FindNearestPoint(end);
        }

        caster.transform.position = end;
        // ����Ʈ�� ���� �� �߰� ����
    }
}