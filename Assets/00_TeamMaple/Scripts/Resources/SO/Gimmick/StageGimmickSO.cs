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
    public GameObject gimmickItemPrefab; // ✅ 아이템 프리팹 등록
    
    public abstract GameObject Execute(Vector3 origin);
}
