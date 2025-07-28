using System;
using System.Collections.Generic;
using TMPro;
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

    [SerializeField] private TMP_Text CurrentLevel;
    [SerializeField] private TMP_Text NextLevel;
    [SerializeField] private TMP_Text Ability;
    
    [Header("Engine Upgrade Settings")]
    public RectTransform upgradeEngineContentParent;
    [SerializeField] private TMP_Text EQName;
    [SerializeField] private TMP_Text EQRank;
    [SerializeField] private TMP_Text EQLevel;
    [SerializeField] private TMP_Text EQAbility;
    
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
        
        Debug.Log(unimoStatData.SpecialStat1);
    }

    // 최종 stat UI
    public void UpdateStatUI()
    {
        foreach (var (stat, getter) in StatCalculator._finalStats)
        {
            CreateStatLine($"{stat.Ko()} {stat.Format(getter(StatCalculator))}");
        }
    }
    
    // 유니모 강화 UI
    private void UpgradeStatUI(UnimoStatData data)
    {
        Name.text = data.Name;
        Level.text = Base_Mng.Data.data.CharLevel[Base_Mng.Data.data.CharCount - 1].ToString();
        CurrentLevel.text = $"Lv. { Base_Mng.Data.data.CharLevel[Base_Mng.Data.data.CharCount - 1]}";
        NextLevel.text = $"Lv. { Base_Mng.Data.data.CharLevel[Base_Mng.Data.data.CharCount - 1] + 1} ";
        Ability.text = BuildSpecialAbilityText(data);
        
        foreach (var (stat, getter) in StatCalculator._unimoStats)
        {
            UpGradeStatLine($"{stat.Ko()} {stat.Format(getter(data))}");
        }

        // 다음 레벨 업그레이드 수치 가져오기
        ShowUpgradeStat(Base_Mng.Data.data.CharLevel[Base_Mng.Data.data.CharCount - 1]);
    }
    
    private string BuildSpecialAbilityText(UnimoStatData data)
    {
        var parts = new List<string>();

        void Add(UnimoStat stat)
        {
            if (stat == UnimoStat.None) return;
            parts.Add($"{stat.Ko()}");
        }

        Add(data.SpecialStat1);
        Add(data.SpecialStat2);
        Add(data.SpecialStat3);

        return "특화 능력치 : " + (parts.Count > 0 ? string.Join(" / ", parts) : "없음");
    }
    
    public void ShowUpgradeStat(int currentLevel)
    {
        var next = unimoLevelDataSO.GetCurrentAndNextStat(currentLevel);
        foreach (var (stat, getter) in StatCalculator._unimoNextStats)
        {
            var v = getter(next);
            UpGradeNextStatLine($"+ {stat.Format(v)}");
        }
    }
    
    // 엔진 강화 UI
    private void UpgradeEngineStatUI(EquipmentStatData data)
    {
        EQName.text = data.Name;
        EQRank.text = data.Rank.ToString();
        EQLevel.text = Base_Mng.Data.data.EQLevel[Base_Mng.Data.data.EQCount - 1].ToString();
        
        EQAbility.text = BuildSpecialEQAbilityText(data);
        
        ShowStatLine(data.StatType1, data.StatValue1);
        ShowStatLine(data.StatType2, data.StatValue2);
        ShowStatLine(data.StatType3, data.StatValue3);
        ShowStatLine(data.StatType4, data.StatValue4);
        
        // 다음 레벨 업그레이드 수치 가져오기
        ShowUpgradeEngineStat(Base_Mng.Data.data.EQLevel[Base_Mng.Data.data.EQCount - 1]);
    }
    
    private string BuildSpecialEQAbilityText(EquipmentStatData data)
    {
        var parts = new List<string>();

        void Add(UnimoStat stat)
        {
            if (stat == UnimoStat.None) return;
            parts.Add($"{stat.Ko()}");
        }

        Add(data.StatType1);
        Add(data.StatType2);
        Add(data.StatType3);

        return "특화 능력치 : " + (parts.Count > 0 ? string.Join(" / ", parts) : "없음");
    }
    
    public void ShowUpgradeEngineStat(int currentLevel)
    {
        if (currentLevel == 5) return;
        var next = engineLevelDataSO.GetCurrentAndNextStat(equipmentStatData.Rank, currentLevel);
        
        UpGradeNextEngineStatLine(engineLevelDataSO.GetLevelUpValue(next, equipmentStatData.StatType1));
        UpGradeNextEngineStatLine(engineLevelDataSO.GetLevelUpValue(next, equipmentStatData.StatType2));
        UpGradeNextEngineStatLine(engineLevelDataSO.GetLevelUpValue(next, equipmentStatData.StatType3));
        UpGradeNextEngineStatLine(engineLevelDataSO.GetLevelUpValue(next, equipmentStatData.StatType4));
    }
    
    private void ShowStatLine(UnimoStat statType, float statValue)
    {
        if (statType == UnimoStat.None) return;
        UpGradeEngineStatLine($"{statType.Ko()} : {statType.Format(statValue)}");
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
        tmp.text = $"{text:F2}";
        lineSpacing = 55;
        
        RectTransform rt = line.GetComponent<RectTransform>();

        rt.anchoredPosition = new Vector2(upgradeBaseX, upgradeBaseY - upgradeCurrentY);
        upgradeCurrentY += lineSpacing;
    }
    
    private float upgradeEQCurrentY = 0f;
    private float upgradeEQBaseX = -440f;
    private float upgradeEQBaseY = 90f; 
    
    private void UpGradeEngineStatLine(string text)
    {
        GameObject line = Instantiate(statTextPrefab, upgradeEngineContentParent, false);
        TMP_Text tmp = line.GetComponent<TMP_Text>();
        tmp.text = $"{text:F2}"; 
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
        tmp.text = $"{text:F2}"; 
        lineSpacing = 55;
        
        RectTransform rt = line.GetComponent<RectTransform>();
        
        rt.anchoredPosition = new Vector2(upgradeNextBaseX, upgradeNextBaseY - upgradeNextCurrentY);
        upgradeNextCurrentY += lineSpacing;
    }

    private float upgradeEngineNextCurrentY = 0f;
    private float upgradeEngineNextBaseX = 40f;
    private float upgradeEngineNextBaseY = 90f; 
    
    private void UpGradeNextEngineStatLine(float value)
    {
        if(value == 0) return;
        
        GameObject line = Instantiate(statTextPrefab, upgradeEngineContentParent, false);
        TMP_Text tmp = line.GetComponent<TMP_Text>();
        tmp.text = $"{value:F2}"; 
        //tmp.text = $"+ {value.ToString()}"; 
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
        upgradeBaseX = -440f;
        upgradeBaseY = 370f;  
        
        upgradeEQCurrentY = 0f;
        upgradeEQBaseX = -440f;
        upgradeEQBaseY = 90f; 
        
        upgradeNextCurrentY = 0f;
        upgradeNextBaseX = 40f;
        upgradeNextBaseY = 370f; 
        
        upgradeEngineNextCurrentY = 0f;
        upgradeEngineNextBaseX = 40f;
        upgradeEngineNextBaseY = 90f; 
        
        unimoStatData = unimoStatDataSo.GetFinalUnimoStatData(Base_Mng.Data.data.CharCount, Base_Mng.Data.data.CharLevel[Base_Mng.Data.data.CharCount - 1]);
        equipmentStatData = equipmentStatDataSo.GetFinalEquipmnetStatData(Base_Mng.Data.data.EQCount, Base_Mng.Data.data.EQLevel[Base_Mng.Data.data.EQCount - 1]);
        
        StatCalculator = new StatCalculator(unimoStatData, equipmentStatData);
        
        if (unimoStatData != null)
        {
            UpdateStatUI();
            UpgradeStatUI(unimoStatData);
            UpgradeEngineStatUI(equipmentStatData);
        }
    }
    
    // 유니모 레벨업
    public void UpgradeUnimoAndStatUI()
    {
        Base_Mng.instance.UpgradeUnimoLevel();
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
