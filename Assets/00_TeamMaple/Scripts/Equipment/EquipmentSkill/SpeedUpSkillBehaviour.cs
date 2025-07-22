using UnityEngine;

// �̵� �ӵ� ���� ��ų
public class SpeedUpSkillBehaviour : MonoBehaviour,IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var speed = LocalPlayer.Instance.PlayerStatHolder.Speed;    // ���� ��
        var addSpeed = speed.Value * skillData.Param;    // ���� �� * ���� �ۼ�Ʈ
        
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] �̵� �ӵ� ���� �нú� �ߵ�");
                speed.Add(addSpeed);    // �̵� �ӵ� ����
                break;
            case EquipmentSkillType.Active:
                break;
        }
    }
}
