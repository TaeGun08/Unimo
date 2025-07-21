using UnityEngine;

// ���� ���� ��ų
public class DefenseUpSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var def = LocalPlayer.Instance.PlayerStatHolder.Def;    // ���� ��
        var addDef = (int)(def.Value * skillData.Param);    // ���� �� * ���� �ۼ�Ʈ
        
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] ���� ���� �нú� �ߵ�");
                def.Add(addDef);    // ���� ����
                break;
            case EquipmentSkillType.Active:
                break;
        }
    }
}
