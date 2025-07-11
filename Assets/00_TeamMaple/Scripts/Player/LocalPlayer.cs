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
    public UnimoData UnimoData => unimoStatData.CreateDefaultUnimo();
    
    private PlayerController playerController;
    public Vector3 LastAttackerPos { get; private set; }
    public int CurMaxHp {get; set;}

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
        
        SetPlayerAnimator(unimo,  engine);
        CurMaxHp = UnimoData.Hp;
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