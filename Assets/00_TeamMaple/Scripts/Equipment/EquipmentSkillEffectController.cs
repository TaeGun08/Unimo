using System.Collections;
using UnityEngine;

public class EquipmentSkillEffectController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private PrefabsTable effectTable;

    // �̸� ������ ����Ʈ
    private GameObject[] skillEffects = new GameObject[2];
    private Coroutine[] effectCoroutines = new Coroutine[2];

    // ��ų�� ����Ʈ �̸� ����
    public void SetSkillEffects(int idx, int skillId)
    {
        // ���Ժ��� ���� ����Ʈ�� Destroy
        if (skillEffects[idx] != null)
        {
            Destroy(skillEffects[idx]);
            skillEffects[idx] = null;
        }
        
        var effectPrefab = effectTable.GetPrefabByKey(skillId);
        if (effectPrefab != null)
        {
            var effectObj = Instantiate(effectPrefab, player.transform);
            effectObj.SetActive(false);
            skillEffects[idx] = effectObj;
        }
    }

    // ��ų ȿ�� ���� (On)
    public void PlaySkillEffect(int idx)
    {
        var effect = skillEffects[idx];
        if (effect == null) return;

        var ps = effect.GetComponent<ParticleSystem>();
        if (ps == null)
        {
            // ��ƼŬ ���� ��� �׳� ���ֱ�(Ư��ȿ�� ��)
            effect.SetActive(true);
            return;
        }

        if (!ps.main.loop) // ��ȸ�� ��ƼŬ (loop=false)
        {
            // ���(��ȸ��): �׻� ���� �Ѱ�, �ڷ�ƾ���� �ڵ� off
            if (effectCoroutines[idx] != null)
                StopCoroutine(effectCoroutines[idx]);

            effect.SetActive(false);
            effect.SetActive(true);

            effectCoroutines[idx] = StartCoroutine(AutoDisableEffect(effect, ps, idx));
        }
        else // loop=true, ������
        {
            effect.SetActive(true);
        }
    }

    public void StopSkillEffect(int idx)
    {
        var effect = skillEffects[idx];
        if (effect == null) return;

        var ps = effect.GetComponent<ParticleSystem>();
        if (ps != null && ps.main.loop)
        {
            effect.SetActive(false);

            if (effectCoroutines[idx] != null)
            {
                StopCoroutine(effectCoroutines[idx]);
                effectCoroutines[idx] = null;
            }
        }
    }
    
    // ��ƼŬ�� ������ �ڵ� ��Ȱ��ȭ
    private IEnumerator AutoDisableEffect(GameObject effect, ParticleSystem ps, int idx)
    {
        // ��ȸ�� ��ƼŬ�� ���� ������ ���
        yield return new WaitForSeconds(ps.main.duration + ps.main.startLifetime.constantMax);

        if (effect != null && effect.activeSelf)
            effect.SetActive(false);

        effectCoroutines[idx] = null;
    }
}
