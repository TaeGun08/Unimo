using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class LocalPlayer : MonoBehaviour, IDamageAble
{
    public static LocalPlayer Instance { get; private set; }

    public GameObject GameObject => gameObject;
    
    [Header("UnimoStatDataSO")]
    [SerializeField] private PrefabsTable unimoTable;
    [SerializeField] private UnimoStatDataSO unimoStatDataSo;
    
    [Header("EquipmentStatDataSO")]
    [SerializeField] private PrefabsTable equipmentTable;
    [SerializeField] private EquipmentStatDataSO equipmentStatDataSo;

    [Header("EquipmentSkillDataSO")]
    [SerializeField] private PrefabsTable equipmentSkillTable;
    [SerializeField] private EquipmentSkillDataSO equipmentSkillDataSo;
    
    private UnimoStatData unimoStatData;
    private UnimoStatLevelUpData unimoStatLevelUpData;
    private EquipmentStatData equipmentStatData;
    private EquipmentStatLevelUpData equipmentStatLevelUpData;
    private EquipmentSkillLevelUpData equipmentSkillLevelUpData;
    
    private bool isSlippery = false;
    
    public StatCalculator StatCalculator { get; private set; }
    public PlayerStatHolder PlayerStatHolder { get; private set; }

    private PlayerController playerController;
    public Vector3 LastAttackerPos { get; private set; }

    // [SerializeField] private TMP_Text remainHp;
    // public TMP_Text RemainHp => remainHp;
    public TMP_Text RemainHp;
    
    // --------------테스트--------------
    [SerializeField] private Button unimoLevelDownButton;
    [SerializeField] private Button unimoLevelUpButton;
    [SerializeField] private Button equipmentLevelDownButton;
    [SerializeField] private Button equipmentLevelUpButton;
    
    [SerializeField] private TMP_Text unimoLevelText;
    [SerializeField] private TMP_Text equipmentLevelText;
    
    [SerializeField] private SkillRunner skillRunner;
    [SerializeField] private AuraController auraController;
    // --------------테스트--------------
    
    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        
        Instance = this;
        
        playerController = GetComponent<PlayerController>();
        
        // 유니모 데이터 셋팅 
        //UnimoData = unimoStatData.CreateDefaultUnimo();
        
        // 장착된 유니모 체크 및 생성
        Debug.Log($"LocalPlayer CharCount:: {Base_Mng.Data.data.CharCount}");
        Debug.Log($"LocalPlayer EQCount:: {Base_Mng.Data.data.EQCount}");

        var unimo = unimoTable.GetPrefabByKey(Base_Mng.Data.data.CharCount);
        var engine = equipmentTable.GetPrefabByKey(Base_Mng.Data.data.EQCount);
        
        // 플레이어 스탯 계산기 설정
        SetPlayerStats();
        
        // 플레이어 애니메이터 설정
        SetPlayerAnimator(unimo,  engine);
        
        // 테스트 세팅
        SetLevelTestButton();
        SetLevelTestText();
    }

    private void Start()
    {
        Debug.Log($"[UnimoStats]\n" +
                  $"Hp: {StatCalculator.Hp}\n" +
                  $"Def: {StatCalculator.Def}\n" +
                  $"Speed: {StatCalculator.Speed}\n" +
                  $"BloomRange: {StatCalculator.BloomRange}\n" +
                  $"BloomSpeed: {StatCalculator.BloomSpeed}\n" +
                  $"FlowerRate: {StatCalculator.FlowerRate}\n" +
                  $"RareFlowerRate: {StatCalculator.RareFlowerRate}\n" +
                  $"Dodge: {StatCalculator.Dodge}\n" +
                  $"StunRecovery: {StatCalculator.StunRecovery}\n" +
                  $"HpRecovery: {StatCalculator.HpRecovery}\n" +
                  $"FlowerDropSpeed: {StatCalculator.FlowerDropSpeed}\n" +
                  $"FlowerDropAmount: {StatCalculator.FlowerDropAmount}");

        // 10초마다 체력 회복
        StartCoroutine(HpRecoveryCoroutine(PlayerStatHolder.HpRecovery.Value));
    }

    private void Update()
    {
        if (RemainHp == null) return;
        RemainHp.text = PlayerStatHolder.Hp.Value.ToString();
    }

    // StatCalculator의 UnimoStatData에 값 넣어주기
    private void SetPlayerStats()
    {
        // 유니모, 엔진 아이디 & 레벨 넣어주기
        unimoStatData = unimoStatDataSo.GetFinalUnimoStatData(Base_Mng.Data.data.CharCount, Base_Mng.Data.data.CharLevel[Base_Mng.Data.data.CharCount - 1]);
        equipmentStatData = equipmentStatDataSo.GetFinalEquipmnetStatData(Base_Mng.Data.data.EQCount, Base_Mng.Data.data.EQLevel[Base_Mng.Data.data.EQCount - 1]);

        StatCalculator = new StatCalculator(unimoStatData, equipmentStatData);
        PlayerStatHolder = new PlayerStatHolder(StatCalculator);
    }
    
    private void SetPlayerAnimator(GameObject unimo, GameObject engine)
    {
        playerController.UnimoAnim = InstantiateAnimator(unimo, transform.position + Vector3.up);
        playerController.EgineAnim = InstantiateAnimator(engine, transform.position);
    }

    private Animator InstantiateAnimator(GameObject prefab, Vector3 position)
    {
        return Instantiate(prefab, position, Quaternion.Euler(0, 180f, 0), transform)
            .GetComponent<Animator>();
    }

    public void TakeDamage(CombatEvent e)
    {
        LastAttackerPos = e.Position;
        playerController.ChangeState(IPlayerState.EState.Hit);
    }
    
    private IEnumerator HpRecoveryCoroutine(float percentPerSec)
    {
        var maxHp = StatCalculator.Hp;    // 정해진 Hp 최대값
        var hp = PlayerStatHolder.Hp;    // 변경 가능한 Hp 값

        while (true)
        {
            int baseHp = maxHp;
            int healAmount = Mathf.Max(1, Mathf.RoundToInt(baseHp * percentPerSec));    // 초당 회복할 체력
            hp.Add(healAmount);    // 체력 증가
            
            yield return new WaitForSeconds(10f);
        }
    }
    
    // -------------------테스트----------------------
    
    private void TestSetPlayerStats()
    {
        StopCoroutine(HpRecoveryCoroutine(PlayerStatHolder.HpRecovery.Value));
        SetPlayerStats();
        StartCoroutine(HpRecoveryCoroutine(PlayerStatHolder.HpRecovery.Value));
        skillRunner.SetEngineSkills(0, 0);     // 보유 스킬 제거
        auraController.InitAura();
        
        Debug.Log($"[UnimoStats]\n" +
                  $"Hp: {StatCalculator.Hp}\n" +
                  $"Def: {StatCalculator.Def}\n" +
                  $"Speed: {StatCalculator.Speed}\n" +
                  $"BloomRange: {StatCalculator.BloomRange}\n" +
                  $"BloomSpeed: {StatCalculator.BloomSpeed}\n" +
                  $"FlowerRate: {StatCalculator.FlowerRate}\n" +
                  $"RareFlowerRate: {StatCalculator.RareFlowerRate}\n" +
                  $"Dodge: {StatCalculator.Dodge}\n" +
                  $"StunRecovery: {StatCalculator.StunRecovery}\n" +
                  $"HpRecovery: {StatCalculator.HpRecovery}\n" +
                  $"FlowerDropSpeed: {StatCalculator.FlowerDropSpeed}\n" +
                  $"FlowerDropAmount: {StatCalculator.FlowerDropAmount}");
    }

    private void SetLevelTestButton()
    {
        unimoLevelDownButton.onClick.AddListener(() =>
        {
            if (Base_Mng.Data.data.CharLevel[Base_Mng.Data.data.CharCount - 1] == 1)
            {
                Debug.Log($"유니모 레벨이 최소치입니다.");
                return;
            }
            Base_Mng.instance.DowngradeUnimoLevel();
            TestSetPlayerStats();
            SetLevelTestText();
        });
        
        unimoLevelUpButton.onClick.AddListener(() =>
        {
            if (Base_Mng.Data.data.CharLevel[Base_Mng.Data.data.CharCount - 1] == 100)
            {
                Debug.Log($"유니모 레벨이 최대치입니다.");
                return;
            }
            Base_Mng.instance.UpgradeUnimoLevel();
            TestSetPlayerStats();
            SetLevelTestText();
        });
        
        equipmentLevelDownButton.onClick.AddListener(() =>
        {
            if (Base_Mng.Data.data.EQLevel[Base_Mng.Data.data.EQCount - 1] == 0)
            {
                Debug.Log($"엔진 레벨이 최소치입니다.");
                return;
            }
            Base_Mng.instance.DowngradeEngineLevel();
            TestSetPlayerStats();
            SetLevelTestText();
        });
        
        equipmentLevelUpButton.onClick.AddListener(() =>
        {
            if (Base_Mng.Data.data.EQLevel[Base_Mng.Data.data.EQCount - 1] == 5)
            {
                Debug.Log($"엔진 레벨이 최대치입니다.");
                return;
            }
            Base_Mng.instance.UpgradeEngineLevel();
            TestSetPlayerStats();
            SetLevelTestText();
        });
    }

    private void SetLevelTestText()
    {
        unimoLevelText.text = "Lv. " + Base_Mng.Data.data.CharLevel[Base_Mng.Data.data.CharCount - 1];
        equipmentLevelText.text = "Lv. " + Base_Mng.Data.data.EQLevel[Base_Mng.Data.data.EQCount - 1];
    }
}