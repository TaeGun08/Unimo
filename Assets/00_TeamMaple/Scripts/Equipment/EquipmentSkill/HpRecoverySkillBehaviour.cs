using UnityEngine;

// ü�� ȸ�� ��ų
public class HpRecoverySkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillType type, float cooldown, float duration, float param1, float param2)
    {
        switch (type)
        {
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] ü�� ȸ�� �нú� �ߵ�");
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] ü�� ȸ�� ��Ƽ�� �ߵ�");
                break;
        }
    }
}
