using UnityEngine;

// ���� ���� ��ų
public class DefenseUpSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillType type, int duration, float param)
    {
        var def = LocalPlayer.Instance.PlayerStatHolder.Def;
        var addDef = (int)(def.Value * param);
        
        switch (type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] ���� ���� �нú� �ߵ�");
                def.Add(addDef);
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] ���� ���� ��Ƽ�� �ߵ�");
                break;
        }
    }
}
