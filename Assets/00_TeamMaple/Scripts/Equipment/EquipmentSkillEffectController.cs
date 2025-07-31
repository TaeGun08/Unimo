using System.Collections;
using UnityEngine;

public class EquipmentSkillEffectController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private PrefabsTable effectTable;

    // �̸� ������ ����Ʈ
    private GameObject[] skillEffects = new GameObject[2];

    // ��ų�� ����Ʈ �̸� ����
    public void SetSkillEffects(int idx, int skillId)
    {
        ResetSkillEffects();

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
        if (skillEffects[idx] != null)
        {
            // �׻� ������Ϸ��� ���� �ѱ� (��ƼŬ ���)
            skillEffects[idx].SetActive(false);
            skillEffects[idx].SetActive(true);

            // ��ƼŬ �ڵ� Off
            var ps = skillEffects[idx].GetComponent<ParticleSystem>();
            if (ps != null)
            {
                StartCoroutine(AutoDisableEffect(skillEffects[idx], ps));
            }
        }
    }

    // ��ƼŬ�� ������ �ڵ� ��Ȱ��ȭ
    private IEnumerator AutoDisableEffect(GameObject effect, ParticleSystem ps)
    {
        yield return new WaitForSeconds(ps.main.duration + ps.main.startLifetime.constantMax);

        if (effect != null && effect.activeSelf)
        {
            effect.SetActive(false);
        }
    }

    // ��ų ��ü, ���½� ����Ʈ ����
    public void ResetSkillEffects()
    {
        for (int i = 0; i < skillEffects.Length; i++)
        {
            if (skillEffects[i] != null)
            {
                Destroy(skillEffects[i]);
                skillEffects[i] = null;
            }
        }
    }
}
