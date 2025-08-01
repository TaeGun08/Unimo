using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class EquipmentSkillManager : MonoBehaviour
{
    public static EquipmentSkillManager Instance;
    
    // �÷��̾� (���ϸ�)
    [SerializeField] private GameObject player;
    
    // ���� & ��ų ������
    [SerializeField] private EquipmentStatDataSO equipmentStatDataSo;
    [SerializeField] private EquipmentSkillDataSO skillDataSo;
    
    // ��ų ���̺� & ������
    [SerializeField] private PrefabsTable skillTable;
    private GameObject[] skillPrefabs = new GameObject[2];
    
    // ��ų ������/����/��Ÿ�� �� ����
    private IEquipmentSkillBehaviour[] skillExecutors = new IEquipmentSkillBehaviour[2];
    private EquipmentSkillData[] skillDatas = new EquipmentSkillData[2];
    private int[] skillIds = new int[2];
    private Coroutine[] skillCooldownCoroutines = new Coroutine[2];
    private bool[] isSkillOnCooldown = new bool[2];
    
    // ��Ʈ�ѷ� & �ڵ鷯
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
        // ��ų Id ����
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
    
    // ��ų ����
    public void SetSkills()
    {
        ResetSkills();

        // ��ų ������ ���� �� ���
        for (int i = 0; i < skillIds.Length; i++)
        {
            if (skillIds[i] == 0)
            {
                uiController.ResetSkillSprite(i);
                Debug.Log($"[Skill{i}] ��ų ���� (ID = 0)");
                continue;
            }
            
            uiController.SetSkillSprite(i, skillIds[i]);    // ��ų Sprite ����
            effectController.SetSkillEffects(i, skillIds[i]);    // ��ų Effect ����
            
            skillPrefabs[i] = Instantiate(skillTable.GetPrefabByKey(skillIds[i]), player.transform);
            
            if (skillPrefabs[i] == null)
            {
                Debug.LogWarning($"[Skill{i}] �������� �������� ����! (ID = {skillIds[i]})");
            }
            else
            {
                skillExecutors[i] = skillPrefabs[i].GetComponent<IEquipmentSkillBehaviour>();
                skillDatas[i] = skillDataSo.GetFinalEquipmentSkillData(skillIds[i], Base_Mng.Data.data.EQLevel[Base_Mng.Data.data.EQCount - 1]);

                // �нú�� ������ ���ÿ� ���
                if (skillDatas[i].Type == EquipmentSkillType.Passive)
                {
                    UseSkill(i);
                }
            }
        }
    }

    // ��ų ���
    public void UseSkill(int idx)
    {
        if (skillExecutors[idx] == null || skillDatas[idx] == null || isSkillOnCooldown[idx])
        {
            return;
        }
        
        skillExecutors[idx].Excute(player, skillDatas[idx]);
        Debug.Log($"[[Skill{idx}]]\n" +
                  $"Id: {skillDatas[idx].Id}\n" +
                  $"Name: {skillDatas[idx].Name}\n" +
                  $"Type: {skillDatas[idx].Type}\n" +
                  $"Cooldown: {skillDatas[idx].Cooldown}\n" +
                  $"Duration: {skillDatas[idx].Duration}\n" +
                  $"Param: {skillDatas[idx].Param}\n" +
                  $"Description: {skillDatas[idx].Description}\n");

        if (skillDatas[idx].Type == EquipmentSkillType.Active)
        {
            StartSkillCooldown(idx, skillDatas[idx].Cooldown);
        }
    }

    // ��ų ��Ÿ�� ����
    public void StartSkillCooldown(int idx, float cooldown)
    {
        if (skillCooldownCoroutines[idx] != null)
        {
            StopCoroutine(skillCooldownCoroutines[idx]);
        }
        
        skillCooldownCoroutines[idx] = StartCoroutine(SkillCooldown(idx, cooldown));
    }

    // ��ų ��Ÿ�� �ڷ�ƾ
    private IEnumerator SkillCooldown(int idx, float cooldown)
    {
        isSkillOnCooldown[idx] = true;
        
        uiController.OnCooldownUI(idx, cooldown);    // UI ���� ��ȣ
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

    // ��ų ���� �ʱ�ȭ
    private void ResetSkills()
    {
        for (int i = 0; i < skillPrefabs.Length; i++)
        {
            // ��Ÿ�� �ڷ�ƾ ������ �ʱ�ȭ
            if (skillCooldownCoroutines[i] != null)
            {
                StopCoroutine(skillCooldownCoroutines[i]);
            }
            
            // ��Ÿ�� �ʱ�ȭ
            uiController.OffCooldownUI(i);
            isSkillOnCooldown[i] = false;
            
            // ���� ��ų ������Ʈ ����
            if (skillPrefabs[i] != null)
            {
                Destroy(skillPrefabs[i]);
            }
        
            // ��ų ���� �ʱ�ȭ
            skillExecutors[i] = null;
            skillDatas[i] = null;
            
            // ��ų ����Ʈ �ʱ�ȭ
            effectController.StopSkillEffect(i);
        }
        
        // ���� �ʱ�ȭ
        LocalPlayer.Instance.PlayerStatHolder.RemoveOnceInvalid();
        LocalPlayer.Instance.PlayerStatHolder.RemoveInvincible();
    }
}
