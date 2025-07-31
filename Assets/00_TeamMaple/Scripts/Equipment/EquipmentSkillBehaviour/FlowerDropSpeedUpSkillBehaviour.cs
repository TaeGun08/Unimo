using System.Collections;
using UnityEngine;

// 꽃 낙하 속도 증가 스킬
public class FlowerDropSpeedUpSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private Coroutine activeCoroutine;
    
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var flowerDropSpeed = LocalPlayer.Instance.PlayerStatHolder.FlowerDropSpeed;    // 기존 값
        var addFlowerDropSpeed = flowerDropSpeed.Value * skillData.Param;    // 기존 값 * 증가 퍼센트
        
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 꽃 낙하 속도 증가 패시브 발동");
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] 꽃 낙하 속도 증가 액티브 발동");
                if (activeCoroutine != null)
                {
                    StopCoroutine(activeCoroutine);
                }
                activeCoroutine = StartCoroutine(ActiveFlowerDropSpeedUp(flowerDropSpeed, skillData.Duration, addFlowerDropSpeed));
                break;
        }
    }
    
    private IEnumerator ActiveFlowerDropSpeedUp(ClampedFloat flowerDropSpeed, float duration, float addFlowerDropSpeed)
    {
        flowerDropSpeed.Add(addFlowerDropSpeed);    // 꽃 낙하 속도 일시 증가
        yield return new WaitForSeconds(duration);    // duration초 대기
        flowerDropSpeed.Subtract(addFlowerDropSpeed);    // 증가분 다시 감소
        activeCoroutine = null;
    }
}
