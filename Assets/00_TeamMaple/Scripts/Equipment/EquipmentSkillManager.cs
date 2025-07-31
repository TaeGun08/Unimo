using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class EquipmentSkillManager : MonoBehaviour
{
    public static EquipmentSkillManager Instance;
    
    // 플레이어 (유니모)
    [SerializeField] private GameObject player;
    
    // 엔진 & 스킬 데이터
    [SerializeField] private EquipmentStatDataSO equipmentStatDataSo;
    [SerializeField] private EquipmentSkillDataSO skillDataSo;
    
    // 스킬 테이블 & 프리펩
    [SerializeField] private PrefabsTable skillTable;
    private GameObject[] skillPrefabs = new GameObject[2];
    
    // 스킬 데이터/실행/쿨타임 등 관리
    private IEquipmentSkillBehaviour[] skillExecutors = new IEquipmentSkillBehaviour[2];
    private EquipmentSkillData[] skillDatas = new EquipmentSkillData[2];
    private int[] skillIds = new int[2];
    private Coroutine[] skillCooldownCoroutines = new Coroutine[2];
    private bool[] isSkillOnCooldown = new bool[2];
    
    // 컨트롤러 & 핸들러
    public EquipmentSkillUIController uiController;
    public EquipmentSkillEffectController effectController;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetSkillIds();
        SetSkills();
    }

    private void SetSkillIds()
    {
        // 스킬 Id 세팅
        var engineStatData = equipmentStatDataSo.GetEquipmentStatData(Base_Mng.Data.data.EQCount);
        
        skillIds[0] = engineStatData.Skill1;
        skillIds[1] = engineStatData.Skill2;
    }

    public void TestSetSkillIds(int[] ids)
    {
        for (int i = 0; i < ids.Length; i++)
        {
            skillIds[i] = ids[i];
        }
    }
    
    // 스킬 세팅
    public void SetSkills()
    {
        ResetSkills();

        // 스킬 프리팹 장착 및 사용
        for (int i = 0; i < skillIds.Length; i++)
        {
            if (skillIds[i] == 0)
            {
                uiController.ResetSkillSprite(i);
                Debug.Log($"[Skill{i}] 스킬 없음 (ID = 0)");
                continue;
            }
            
            uiController.SetSkillSprite(i, skillIds[i]);    // 스킬 Sprite 설정
            effectController.SetSkillEffects(i, skillIds[i]);    // 스킬 Effect 설정
            
            skillPrefabs[i] = Instantiate(skillTable.GetPrefabByKey(skillIds[i]), player.transform);
            
            if (skillPrefabs[i] == null)
            {
                Debug.LogWarning($"[Skill{i}] 프리팹이 존재하지 않음! (ID = {skillIds[i]})");
            }
            else
            {
                skillExecutors[i] = skillPrefabs[i].GetComponent<IEquipmentSkillBehaviour>();
                skillDatas[i] = skillDataSo.GetFinalEquipmentSkillData(skillIds[i], Base_Mng.Data.data.EQLevel[Base_Mng.Data.data.EQCount - 1]);

                // 패시브는 장착과 동시에 사용
                if (skillDatas[i].Type == EquipmentSkillType.Passive)
                {
                    UseSkill(i);
                    Debug.Log($"[[Skill{i}]]\n" +
                              $"Id: {skillDatas[i].Id}\n" +
                              $"Name: {skillDatas[i].Name}\n" +
                              $"Type: {skillDatas[i].Id}\n" +
                              $"Cooldown: {skillDatas[i].Id}\n" +
                              $"Duration: {skillDatas[i].Id}\n" +
                              $"Param: {skillDatas[i].Id}\n" +
                              $"Description: {skillDatas[i].Description}\n");
                }
            }
        }
    }

    // 스킬 사용
    public void UseSkill(int idx)
    {
        if (skillExecutors[idx] == null || skillDatas[idx] == null || isSkillOnCooldown[idx])
        {
            return;
        }
        
        skillExecutors[idx].Excute(player, skillDatas[idx]);
        effectController.PlaySkillEffect(idx);
        Debug.Log($"[[Skill{idx}]]\n" +
                  $"Id: {skillDatas[idx].Id}\n" +
                  $"Name: {skillDatas[idx].Name}\n" +
                  $"Type: {skillDatas[idx].Id}\n" +
                  $"Cooldown: {skillDatas[idx].Id}\n" +
                  $"Duration: {skillDatas[idx].Id}\n" +
                  $"Param: {skillDatas[idx].Id}\n" +
                  $"Description: {skillDatas[idx].Description}\n");

        if (skillDatas[idx].Type == EquipmentSkillType.Active)
        {
            StartSkillCooldown(idx, skillDatas[idx].Cooldown);
        }
    }

    // 스킬 쿨타임 시작
    public void StartSkillCooldown(int idx, float cooldown)
    {
        if (skillCooldownCoroutines[idx] != null)
        {
            StopCoroutine(skillCooldownCoroutines[idx]);
        }
        
        skillCooldownCoroutines[idx] = StartCoroutine(SkillCooldown(idx, cooldown));
    }

    // 스킬 쿨타임 코루틴
    private IEnumerator SkillCooldown(int idx, float cooldown)
    {
        isSkillOnCooldown[idx] = true;
        
        uiController.OnCooldownUI(idx, cooldown);    // UI 갱신 신호
        float timer = cooldown;
        while (timer > 0f)
        {
            uiController.UpdateCooldown(idx, timer / cooldown, Mathf.Ceil(timer));
            timer -= Time.deltaTime;
            yield return null;
        }
        uiController.OffCooldownUI(idx);
        
        isSkillOnCooldown[idx] = false;
    }

    // 스킬 세팅 초기화
    private void ResetSkills()
    {
        for (int i = 0; i < skillPrefabs.Length; i++)
        {
            // 쿨타임 코루틴 있으면 초기화
            if (skillCooldownCoroutines[i] != null)
            {
                StopCoroutine(skillCooldownCoroutines[i]);
            }
            
            // 쿨타임 초기화
            uiController.OffCooldownUI(i);
            isSkillOnCooldown[i] = false;
            
            // 이전 스킬 오브젝트 제거
            if (skillPrefabs[i] != null)
            {
                Destroy(skillPrefabs[i]);
            }
        
            // 스킬 세팅 초기화
            skillExecutors[i] = null;
            skillDatas[i] = null;
        }
        
        // 상태 초기화
        LocalPlayer.Instance.PlayerStatHolder.RemoveOnceInvalid();
        LocalPlayer.Instance.PlayerStatHolder.RemoveInvincible();
    }
}
