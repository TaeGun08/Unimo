using System.Collections;
using UnityEngine;

// Èñ±Í ²É »ý¼º È®·ü Áõ°¡ ½ºÅ³
public class RareFlowerRateUpSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private Coroutine activeCoroutine;
    private EquipmentSkillManager skillManager;
    
    private void Awake()
    {
        skillManager = EquipmentSkillManager.Instance;
    }
    
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var rareFlowerRate = LocalPlayer.Instance.PlayerStatHolder.RareFlowerRate;    // ±âÁ¸ °ª
        var addRareFlowerRate = rareFlowerRate.Value * skillData.Param;    // ±âÁ¸ °ª * Áõ°¡ ÆÛ¼¾Æ®
        
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] Èñ±Í ²É »ý¼º È®·ü Áõ°¡ ÆÐ½Ãºê ¹ßµ¿");
                rareFlowerRate.Add(addRareFlowerRate);    // Èñ±Í ²É »ý¼º È®·ü Áõ°¡
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] Èñ±Í ²É »ý¼º È®·ü Áõ°¡ ¾×Æ¼ºê ¹ßµ¿");
                if (activeCoroutine != null)
                {
                    StopCoroutine(activeCoroutine);
                }
                activeCoroutine = StartCoroutine(ActiveRareFlowerRateUp(rareFlowerRate, skillData.Duration, addRareFlowerRate));
                break;
        }
    }
    
    private IEnumerator ActiveRareFlowerRateUp(ClampedFloat rareFlowerRate, float duration, float addRareFlowerRate)
    {
        rareFlowerRate.Add(addRareFlowerRate);    // Èñ±Í ²É »ý¼º È®·ü ÀÏ½Ã Áõ°¡
        skillManager.effectController.PlaySkillEffect(1);
        yield return new WaitForSeconds(duration);    // durationÃÊ ´ë±â
        rareFlowerRate.Subtract(addRareFlowerRate);    // Áõ°¡ºÐ ´Ù½Ã °¨¼Ò
        skillManager.effectController.StopSkillEffect(1);
        activeCoroutine = null;
    }
}
