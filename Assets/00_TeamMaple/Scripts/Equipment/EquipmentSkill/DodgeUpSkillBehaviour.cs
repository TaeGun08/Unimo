using System.Collections;
using UnityEngine;

// 회피율 증가 스킬
public class DodgeUpSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private Coroutine activeCoroutine;
    
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var dodge = LocalPlayer.Instance.PlayerStatHolder.Dodge;    // 기존 값
        var addDodge = dodge.Value * skillData.Param;    // 기존 값 * 증가 퍼센트
        
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 회피율 증가 패시브 발동");
                dodge.Add(addDodge);    // 회피율 증가
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] 회피율 증가 액티브 발동");
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
        dodge.Add(addDodge);    // 회피율 일시 증가
        yield return new WaitForSeconds(duration);    // duration초 대기
        dodge.Subtract(addDodge);    // 증가분 다시 감소
        activeCoroutine = null;
    }
}
