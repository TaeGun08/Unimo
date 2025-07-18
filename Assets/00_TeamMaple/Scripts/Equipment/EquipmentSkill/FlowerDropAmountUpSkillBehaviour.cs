using UnityEngine;

// �� ���Ϸ� ���� ��ų
public class FlowerDropAmountUpSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillType type, int duration, float param)
    {
        var flowerDropAmount = LocalPlayer.Instance.PlayerStatHolder.FlowerDropAmount;
        var addFlowerDropAmount = flowerDropAmount.Value * param;
        
        switch (type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] �� ���Ϸ� ���� �нú� �ߵ�");
                flowerDropAmount.Add(addFlowerDropAmount);
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] �� ���Ϸ� ���� ��Ƽ�� �ߵ�");
                break;
        }
    }
}
