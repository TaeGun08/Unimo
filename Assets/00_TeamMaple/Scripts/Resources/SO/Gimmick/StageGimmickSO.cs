using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableObjectScript", menuName = "Scriptable Objects/NewScriptableObjectScript")]
public abstract class StageGimmickSO : ScriptableObject
{
    public StageGimmickType gimmickType;
    
    public abstract GameObject Execute(Vector3 origin);
}
