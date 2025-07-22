using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SkillRunner : MonoBehaviour
{
    [SerializeField] private PrefabsTable skillTable;
    [SerializeField] private GameObject player;
    
    [SerializeField] private EquipmentStatDataSO equipmentStatDataSo;
    [SerializeField] private EquipmentSkillDataSO skillDataSo;

    [Header("Skill Change UI")]
    [SerializeField] private TMP_InputField skillInput1;
    [SerializeField] private TMP_InputField skillInput2;
    [SerializeField] private Button confirmButton;
    
    private bool isSkill2OnCooldown = false;    // 쿨타임 체크 변수
    private Coroutine skill2CooldownCoroutine;
    
    // 스킬 프리펩
    private GameObject skillPrefab1;
    private GameObject skillPrefab2;
    
    // Skill2 실행 참조용 변수 (셋업 시 저장)
    private IEquipmentSkillBehaviour skillExcutor2;
    private EquipmentSkillData skillData2;
    
    private float lastTapTime = 0f;
    private const float doubleTapThreshold = 0.25f; // 더블탭 허용 간격 (초)
    
    private void Start()
    {
        var engineStatData = equipmentStatDataSo.GetEquipmentStatData(Base_Mng.Data.data.EQCount);
        
        // 스킬 아이디 확인 (0이면 스킬 없음)
        var skillId1 = engineStatData.Skill1;
        var skillId2 = engineStatData.Skill2;
        
        SetEngineSkills(skillId1, skillId2);
        
        // 확인 버튼에 이벤트 연결
        confirmButton.onClick.AddListener(() =>
        {
            OnConfirmSkillChange();
        });
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

    private void OnConfirmSkillChange()
    {
        int id1, id2;

        if (!int.TryParse(skillInput1.text, out id1)) id1 = 0;
        if (!int.TryParse(skillInput2.text, out id2)) id2 = 0;

        Debug.Log($"[SkillChange] Skill1: {id1}, Skill2: {id2}");

        SetEngineSkills(id1, id2); // 새로운 스킬로 교체

        isSkill2OnCooldown = false;
    }
    
    private void SetEngineSkills(int skillId1, int skillId2)
    {
        // 이전 스킬 오브젝트 제거
        if (skillPrefab1 != null) Destroy(skillPrefab1);
        if (skillPrefab2 != null) Destroy(skillPrefab2);
        
        skillExcutor2 = null;
        skillData2 = null;
        
        // 스킬1
        if (skillId1 != 0)
        {
            skillPrefab1 = Instantiate(skillTable.GetPrefabByKey(skillId1), player.transform);
            if (skillPrefab1 == null)
            {
                Debug.LogWarning($"[Skill1] 프리팹이 존재하지 않음! (ID={skillId1})");
            }
            else
            {
                var skillExcutor1 = skillPrefab1.GetComponent<IEquipmentSkillBehaviour>();
                var skillData1 = skillDataSo.GetEquipmentSkillData(skillId1);

                skillExcutor1.Excute(player, skillData1);
                Debug.Log($"[Skill1] Id: {skillId1} / Type: {skillData1.Type} / Cooldown: {skillData1.Cooldown} / Duration: {skillData1.Duration} / Param: {skillData1.Param}");
            }
        }
        else
        {
            Debug.Log("[Skill1] 스킬 없음 (ID=0)");
        }

        // 스킬2
        if (skillId2 != 0)
        {
            skillPrefab2 = Instantiate(skillTable.GetPrefabByKey(skillId2), player.transform);
            if (skillPrefab2 == null)
            {
                Debug.LogWarning($"[Skill2] 프리팹이 존재하지 않음! (ID={skillId1})");
            }
            else
            {
                skillExcutor2 = skillPrefab2.GetComponent<IEquipmentSkillBehaviour>();
                skillData2 = skillDataSo.GetEquipmentSkillData(skillId2);
            }
        }
        else
        {
            Debug.Log("[Skill2] 스킬 없음 (ID=0)");
        }
    }
    
    private void TryUseSkill2()
    {
        if (skillExcutor2 == null || skillData2 == null)
        {
            Debug.LogWarning("[Skill2] 스킬 데이터가 잘못되었거나 존재하지 않음!");
            return;
        }

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
