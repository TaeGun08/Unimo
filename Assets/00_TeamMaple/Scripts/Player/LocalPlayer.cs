using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LocalPlayer : MonoBehaviour, IDamageAble
{
    public static LocalPlayer Instance { get; private set; }
    
    [Header("UnimoStatDataSO")]
    [SerializeField] private PrefabsTable unimoTable;
    [SerializeField] private UnimoStatDataSO unimoStatDataSO;
    [SerializeField] private UnimoStatLevelUpDataSO unimoStatLevelUpDataSO;
    
    [Header("EquipmentStatDataSO")]
    [SerializeField] private PrefabsTable equipmentTable;
    [SerializeField] private EquipmentStatDataSO equipmentStatDataSO;
    [SerializeField] private EquipmentStatLevelUpDataSO equipmentStatLevelUpDataSO;

    [Header("EquipmentSkillDataSO")]
    [SerializeField] private PrefabsTable equipmentSkillTable;
    [SerializeField] private EquipmentSkillDataSO equipmentSkillDataSO;
    [SerializeField] private EquipmentSkillLevelUpDataSO equipmentSkillLevelUpDataSO;
    
    public UnimoStatData UnimoStatData;
    private UnimoStatLevelUpData UnimoStatLevelUpData;
    private EquipmentStatData EquipmentStatData;
    private EquipmentStatLevelUpData EquipmentStatLevelUpData;
    private EquipmentSkillLevelUpData EquipmentSkillLevelUpData;
    
    public StatCalculator StatCalculator;
    
    private PlayerController playerController;
    public Vector3 LastAttackerPos { get; private set; }

    public TMP_Text RemainHp; 
    public int CurMaxHp {get; set;}

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
        
        UnimoStatData = unimoStatDataSO.SettingsUnimoData(Base_Mng.Data.data.CharCount);

        var unimo = unimoTable.GetPrefabByKey(Base_Mng.Data.data.CharCount);
        var engine = equipmentTable.GetPrefabByKey(Base_Mng.Data.data.EQCount);
        
        Debug.Log($"UnimoData:: {UnimoStatData.Level},  {UnimoStatData.Name}, {UnimoStatData.FlowerRate}");
        
        SetPlayerAnimator(unimo,  engine);
        CurMaxHp = UnimoStatData.Hp;
    }

    private void Start()
    {
        SetPlayerStats();

        Debug.Log($"[UnimoStats]\n" +
                  $"Hp: {StatCalculator.UnimoStatData.Hp}\n" +
                  $"Def: {StatCalculator.UnimoStatData.Def}\n" +
                  $"Speed: {StatCalculator.UnimoStatData.Speed}\n" +
                  $"BloomRange: {StatCalculator.UnimoStatData.BloomRange}\n" +
                  $"BloomSpeed: {StatCalculator.UnimoStatData.BloomSpeed}\n" +
                  $"FlowerRate: {StatCalculator.UnimoStatData.FlowerRate}\n" +
                  $"RareFlowerRate: {StatCalculator.UnimoStatData.RareFlowerRate}\n" +
                  $"Dodge: {StatCalculator.UnimoStatData.Dodge}\n" +
                  $"StunRecovery: {StatCalculator.UnimoStatData.StunRecovery}\n" +
                  $"HpRecovery: {StatCalculator.UnimoStatData.HpRecovery}\n" +
                  $"FlowerDropSpeed: {StatCalculator.UnimoStatData.FlowerDropSpeed}\n" +
                  $"FlowerDropAmount: {StatCalculator.UnimoStatData.FlowerDropAmount}");
    }

    private void Update()
    {
        RemainHp.text = UnimoStatData.Hp.ToString();
    }

    // StatCalculator의 UnimoStatData에 값 넣어주기
    private void SetPlayerStats()
    {
        UnimoStatLevelUpData = unimoStatLevelUpDataSO.GetUnimoStatLevelUpData(UnimoStatData.Level);
        EquipmentStatData = equipmentStatDataSO.GetEquipmentStatData(1);    // 착용 중인 엔진 아이디 넣어야 함
        EquipmentStatLevelUpData = equipmentStatLevelUpDataSO.GetEquipmentStatLevelUpData(EquipmentStatData.Rank, 1);    // 테이블에 레벨 0도 추가해야 할듯
        EquipmentSkillLevelUpData = equipmentSkillLevelUpDataSO.GetEquipmentSkillLevelUpData(2001, 0);    // 착용 중인 엔진의 스킬 아이디 사용해야함

        StatCalculator = new StatCalculator(
            UnimoStatData,
            UnimoStatLevelUpData,
            EquipmentStatData,
            EquipmentStatLevelUpData,
            EquipmentSkillLevelUpData);
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

    public void TakeDamage(Vector3 attackerPos)
    {
        LastAttackerPos = attackerPos;
        playerController.ChangeState(IPlayerState.EState.Hit);
    }
}