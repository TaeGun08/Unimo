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
    
    [Header("EquipmentStatDataSO")]
    [SerializeField] private PrefabsTable equipmentTable;
    [SerializeField] private EquipmentStatDataSO equipmentStatDataSO;

    [Header("EquipmentSkillDataSO")]
    [SerializeField] private PrefabsTable equipmentSkillTable;
    [SerializeField] private EquipmentSkillDataSO equipmentSkillDataSO;
    
    private UnimoStatData unimoStatData;
    private UnimoStatLevelUpData unimoStatLevelUpData;
    private EquipmentStatData equipmentStatData;
    private EquipmentStatLevelUpData equipmentStatLevelUpData;
    private EquipmentSkillLevelUpData equipmentSkillLevelUpData;

    public StatCalculator StatCalculator { get; private set; }
    public PlayerStatHolder PlayerStatHolder { get; private set; }

    private PlayerController playerController;
    public Vector3 LastAttackerPos { get; private set; }

    // [SerializeField] private TMP_Text remainHp;
    // public TMP_Text RemainHp => remainHp;
    public TMP_Text RemainHp;
    
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
    }

    private void Update()
    {
        RemainHp.text = PlayerStatHolder.Hp.Value.ToString();
    }

    // StatCalculator의 UnimoStatData에 값 넣어주기
    private void SetPlayerStats()
    {
        unimoStatData = unimoStatDataSO.GetFinalUnimoStatData(Base_Mng.Data.data.CharCount);
        equipmentStatData = equipmentStatDataSO.GetFinalEquipmnetStatData(Base_Mng.Data.data.EQCount);    // 착용 중인 엔진 아이디 넣어야 함

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

    public void TakeDamage(Vector3 attackerPos)
    {
        LastAttackerPos = attackerPos;
        playerController.ChangeState(IPlayerState.EState.Hit);
    }
}