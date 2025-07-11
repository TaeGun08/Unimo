using UnityEngine;

// 스턴 회복률 증가 스킬
public class StunRecoverySkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillType type, float cooldown, float duration, float param)
    {
        switch (type)
        {
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 스턴 회복률 증가 패시브 발동");
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] 스턴 회복률 증가 액티브 발동");
                break;
        }
    }
}
