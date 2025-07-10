using System;
using UnityEngine;

// 개화 범위 증가 스킬
public class BloomRangeUpSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillType type, float cooldown, float duration, float param1, float param2)
    {
        switch (type)
        {
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 개화 범위 증가 패시브 발동");
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] 개화 범위 증가 액티브 발동");
                break;
        }
    }
}
