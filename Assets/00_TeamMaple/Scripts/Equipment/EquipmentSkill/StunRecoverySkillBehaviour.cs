using UnityEngine;

// ���� ȸ���� ���� ��ų
public class StunRecoverySkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillType type, float cooldown, float duration, float param)
    {
        switch (type)
        {
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] ���� ȸ���� ���� �нú� �ߵ�");
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] ���� ȸ���� ���� ��Ƽ�� �ߵ�");
                break;
        }
    }
}
