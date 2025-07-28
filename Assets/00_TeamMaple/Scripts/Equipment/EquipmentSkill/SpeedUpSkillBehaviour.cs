using System.Collections;
using UnityEngine;

// 이동 속도 증가 스킬
public class SpeedUpSkillBehaviour : MonoBehaviour,IEquipmentSkillBehaviour
{
    private Coroutine activeCoroutine;
    
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var speed = LocalPlayer.Instance.PlayerStatHolder.Speed;    // 기존 값
        var addSpeed = speed.Value * skillData.Param;    // 기존 값 * 증가 퍼센트
        
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 이동 속도 증가 패시브 발동");
                speed.Add(addSpeed);    // 이동 속도 증가
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] 이동 속도 증가 액티브 발동");
                if (activeCoroutine != null)
                {
                    StopCoroutine(activeCoroutine);
                }
                activeCoroutine = StartCoroutine(ActiveSpeedUp(speed, skillData.Duration, addSpeed));
                break;
        }
    }
    
    private IEnumerator ActiveSpeedUp(ClampedFloat speed, int duration, float addSpeed)
    {
        speed.Add(addSpeed);    // 이동 속도 일시 증가
        yield return new WaitForSeconds(duration);    // duration초 대기
        speed.Subtract(addSpeed);    // 증가분 다시 감소
        activeCoroutine = null;
    }
}
