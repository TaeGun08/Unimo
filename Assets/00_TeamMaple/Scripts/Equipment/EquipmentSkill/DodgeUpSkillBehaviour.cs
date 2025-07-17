using System.Collections;
using UnityEngine;

// 회피율 증가 스킬
public class DodgeUpSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private Coroutine activeCoroutine;
    
    public void Excute(GameObject caster, EquipmentSkillType type, int duration, float param)
    {
        var dodge = LocalPlayer.Instance.PlayerStatHolder.Dodge;
        var addDodge = dodge.Value * param;
        
        switch (type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 회피율 증가 패시브 발동");
                dodge.Add(addDodge);
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] 회피율 증가 액티브 발동");
                if (activeCoroutine != null)
                {
                    StopCoroutine(activeCoroutine);
                }
                activeCoroutine = StartCoroutine(ActiveDodgeUp(dodge, duration, addDodge));
                break;
        }
    }
    
    private IEnumerator ActiveDodgeUp(ClampedFloat dodge, int duration, float addDodge)
    {
        dodge.Add(addDodge);    // 회피율 일시 증가
        yield return new WaitForSeconds(duration);    // duration초 대기
        dodge.Subtract(addDodge);       // 증가분 다시 감소
        activeCoroutine = null;
    }
}
