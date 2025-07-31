using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSkillInputHandler : MonoBehaviour
{
    // ��Ƽ�� ��ų ��� ��ư
    [SerializeField] private Button activeSkillButton;
    
    // �׽�Ʈ�� ��ǲ �ʵ�
    [SerializeField] private TMP_InputField[] skillInputs;
    [SerializeField] private Button confirmButton;
    
    // ��Ʈ�ѷ� & �Ŵ���
    [SerializeField] private EquipmentSkillManager equipmentSkillManager;
    
    // Ŭ�� ���� ����
    private bool canClick = true;
    
    // ���� �� ����
    private float lastTapTime = 0f;
    private const float doubleTapThreshold = 0.25f;    // ���� �� ��� ���� (��)
    
    private void Start()
    {
        activeSkillButton.onClick.AddListener(() =>
        {
            equipmentSkillManager.UseSkill(1);
        });
        
        PlaySystemRefStorage.playProcessController.SubscribePauseAction(StopClick);
        PlaySystemRefStorage.playProcessController.SubscribeGameoverAction(StopClick);
        PlaySystemRefStorage.playProcessController.SubscribeResumeAction(StartClick);
        
        // �׽�Ʈ�� ��ǲ �ʵ� Ȯ�� ��ư
        confirmButton.onClick.AddListener(() =>
        {
            OnConfirmSkillChange();
        });
    }
    
    private void Update()
    {
#if UNITY_EDITOR
        // �����Ϳ����� ���콺 ����Ŭ���� ����
        if (Input.GetMouseButtonDown(0) && canClick)
        {
            float currentTime = Time.time;
            if (currentTime - lastTapTime < doubleTapThreshold)
            {
                equipmentSkillManager.UseSkill(1);
            }
            lastTapTime = currentTime;
        }
#else
        // �����: �� �հ��� ������ ����
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                float currentTime = Time.time;
                if (currentTime - lastTapTime < doubleTapThreshold)
                {
                    equipmentSkillManager.UseSkill(1);
                }
                lastTapTime = currentTime;
            }
        }
#endif
    }
    
    private void StopClick()
    {
        canClick = false;
    }
    private void StartClick()
    {
        canClick = true;
    }
    
    private void OnConfirmSkillChange()
    {
        int[] ids = new int[skillInputs.Length];

        for (int i = 0; i < ids.Length; i++)
        {
            if (!int.TryParse(skillInputs[i].text, out ids[i]))
            {
                ids[i] = 0;
            }
            
            Debug.Log($"[SkillChange] Skill{i}: {ids[i]}");
        }

        equipmentSkillManager.TestSetSkillIds(ids);
        LocalPlayer.Instance.TestSetPlayerStats();
    }
}
