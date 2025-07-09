using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableObjectScript", menuName = "Scriptable Objects/NewScriptableObjectScript")]
public abstract class StageGimmickSO : ScriptableObject
{
    public abstract GameObject Execute(Vector3 origin);
}
