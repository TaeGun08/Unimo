using System;
using UnityEngine;

// ��ȭ ���� ���� ��ų
public class BloomRangeUpSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillType type, float cooldown, float duration, float param1, float param2)
    {
        switch (type)
        {
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] ��ȭ ���� ���� �нú� �ߵ�");
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] ��ȭ ���� ���� ��Ƽ�� �ߵ�");
                break;
        }
    }
}
