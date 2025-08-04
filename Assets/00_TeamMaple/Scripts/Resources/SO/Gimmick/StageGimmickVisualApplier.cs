using UnityEngine;

public class StageGimmickVisualApplier : MonoBehaviour
{
    [SerializeField] private MeshRenderer groundRenderer;
    [SerializeField] private MeshRenderer backgroundRenderer;
    [SerializeField] private MeshRenderer environmentRenderer;

    public void ApplyVisuals(StageGimmickSO so)
    {
        if (so.groundMaterial != null && groundRenderer != null)
            groundRenderer.sharedMaterial = so.groundMaterial;

        if (so.backgroundMaterial != null && backgroundRenderer != null)
            backgroundRenderer.sharedMaterial = so.backgroundMaterial;

        if (so.environmentMaterial != null && environmentRenderer != null)
            environmentRenderer.sharedMaterial = so.environmentMaterial;
    }
}