using System;
using UnityEngine;

public class LocalPlayer : MonoBehaviour, IDamageAble
{
    public static LocalPlayer Instance { get; private set; }

    [Header("UnimoStatDataSO")]
    [SerializeField] private UnimoStatDataSO unimoStatData;
    
    private PlayerController playerController;
    
    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        
        Instance = this;
        
        playerController = GetComponent<PlayerController>();
    }

    public void TakeDamage()
    {
        playerController.ChangeState(IPlayerState.EState.Hit);
    }
}