using UnityEngine;

public class ItemRenderer : MonoBehaviour
{
    MeshRenderer mainRenderer;
    MeshRenderer[] childRenderers;

    public void Awake()
    {
        mainRenderer = GetComponent<MeshRenderer>();
        childRenderers = GetComponentsInChildren<MeshRenderer>();
    }

    public void Active(bool state)
    {
        mainRenderer.enabled = state;

        for (int i = 0; i < childRenderers.Length; i++)
        {
            childRenderers[i].enabled = state;
        }
    }

    public void SetLayers(int numbler)
    {
        mainRenderer.gameObject.layer = numbler;

        for (int i = 0; i < childRenderers.Length; i++)
        {
            childRenderers[i].gameObject.layer = numbler;
        }
    }

    public void SetMaterial(Material material)
    {
        mainRenderer.material = material;

        for (int i = 0; i < childRenderers.Length; i++)
        {
            childRenderers[i].material = material;
        }
    }
}
