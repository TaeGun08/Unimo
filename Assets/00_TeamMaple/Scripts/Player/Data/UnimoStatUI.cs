using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnimoStatUI : MonoBehaviour
{
    [SerializeField] private UnimoStatDataSO statDataSO;
    public GameObject statTextPrefab;
    public RectTransform contentParent;
    public RectTransform upgradeContentParent;
    public float lineSpacing = 50f;

    [SerializeField] private TMP_Text Name;
    [SerializeField] private TMP_Text Level;
    
    public void Start()
    {
        UnimoStatData finalData = statDataSO.GetFinalUnimoStatData(Base_Mng.Data.data.CharCount);
        if (finalData != null)
        {
            UpdateStatUI(finalData);
            UpgradeStatUI(finalData);
        }
    }

    public void UpdateStatUI(UnimoStatData data)
    {
        CreateStatLine($"체력 {data.Hp}");
        CreateStatLine($"방어력 {data.Def}");
        CreateStatLine($"이동속도 {data.Speed}%");
        CreateStatLine($"개화 범위 {data.BloomRange}");
        CreateStatLine($"개화 속도 {data.BloomSpeed}%");
        CreateStatLine($"꽃 생성 주기 {data.FlowerRate}%");
        CreateStatLine($"희귀 꽃 확률 {data.RareFlowerRate}%");
        CreateStatLine($"회피율 {data.Dodge}%");
        CreateStatLine($"스턴 회복력 {data.StunRecovery}%");
        CreateStatLine($"체력 재생 {data.HpRecovery}%");
        CreateStatLine($"꽃 낙하 속도 {data.FlowerDropSpeed}%");
        CreateStatLine($"낙하량 증가 {data.FlowerDropAmount}%");
    }
    
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
        //ShowUpgradeStat(Base_Mng.Data.data.CharCount, Base_Mng.Data.data.CharLevel[Base_Mng.Data.data.CharCount - 1]);
    }
    
    public void ShowUpgradeStat(int id, int currentLevel)
    {
        var (current, next) = statDataSO.GetCurrentAndNextStat(id, currentLevel);
        Debug.Log($"{current.Hp} / {next.Hp}");
        
        if (current == null || next == null)
        {
            Debug.LogWarning("데이터가 부족합니다.");
            return;
        }
        
        UpGradeNextStatLine($"체력 {next.Hp}");
        UpGradeNextStatLine($"방어력 {next.Def}");
        UpGradeNextStatLine($"이동속도 {next.Speed}%");
        UpGradeNextStatLine($"개화 범위 {next.BloomRange}");
        UpGradeNextStatLine($"개화 속도 {next.BloomSpeed}%");
        UpGradeNextStatLine($"꽃 생성 주기 {next.FlowerRate}%");
        UpGradeNextStatLine($"희귀 꽃 확률 {next.RareFlowerRate}%");
        UpGradeNextStatLine($"회피율 {next.Dodge}%");
        UpGradeNextStatLine($"스턴 회복력 {next.StunRecovery}%");
        UpGradeNextStatLine($"체력 재생 {next.HpRecovery}%");
        UpGradeNextStatLine($"꽃 낙하 속도 {next.FlowerDropSpeed}%");
        UpGradeNextStatLine($"낙하량 증가 {next.FlowerDropAmount}%");
    }

    private float currentY = 0;
    private float baseY  = 0;
    private float baseX = 0;
    
    private void CreateStatLine(string text)
    {
        GameObject statLine = Instantiate(statTextPrefab, contentParent, false);
        TMP_Text tmp = statLine.GetComponent<TMP_Text>();
        tmp.text = text;
        
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
        
        RectTransform rt = line.GetComponent<RectTransform>();

        if (upgradeBaseY == 0f)
        {
            upgradeBaseY = rt.anchoredPosition.y;
            upgradeBaseX = rt.anchoredPosition.x;
        }

        rt.anchoredPosition = new Vector2(upgradeBaseX, upgradeBaseY - upgradeCurrentY);
        upgradeCurrentY += lineSpacing;
    }
    
    private float upgradeNextCurrentY = 0f;
    private float upgradeNextBaseX = 40f;
    private float upgradeNextBaseY = 370f; 
    
    private void UpGradeNextStatLine(string text)
    {
        GameObject line = Instantiate(statTextPrefab, upgradeContentParent, false);
        TMP_Text tmp = line.GetComponent<TMP_Text>();
        tmp.text = text; 
        
        RectTransform rt = line.GetComponent<RectTransform>();

        if (upgradeNextBaseY == 0f)
        {
            upgradeNextBaseY = rt.anchoredPosition.y;
            upgradeNextBaseX = rt.anchoredPosition.x;
        }

        rt.anchoredPosition = new Vector2(upgradeNextBaseX, upgradeNextBaseY - upgradeNextCurrentY);
        upgradeNextCurrentY += lineSpacing;
    }

    public void RefreshUI()
    {
        // 기존 스탯 UI 삭제
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // 기존 업그레이드 UI 삭제
        foreach (Transform child in upgradeContentParent)
            Destroy(child.gameObject);

        // 위치 초기화
        currentY = 0;
        baseY = 0;

        upgradeCurrentY = 0;
        upgradeBaseY = 370f;  
        upgradeBaseX = -440f;
        
        upgradeNextCurrentY = 0f;
        upgradeNextBaseX = -340f;
        upgradeNextBaseY = 370f; 

        var finalData = statDataSO.GetFinalUnimoStatData(Base_Mng.Data.data.CharCount);
        if (finalData != null)
        {
            UpdateStatUI(finalData);
            UpgradeStatUI(finalData);
        }
    }
    
    public void UpgradeUnimoAndStatUI(int type)
    {
        Base_Mng.instance.UpgradeUnimoLevel(type);
        RefreshUI();
    }
}
