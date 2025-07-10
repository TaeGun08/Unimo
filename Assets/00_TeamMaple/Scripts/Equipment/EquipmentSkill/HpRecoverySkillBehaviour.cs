using UnityEngine;

// 체력 회복 스킬
public class HpRecoverySkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillType type, float cooldown, float duration, float param1, float param2)
    {
        switch (type)
        {
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 체력 회복 패시브 발동");
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] 체력 회복 액티브 발동");
                break;
        }
    }
}
