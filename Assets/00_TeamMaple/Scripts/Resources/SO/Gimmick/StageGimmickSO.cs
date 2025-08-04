using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableObjectScript", menuName = "Scriptable Objects/NewScriptableObjectScript")]
public abstract class StageGimmickSO : ScriptableObject
{
    public StageGimmickType gimmickType;
    [Header("Visual Settings")]
    public Material groundMaterial; // 넣지 않으면 null
    public Material backgroundMaterial; // 선택 사항
    public Material environmentMaterial; // 선택 사항
    
    public abstract GameObject Execute(Vector3 origin);
}
