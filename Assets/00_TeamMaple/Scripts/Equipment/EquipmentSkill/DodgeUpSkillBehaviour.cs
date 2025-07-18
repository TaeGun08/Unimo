using System.Collections;
using UnityEngine;

// ȸ���� ���� ��ų
public class DodgeUpSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private Coroutine activeCoroutine;
    
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var dodge = LocalPlayer.Instance.PlayerStatHolder.Dodge;    // ���� ��
        var addDodge = dodge.Value * skillData.Param;    // ���� �� * ���� �ۼ�Ʈ
        
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] ȸ���� ���� �нú� �ߵ�");
                dodge.Add(addDodge);    // ȸ���� ����
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] ȸ���� ���� ��Ƽ�� �ߵ�");
                if (activeCoroutine != null)
                {
                    StopCoroutine(activeCoroutine);
                }
                activeCoroutine = StartCoroutine(ActiveDodgeUp(dodge, skillData.Duration, addDodge));
                break;
        }
    }
    
    private IEnumerator ActiveDodgeUp(ClampedFloat dodge, int duration, float addDodge)
    {
        dodge.Add(addDodge);    // ȸ���� �Ͻ� ����
        yield return new WaitForSeconds(duration);    // duration�� ���
        dodge.Subtract(addDodge);    // ������ �ٽ� ����
        activeCoroutine = null;
    }
}
