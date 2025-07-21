using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SkillRunner : MonoBehaviour
{
    [SerializeField] private PrefabsTable skillTable;
    [SerializeField] private GameObject player;
    
    [SerializeField] private EquipmentStatDataSO equipmentStatDataSo;
    [SerializeField] private EquipmentSkillDataSO skillDataSo;

    private bool isSkill2OnCooldown = false;    // 쿨타임 체크 변수
    private Coroutine skill2CooldownCoroutine;
    
    // Skill2 실행 참조용 변수 (셋업 시 저장)
    private IEquipmentSkillBehaviour skillExcutor2;
    private EquipmentSkillData skillData2;
    
    private float lastTapTime = 0f;
    private const float doubleTapThreshold = 0.25f; // 더블탭 허용 간격 (초)
    
    private void Start()
    {
        SetEngineSkills();
    }

    private void Update()
    {
#if UNITY_EDITOR
        // 에디터에서는 마우스 더블클릭도 지원
        if (Input.GetMouseButtonDown(0))
        {
            float currentTime = Time.time;
            if (currentTime - lastTapTime < doubleTapThreshold)
            {
                TryUseSkill2();
            }
            lastTapTime = currentTime;
        }
#else
        // 모바일: 한 손가락 더블탭 감지
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                float currentTime = Time.time;
                if (currentTime - lastTapTime < doubleTapThreshold)
                {
                    TryUseSkill2();
                }
                lastTapTime = currentTime;
            }
        }
#endif
    }

    private void SetEngineSkills()
    {
        var engineStatData = equipmentStatDataSo.GetEquipmentStatData(Base_Mng.Data.data.EQCount);

        // 테스트용
        var skillId1 = 2011;
        var skillId2 = 2017;
        
        // 스킬 아이디 확인 (0이면 스킬 없음)
        // var skillId1 = engineStatData.Skill1;
        // var skillId2 = engineStatData.Skill2;

        // 스킬1
        if (skillId1 != 0)
        {
            var skillPrefab1 = Instantiate(skillTable.GetPrefabByKey(skillId1), player.transform);
            var skillExcutor1 = skillPrefab1.GetComponent<IEquipmentSkillBehaviour>();
            var skillData1 = skillDataSo.GetEquipmentSkillData(skillId1);

            skillExcutor1.Excute(player, skillData1);
            Debug.Log($"[Skill1] Id: {skillId1} / Type: {skillData1.Type} / Cooldown: {skillData1.Cooldown} / Duration: {skillData1.Duration} / Param: {skillData1.Param}");
        }
        else
        {
            Debug.Log("[Skill1] 스킬 없음 (ID=0)");
        }

        // 스킬2
        if (skillId2 != 0)
        {
            var skillPrefab2 = Instantiate(skillTable.GetPrefabByKey(skillId2), player.transform);
            skillExcutor2 = skillPrefab2.GetComponent<IEquipmentSkillBehaviour>();
            skillData2 = skillDataSo.GetEquipmentSkillData(skillId2);
        }
        else
        {
            Debug.Log("[Skill2] 스킬 없음 (ID=0)");
        }
    }
    
    private void TryUseSkill2()
    {
        if (skillExcutor2 == null || skillData2 == null)
            return;

        if (isSkill2OnCooldown)
        {
            Debug.Log("[Skill2] 쿨타임 중!");
            return;
        }

        skillExcutor2.Excute(player, skillData2);
        Debug.Log($"[Skill2] Id: {skillData2.Id} / Type: {skillData2.Type} / Cooldown: {skillData2.Cooldown} / Duration: {skillData2.Duration} / Param: {skillData2.Param}");

        if (skill2CooldownCoroutine != null)
        {
            StopCoroutine(skill2CooldownCoroutine);
        }
        skill2CooldownCoroutine = StartCoroutine(Skill2Cooldown(skillData2.Cooldown));
    }
    
    private IEnumerator Skill2Cooldown(float cooldown)
    {
        isSkill2OnCooldown = true;
        Debug.Log($"[Skill2] 쿨타임 시작 ({cooldown}초)");
        
        yield return new WaitForSeconds(cooldown);
        
        isSkill2OnCooldown = false;
        Debug.Log("[Skill2] 쿨타임 종료");
    }
}
