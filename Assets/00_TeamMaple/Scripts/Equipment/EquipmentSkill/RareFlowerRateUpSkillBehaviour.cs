using System.Collections;
using UnityEngine;

// Èñ±Í ²É »ý¼º È®·ü Áõ°¡ ½ºÅ³
public class RareFlowerRateUpSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private Coroutine activeCoroutine;
    
    public void Excute(GameObject caster, EquipmentSkillType type, int duration, float param)
    {
        var rareFlowerRate = LocalPlayer.Instance.PlayerStatHolder.RareFlowerRate;
        var addRareFlowerRate = rareFlowerRate.Value * param;
        
        switch (type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] Èñ±Í ²É »ý¼º È®·ü Áõ°¡ ÆÐ½Ãºê ¹ßµ¿");
                rareFlowerRate.Add(addRareFlowerRate);
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] Èñ±Í ²É »ý¼º È®·ü Áõ°¡ ¾×Æ¼ºê ¹ßµ¿");
                if (activeCoroutine != null)
                {
                    StopCoroutine(activeCoroutine);
                }
                activeCoroutine = StartCoroutine(ActiveRareFlowerRateUp(rareFlowerRate, duration, addRareFlowerRate));
                break;
        }
    }
    
    private IEnumerator ActiveRareFlowerRateUp(ClampedFloat rareFlowerRate, int duration, float addRareFlowerRate)
    {
        rareFlowerRate.Add(addRareFlowerRate);
        yield return new WaitForSeconds(duration);
        rareFlowerRate.Subtract(addRareFlowerRate);
        activeCoroutine = null;
    }
}
