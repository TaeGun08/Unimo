using System.Collections;
using UnityEngine;

public class EquipmentSkillEffectController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private PrefabsTable effectTable;

    // 미리 생성된 이펙트
    private GameObject[] skillEffects = new GameObject[2];

    // 스킬별 이펙트 미리 생성
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

    // 스킬 효과 실행 (On)
    public void PlaySkillEffect(int idx)
    {
        if (skillEffects[idx] != null)
        {
            // 항상 재실행하려면 껐다 켜기 (파티클 재생)
            skillEffects[idx].SetActive(false);
            skillEffects[idx].SetActive(true);

            // 파티클 자동 Off
            var ps = skillEffects[idx].GetComponent<ParticleSystem>();
            if (ps != null)
            {
                StartCoroutine(AutoDisableEffect(skillEffects[idx], ps));
            }
        }
    }

    // 파티클이 끝나면 자동 비활성화
    private IEnumerator AutoDisableEffect(GameObject effect, ParticleSystem ps)
    {
        yield return new WaitForSeconds(ps.main.duration + ps.main.startLifetime.constantMax);

        if (effect != null && effect.activeSelf)
        {
            effect.SetActive(false);
        }
    }

    // 스킬 교체, 리셋시 이펙트 제거
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
