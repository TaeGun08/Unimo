using System.Collections;
using UnityEngine;

// ��� �� ���� Ȯ�� ���� ��ų
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
        var rareFlowerRate = LocalPlayer.Instance.PlayerStatHolder.RareFlowerRate;    // ���� ��
        var addRareFlowerRate = rareFlowerRate.Value * skillData.Param;    // ���� �� * ���� �ۼ�Ʈ
        
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] ��� �� ���� Ȯ�� ���� �нú� �ߵ�");
                rareFlowerRate.Add(addRareFlowerRate);    // ��� �� ���� Ȯ�� ����
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] ��� �� ���� Ȯ�� ���� ��Ƽ�� �ߵ�");
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
        rareFlowerRate.Add(addRareFlowerRate);    // ��� �� ���� Ȯ�� �Ͻ� ����
        skillManager.effectController.PlaySkillEffect(1);
        yield return new WaitForSeconds(duration);    // duration�� ���
        rareFlowerRate.Subtract(addRareFlowerRate);    // ������ �ٽ� ����
        skillManager.effectController.StopSkillEffect(1);
        activeCoroutine = null;
    }
}
