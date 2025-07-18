using UnityEngine;

// �̵� �ӵ� ���� ��ų
public class SpeedUpSkillBehaviour : MonoBehaviour,IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillType type, int duration, float param)
    {
        var speed = LocalPlayer.Instance.PlayerStatHolder.Speed;
        var addSpeed = speed.Value * param;
        
        switch (type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] �̵� �ӵ� ���� �нú� �ߵ�");
                speed.Add(addSpeed);
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] �̵� �ӵ� ���� ��Ƽ�� �ߵ�");
                break;
        }
    }
}
