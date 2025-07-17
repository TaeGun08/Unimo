using UnityEngine;

// ���� ȸ���� ���� ��ų
public class StunRecoverySkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillType type, int duration, float param)
    {
        var stunRecovery = LocalPlayer.Instance.PlayerStatHolder.StunRecovery;
        
        switch (type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] ���� ȸ���� ���� �нú� �ߵ�");
                stunRecovery.Add(param);
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] ���� ȸ���� ���� ��Ƽ�� �ߵ�");
                break;
        }
    }
}
