using System.Collections;
using UnityEngine;

public class EquipmentSkillEffectController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private PrefabsTable effectTable;

    // 미리 생성된 이펙트
    private GameObject[] skillEffects = new GameObject[2];
    private Coroutine[] effectCoroutines = new Coroutine[2];

    // 스킬별 이펙트 미리 생성
    public void SetSkillEffects(int idx, int skillId)
    {
        // 슬롯별로 기존 이펙트만 Destroy
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

    // 스킬 효과 실행 (On)
    public void PlaySkillEffect(int idx)
    {
        var effect = skillEffects[idx];
        if (effect == null) return;

        var ps = effect.GetComponent<ParticleSystem>();
        if (ps == null)
        {
            // 파티클 없는 경우 그냥 켜주기(특수효과 등)
            effect.SetActive(true);
            return;
        }

        if (!ps.main.loop) // 일회성 파티클 (loop=false)
        {
            // 즉발(일회성): 항상 껐다 켜고, 코루틴으로 자동 off
            if (effectCoroutines[idx] != null)
                StopCoroutine(effectCoroutines[idx]);

            effect.SetActive(false);
            effect.SetActive(true);

            effectCoroutines[idx] = StartCoroutine(AutoDisableEffect(effect, ps, idx));
        }
        else // loop=true, 지속형
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
    
    // 파티클이 끝나면 자동 비활성화
    private IEnumerator AutoDisableEffect(GameObject effect, ParticleSystem ps, int idx)
    {
        // 일회성 파티클이 끝날 때까지 대기
        yield return new WaitForSeconds(ps.main.duration + ps.main.startLifetime.constantMax);

        if (effect != null && effect.activeSelf)
            effect.SetActive(false);

        effectCoroutines[idx] = null;
    }
}
