using System.Collections.Generic;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

public class UnimoStatUI : MonoBehaviour
{
    [SerializeField] private UnimoStatDataSO unimoStatDataSo;
    [SerializeField] private EquipmentStatDataSO equipmentStatDataSo;
    [SerializeField] private UnimoStatLevelUpDataSO unimoLevelDataSO;
    [SerializeField] private EquipmentStatLevelUpDataSO engineLevelDataSO;
    
    public StatCalculator StatCalculator { get; private set; }
    public GameObject statTextPrefab;
    
    [Header("Unimo Upgrade Settings")]
    public RectTransform contentParent;
    public RectTransform upgradeContentParent;
    [SerializeField] private TMP_Text Name;
    [SerializeField] private TMP_Text Level;

    [Header("Engine Upgrade Settings")]
    public RectTransform upgradeEngineContentParent;
    [SerializeField] private TMP_Text EQName;
    [SerializeField] private TMP_Text EQRank;
    [SerializeField] private TMP_Text EQLevel;

    private float lineSpacing = 30f;

    private UnimoStatData unimoStatData;
    private EquipmentStatData equipmentStatData;
    
    public void Start()
    {
        unimoStatData = unimoStatDataSo.GetFinalUnimoStatData(Base_Mng.Data.data.CharCount, Base_Mng.Data.data.CharLevel[Base_Mng.Data.data.CharCount - 1]);
        equipmentStatData = equipmentStatDataSo.GetFinalEquipmnetStatData(Base_Mng.Data.data.EQCount, Base_Mng.Data.data.EQLevel[Base_Mng.Data.data.EQCount - 1]);

        // Unimo Engine 최종 stat
        StatCalculator = new StatCalculator(unimoStatData, equipmentStatData);
        
        if (unimoStatData != null)
        {
            UpdateStatUI();
            UpgradeStatUI(unimoStatData);
            UpgradeEngineStatUI(equipmentStatData);
        }
    }

    // 최종 stat UI
    public void UpdateStatUI()
    {
        CreateStatLine($"체력 {StatCalculator.Hp}");
        CreateStatLine($"방어력 {StatCalculator.Def}");
        CreateStatLine($"이동속도 {StatCalculator.Speed}%");
        CreateStatLine($"개화 범위 {StatCalculator.BloomRange}");
        CreateStatLine($"개화 속도 {StatCalculator.BloomSpeed}%");
        CreateStatLine($"꽃 생성 주기 {StatCalculator.FlowerRate}%");
        CreateStatLine($"희귀 꽃 확률 {StatCalculator.RareFlowerRate}%");
        CreateStatLine($"회피율 {StatCalculator.Dodge}%");
        CreateStatLine($"스턴 회복력 {StatCalculator.StunRecovery}%");
        CreateStatLine($"체력 재생 {StatCalculator.HpRecovery}%");
        CreateStatLine($"꽃 낙하 속도 {StatCalculator.FlowerDropSpeed}%");
        CreateStatLine($"낙하량 증가 {StatCalculator.FlowerDropAmount}%");
    }
    
    // 유니모 강화 UI
    private void UpgradeStatUI(UnimoStatData data)
    {
        Name.text = data.Name;
        Level.text = Base_Mng.Data.data.CharLevel[Base_Mng.Data.data.CharCount - 1].ToString();
        
        UpGradeStatLine($"체력 {data.Hp}");
        UpGradeStatLine($"방어력 {data.Def}");
        UpGradeStatLine($"이동속도 {data.Speed}%");
        UpGradeStatLine($"개화 범위 {data.BloomRange}");
        UpGradeStatLine($"개화 속도 {data.BloomSpeed}%");
        UpGradeStatLine($"꽃 생성 주기 {data.FlowerRate}%");
        UpGradeStatLine($"희귀 꽃 확률 {data.RareFlowerRate}%");
        UpGradeStatLine($"회피율 {data.Dodge}%");
        UpGradeStatLine($"스턴 회복력 {data.StunRecovery}%");
        UpGradeStatLine($"체력 재생 {data.HpRecovery}%");
        UpGradeStatLine($"꽃 낙하 속도 {data.FlowerDropSpeed}%");
        UpGradeStatLine($"낙하량 증가 {data.FlowerDropAmount}%");

        // 다음 레벨 업그레이드 수치 가져오기
        ShowUpgradeStat(Base_Mng.Data.data.CharLevel[Base_Mng.Data.data.CharCount - 1]);
    }
    
    public void ShowUpgradeStat( int currentLevel)
    {
        var next = unimoLevelDataSO.GetCurrentAndNextStat(currentLevel);
        
        UpGradeNextStatLine($"+ {next.PlusHp}");
        UpGradeNextStatLine($"+ {next.PlusDef}");
        UpGradeNextStatLine($"+ {next.PlusSpeed}%");
        UpGradeNextStatLine($"+ {next.PlusBloomRange}");
        UpGradeNextStatLine($"+ {next.PlusBloomSpeed}%");
        UpGradeNextStatLine($"+ {next.PlusFlowerRate}%");
        UpGradeNextStatLine($"+ {next.PlusRareFlowerRate}%");
        UpGradeNextStatLine($"+ {next.PlusDodge}%");
        UpGradeNextStatLine($"+ {next.PlusStunRecovery}%");
        UpGradeNextStatLine($"+ {next.PlusHpRecovery}%");
        UpGradeNextStatLine($"+ {next.PlusFlowerDropSpeed}%");
        UpGradeNextStatLine($"+ {next.PlusFlowerDropAmount}%");
    }
    
    // 엔진 강화 UI
    private void UpgradeEngineStatUI(EquipmentStatData data)
    {
        EQName.text = data.Name;
        EQRank.text = data.Rank.ToString();
        EQLevel.text = Base_Mng.Data.data.EQLevel[Base_Mng.Data.data.EQCount - 1].ToString();
        
        ShowStatLine(data.StatType1, data.StatValue1);
        ShowStatLine(data.StatType2, data.StatValue2);
        ShowStatLine(data.StatType3, data.StatValue3);
        ShowStatLine(data.StatType4, data.StatValue4);
        
        // 다음 레벨 업그레이드 수치 가져오기
        ShowUpgradeEngineStat(Base_Mng.Data.data.EQLevel[Base_Mng.Data.data.EQCount - 1]);
    }
    
    public void ShowUpgradeEngineStat( int currentLevel)
    {
        var next = engineLevelDataSO.GetCurrentAndNextStat(equipmentStatData.Rank, currentLevel);
   
        UpGradeNextEngineStatLine(engineLevelDataSO.GetLevelUpValue(next, equipmentStatData.StatType1));
        UpGradeNextEngineStatLine(engineLevelDataSO.GetLevelUpValue(next, equipmentStatData.StatType2));
        UpGradeNextEngineStatLine(engineLevelDataSO.GetLevelUpValue(next, equipmentStatData.StatType3));
        UpGradeNextEngineStatLine(engineLevelDataSO.GetLevelUpValue(next, equipmentStatData.StatType4));
    }
    
    private void ShowStatLine(UnimoStat statType, float statValue)
    {
        if (statType == UnimoStat.None) return;

        string statName = GetStatDisplayName(statType);
        string valueText = $"{statValue:0.##}"; // 소수점 2자리까지

        UpGradeEngineStatLine($"{statName} : {valueText}");
    }
    
    private string GetStatDisplayName(UnimoStat stat)
    {
        return stat switch
        {
            UnimoStat.Hp => "체력",
            UnimoStat.Def => "방어력",
            UnimoStat.Speed => "이동속도",
            UnimoStat.BloomRange => "개화 범위",
            UnimoStat.BloomSpeed => "개화 속도",
            UnimoStat.FlowerRate => "꽃 생성 주기",
            UnimoStat.RareFlowerRate => "희귀 꽃 확률",
            UnimoStat.Dodge => "회피율",
            UnimoStat.StunRecovery => "스턴 회복력",
            UnimoStat.HpRecovery => "체력 재생",
            UnimoStat.FlowerDropSpeed => "꽃 낙하 속도",
            UnimoStat.FlowerDropAmount => "낙하량 증가",
            _ => stat.ToString()
        };
    }
    
    // ui line 조정
    private float currentY = 0;
    private float baseY  = 0;
    private float baseX = 0;
    
    private void CreateStatLine(string text)
    {
        GameObject statLine = Instantiate(statTextPrefab, contentParent, false);
        TMP_Text tmp = statLine.GetComponent<TMP_Text>();
        tmp.text = text;
        tmp.fontSize = 20;
        lineSpacing = 40;
        
        RectTransform rt = statLine.GetComponent<RectTransform>();
        
        if (baseY == 0)
        {
            baseY = rt.anchoredPosition.y;
            baseX = rt.anchoredPosition.x;
        }

        rt.anchoredPosition = new Vector2(baseX, baseY - currentY);
        currentY += lineSpacing;
    }

    private float upgradeCurrentY = 0f;
    private float upgradeBaseX = -440f;
    private float upgradeBaseY = 370f; 
    
    private void UpGradeStatLine(string text)
    {
        GameObject line = Instantiate(statTextPrefab, upgradeContentParent, false);
        TMP_Text tmp = line.GetComponent<TMP_Text>();
        tmp.text = text; 
        lineSpacing = 55;
        
        RectTransform rt = line.GetComponent<RectTransform>();

        rt.anchoredPosition = new Vector2(upgradeBaseX, upgradeBaseY - upgradeCurrentY);
        upgradeCurrentY += lineSpacing;
    }
    
    private float upgradeEQCurrentY = 0f;
    private float upgradeEQBaseX = -440f;
    private float upgradeEQBaseY = 370f; 
    
    private void UpGradeEngineStatLine(string text)
    {
        GameObject line = Instantiate(statTextPrefab, upgradeEngineContentParent, false);
        TMP_Text tmp = line.GetComponent<TMP_Text>();
        tmp.text = text; 
        lineSpacing = 55;
        
        RectTransform rt = line.GetComponent<RectTransform>();

        rt.anchoredPosition = new Vector2(upgradeEQBaseX, upgradeEQBaseY - upgradeEQCurrentY);
        upgradeEQCurrentY += lineSpacing;
    }
    
    private float upgradeNextCurrentY = 0f;
    private float upgradeNextBaseX = 40f;
    private float upgradeNextBaseY = 370f; 
    
    private void UpGradeNextStatLine(string text)
    {
        GameObject line = Instantiate(statTextPrefab, upgradeContentParent, false);
        TMP_Text tmp = line.GetComponent<TMP_Text>();
        tmp.text = text; 
        lineSpacing = 55;
        
        RectTransform rt = line.GetComponent<RectTransform>();
        
        rt.anchoredPosition = new Vector2(upgradeNextBaseX, upgradeNextBaseY - upgradeNextCurrentY);
        upgradeNextCurrentY += lineSpacing;
    }

    private float upgradeEngineNextCurrentY = 0f;
    private float upgradeEngineNextBaseX = 40f;
    private float upgradeEngineNextBaseY = 370f; 
    
    private void UpGradeNextEngineStatLine(float value)
    {
        if(value == 0) return;
        
        GameObject line = Instantiate(statTextPrefab, upgradeEngineContentParent, false);
        TMP_Text tmp = line.GetComponent<TMP_Text>();
        tmp.text = $"+ {value.ToString()}"; 
        lineSpacing = 55;
        
        RectTransform rt = line.GetComponent<RectTransform>();
        
        rt.anchoredPosition = new Vector2(upgradeEngineNextBaseX, upgradeEngineNextBaseY - upgradeEngineNextCurrentY);
        upgradeEngineNextCurrentY += lineSpacing;
    }
    
    // ui 리로드
    public void RefreshUI()
    {
        // 기존 스탯 UI 삭제
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // 기존 업그레이드 UI 삭제
        foreach (Transform child in upgradeContentParent)
            Destroy(child.gameObject);
        
        foreach (Transform child in upgradeEngineContentParent)
            Destroy(child.gameObject);
        
        // 위치 초기화
        currentY = 0;
        baseY = 0;

        upgradeCurrentY = 0;
        upgradeBaseY = 370f;  
        upgradeBaseX = -440f;
        
        upgradeEQCurrentY = 0f;
        upgradeEQBaseX = -440f;
        upgradeEQBaseY = 370f; 
        
        upgradeNextCurrentY = 0f;
        upgradeNextBaseX = 40f;
        upgradeNextBaseY = 370f; 
        
        unimoStatData = unimoStatDataSo.GetFinalUnimoStatData(Base_Mng.Data.data.CharCount, Base_Mng.Data.data.CharLevel[Base_Mng.Data.data.CharCount - 1]);
        equipmentStatData = equipmentStatDataSo.GetFinalEquipmnetStatData(Base_Mng.Data.data.EQCount, Base_Mng.Data.data.EQLevel[Base_Mng.Data.data.EQCount - 1]);
        
        if (unimoStatData != null)
        {
            UpdateStatUI();
            UpgradeStatUI(unimoStatData);
            UpgradeEngineStatUI(equipmentStatData);
        }
    }
    
    // 유니모 레벨업
    public void UpgradeUnimoAndStatUI(int type)
    {
        Base_Mng.instance.UpgradeUnimoLevel(type);
        RefreshUI();
    }
    
    // 엔진 레벨업
    public void UpgradeEngineAndStatUI()
    {
        if (Base_Mng.Data.data.EQLevel[Base_Mng.Data.data.EQCount - 1] >= 5)
        {
            Debug.Log("최대 레벨입니다.");
            return;
        }
        
        Base_Mng.instance.UpgradeEngineLevel();
        RefreshUI();
    }
}
