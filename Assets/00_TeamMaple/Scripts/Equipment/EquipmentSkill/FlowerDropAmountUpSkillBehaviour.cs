using UnityEngine;

// 꽃 낙하량 증가 스킬
public class FlowerDropAmountUpSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var flowerDropAmount = LocalPlayer.Instance.PlayerStatHolder.FlowerDropAmount;    // 기존 값
        var addFlowerDropAmount = flowerDropAmount.Value * skillData.Param;    // 기존 값 * 증가 퍼센트
        
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 꽃 낙하량 증가 패시브 발동");
                flowerDropAmount.Add(addFlowerDropAmount);    // 꽃 낙하량 증가
                break;
            case EquipmentSkillType.Active:
                break;
        }
    }
}
