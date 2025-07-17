using System.Collections;
using UnityEngine;

// ȸ���� ���� ��ų
public class DodgeUpSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private Coroutine activeCoroutine;
    
    public void Excute(GameObject caster, EquipmentSkillType type, int duration, float param)
    {
        var dodge = LocalPlayer.Instance.PlayerStatHolder.Dodge;
        
        switch (type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] ȸ���� ���� �нú� �ߵ�");
                dodge.Add(param);
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] ȸ���� ���� ��Ƽ�� �ߵ�");
                if (activeCoroutine != null)
                {
                    StopCoroutine(activeCoroutine);
                }
                activeCoroutine = StartCoroutine(ActiveDodgeUp(dodge, duration, param));
                break;
        }
    }
    
    private IEnumerator ActiveDodgeUp(ClampedFloat dodge, int duration, float param)
    {
        dodge.Add(param);    // ȸ���� �Ͻ� ����
        yield return new WaitForSeconds(duration);    // duration�� ���
        dodge.Subtract(param);       // ������ �ٽ� ����
        activeCoroutine = null;
    }
}
