using System.Collections;
using UnityEngine;

// ��� �� ���� Ȯ�� ���� ��ų
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
                Debug.Log("[Passive] ��� �� ���� Ȯ�� ���� �нú� �ߵ�");
                rareFlowerRate.Add(addRareFlowerRate);
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] ��� �� ���� Ȯ�� ���� ��Ƽ�� �ߵ�");
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
