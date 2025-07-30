using System.Collections;
using UnityEngine;

// �̵� �ӵ� ���� ��ų
public class SpeedUpSkillBehaviour : MonoBehaviour,IEquipmentSkillBehaviour
{
    private Coroutine activeCoroutine;
    
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var speed = LocalPlayer.Instance.PlayerStatHolder.Speed;    // ���� ��
        var addSpeed = speed.Value * skillData.Param;    // ���� �� * ���� �ۼ�Ʈ
        
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] �̵� �ӵ� ���� �нú� �ߵ�");
                speed.Add(addSpeed);    // �̵� �ӵ� ����
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] �̵� �ӵ� ���� ��Ƽ�� �ߵ�");
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
        speed.Add(addSpeed);    // �̵� �ӵ� �Ͻ� ����
        yield return new WaitForSeconds(duration);    // duration�� ���
        speed.Subtract(addSpeed);    // ������ �ٽ� ����
        activeCoroutine = null;
    }
}
