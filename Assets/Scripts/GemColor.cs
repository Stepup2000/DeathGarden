using UnityEngine;

public class GemColor : MonoBehaviour
{
    private Renderer gemRenderer;
    private MaterialPropertyBlock propBlock;

    private const int GEM_MATERIAL_INDEX = 1;

    void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        gemRenderer = GetComponent<Renderer>();
    }

    public void SetColor(Color color)
    {
        gemRenderer.GetPropertyBlock(propBlock, GEM_MATERIAL_INDEX);
        propBlock.SetColor("_BaseColor", color);
        gemRenderer.SetPropertyBlock(propBlock, GEM_MATERIAL_INDEX);
    }

    public void SetRed() => SetColor(Color.red);
    public void SetGreen() => SetColor(Color.green);
}
