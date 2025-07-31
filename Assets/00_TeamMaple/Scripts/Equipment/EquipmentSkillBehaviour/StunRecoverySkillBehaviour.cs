using UnityEngine;

// ���� ȸ���� ���� ��ų
public class StunRecoverySkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var stunRecovery = LocalPlayer.Instance.PlayerStatHolder.StunRecovery;    // ���� ��
        var addStunRecovery = stunRecovery.Value * skillData.Param;    // ���� �� * ���� �ۼ�Ʈ
        
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] ���� ȸ���� ���� �нú� �ߵ�");
                stunRecovery.Add(addStunRecovery);    // ���� ȸ���� ����
                break;
            case EquipmentSkillType.Active:
                break;
        }
    }
}
