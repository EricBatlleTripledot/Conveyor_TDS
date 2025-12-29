using UnityEngine;

public class CellView : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private Transform arrowTransform;

    private MaterialPropertyBlock propertyBlock;
    private static readonly int ColorId = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

    public void SetColor(Color c)
    {
        meshRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(ColorId, c);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    public void SetRotation(ArrowDirection dir)
    {
        float rot = dir switch {
            ArrowDirection.Up => 0f,
            ArrowDirection.Right => -90f,
            ArrowDirection.Down => 180f,
            ArrowDirection.Left => 90f,
            _ => 0f
        };
        arrowTransform.rotation = Quaternion.Euler(0, 0, rot);
    }
}