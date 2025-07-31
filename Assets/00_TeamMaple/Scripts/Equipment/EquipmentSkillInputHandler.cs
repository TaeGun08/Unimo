using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSkillInputHandler : MonoBehaviour
{
    // 액티브 스킬 사용 버튼
    [SerializeField] private Button activeSkillButton;
    
    // 테스트용 인풋 필드
    [SerializeField] private TMP_InputField[] skillInputs;
    [SerializeField] private Button confirmButton;
    
    // 컨트롤러 & 매니저
    [SerializeField] private EquipmentSkillManager equipmentSkillManager;
    
    // 클릭 가능 여부
    private bool canClick = true;
    
    // 더블 탭 세팅
    private float lastTapTime = 0f;
    private const float doubleTapThreshold = 0.25f;    // 더블 탭 허용 간격 (초)
    
    private void Start()
    {
        activeSkillButton.onClick.AddListener(() =>
        {
            equipmentSkillManager.UseSkill(1);
        });
        
        PlaySystemRefStorage.playProcessController.SubscribePauseAction(StopClick);
        PlaySystemRefStorage.playProcessController.SubscribeGameoverAction(StopClick);
        PlaySystemRefStorage.playProcessController.SubscribeResumeAction(StartClick);
        
        // 테스트용 인풋 필드 확인 버튼
        confirmButton.onClick.AddListener(() =>
        {
            OnConfirmSkillChange();
        });
    }
    
    private void Update()
    {
#if UNITY_EDITOR
        // 에디터에서는 마우스 더블클릭도 지원
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
        // 모바일: 한 손가락 더블탭 감지
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
