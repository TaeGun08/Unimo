using System;
using UnityEngine;
using UnityEngine.UIElements;

public class LocalPlayer : MonoBehaviour, IDamageAble
{
    public static LocalPlayer Instance { get; private set; }
    
    [Header("UnimoStatDataSO")]
    [SerializeField] private UnimoStatDataSO unimoStatData;
    [SerializeField] private PrefabsTable unimoTable;
    [SerializeField] private PrefabsTable engineTable;
    
    private PlayerController playerController;
    
    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        
        Instance = this;
        
        playerController = GetComponent<PlayerController>();
        
        // 장착된 유니모 체크 및 생성
        Debug.Log($"LocalPlayer CharCount:: {Base_Mng.Data.data.CharCount}");
        Debug.Log($"LocalPlayer EQCount:: {Base_Mng.Data.data.EQCount}");

        var unimo =unimoTable.GetPrefabByKey(Base_Mng.Data.data.CharCount);
        var engine =engineTable.GetPrefabByKey(Base_Mng.Data.data.EQCount);
        
        playerController.UnimoAnim = Instantiate(unimo, transform.position + Vector3.up,
            Quaternion.Euler(0,180f,0), transform).GetComponent<Animator>();
        playerController.EqAnim = Instantiate(engine, transform.position, 
            Quaternion.Euler(0,180f,0), transform).GetComponent<Animator>();

        playerController.UnimoAnim.enabled = true;
        playerController.EqAnim.enabled = true;
    }

    public void TakeDamage()
    {
        playerController.ChangeState(IPlayerState.EState.Hit);
    }
}