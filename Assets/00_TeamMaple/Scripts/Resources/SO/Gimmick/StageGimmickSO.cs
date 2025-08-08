using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableObjectScript", menuName = "Scriptable Objects/NewScriptableObjectScript")]
public abstract class StageGimmickSO : ScriptableObject
{
    public StageGimmickType gimmickType;
    [Header("Visual Settings")]
    public Material groundMaterial; // 넣지 않으면 null
    public Material backgroundMaterial; // 선택 사항
    public Material environmentMaterial; // 선택 사항
    
    [Header("Gimmick Items")]
    public GameObject gimmickItemPrefab; // ✅ 아이템 프리팹
    public GameObject pickupEffect;      // ✅ 획득 시 이펙트
    public GameObject durationEffect;    // ✅ 지속 이펙트
    
    public abstract GameObject Execute(Vector3 origin);
}
