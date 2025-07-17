using System.Collections;
using UnityEngine;

// 회피율 증가 스킬
public class DodgeUpSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private Coroutine activeCoroutine;
    
    public void Excute(GameObject caster, EquipmentSkillType type, int duration, float param)
    {
        var dodge = LocalPlayer.Instance.PlayerStatHolder.Dodge;
        
        switch (type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 회피율 증가 패시브 발동");
                dodge.Add(param);
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] 회피율 증가 액티브 발동");
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
        dodge.Add(param);    // 회피율 일시 증가
        yield return new WaitForSeconds(duration);    // duration초 대기
        dodge.Subtract(param);       // 증가분 다시 감소
        activeCoroutine = null;
    }
}
