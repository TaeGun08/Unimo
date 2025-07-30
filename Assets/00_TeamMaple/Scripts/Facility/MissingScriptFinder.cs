using UnityEditor;
using UnityEngine;

public class MissingScriptFinder : MonoBehaviour
{
    [MenuItem("Tools/Remove Missing Scripts In Scene")]
    static void RemoveMissingScripts()
    {
        int count = 0;

        foreach (GameObject go in FindObjectsOfType<GameObject>(true))
        {
            int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            if (removed > 0)
            {
                Debug.Log($"Removed {removed} missing script(s) from {go.name}");
                count += removed;
            }
        }

        Debug.Log($"Total missing scripts removed: {count}");
    }
}