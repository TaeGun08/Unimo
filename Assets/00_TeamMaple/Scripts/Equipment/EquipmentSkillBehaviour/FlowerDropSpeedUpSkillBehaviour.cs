using System.Collections;
using UnityEngine;

// �� ���� �ӵ� ���� ��ų
public class FlowerDropSpeedUpSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private Coroutine activeCoroutine;
    
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var flowerDropSpeed = LocalPlayer.Instance.PlayerStatHolder.FlowerDropSpeed;    // ���� ��
        var addFlowerDropSpeed = flowerDropSpeed.Value * skillData.Param;    // ���� �� * ���� �ۼ�Ʈ
        
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] �� ���� �ӵ� ���� �нú� �ߵ�");
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] �� ���� �ӵ� ���� ��Ƽ�� �ߵ�");
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
        flowerDropSpeed.Add(addFlowerDropSpeed);    // �� ���� �ӵ� �Ͻ� ����
        yield return new WaitForSeconds(duration);    // duration�� ���
        flowerDropSpeed.Subtract(addFlowerDropSpeed);    // ������ �ٽ� ����
        activeCoroutine = null;
    }
}
