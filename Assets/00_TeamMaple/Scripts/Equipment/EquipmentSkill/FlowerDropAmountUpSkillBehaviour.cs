using UnityEngine;

// ²É ³«ÇÏ·® Áõ°¡ ½ºÅ³
public class FlowerDropAmountUpSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillType type, int duration, float param)
    {
        var flowerDropAmount = LocalPlayer.Instance.PlayerStatHolder.FlowerDropAmount;
        var addFlowerDropAmount = flowerDropAmount.Value * param;
        
        switch (type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] ²É ³«ÇÏ·® Áõ°¡ ÆÐ½Ãºê ¹ßµ¿");
                flowerDropAmount.Add(addFlowerDropAmount);
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] ²É ³«ÇÏ·® Áõ°¡ ¾×Æ¼ºê ¹ßµ¿");
                break;
        }
    }
}
