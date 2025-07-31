using UnityEngine;

// �� ���Ϸ� ���� ��ų
public class FlowerDropAmountUpSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var flowerDropAmount = LocalPlayer.Instance.PlayerStatHolder.FlowerDropAmount;    // ���� ��
        var addFlowerDropAmount = flowerDropAmount.Value * skillData.Param;    // ���� �� * ���� �ۼ�Ʈ
        
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] �� ���Ϸ� ���� �нú� �ߵ�");
                flowerDropAmount.Add(addFlowerDropAmount);    // �� ���Ϸ� ����
                break;
            case EquipmentSkillType.Active:
                break;
        }
    }
}
