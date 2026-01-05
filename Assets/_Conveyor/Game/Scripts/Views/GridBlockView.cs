using TMPro;
using UnityEngine;

namespace Game
{
	[RequireComponent(typeof(MeshRenderer))]
	public class GridBlockView : MonoBehaviour
	{
		[SerializeField]
		private ColorBlock colorBlock;
		[SerializeField]
		private TextMeshPro text;
		[SerializeField]
		private MeshRenderer meshRenderer;

		public ColorBlock ColorBlock => colorBlock;

		public void Initialize(ColorBlock colorBlock)
		{
			this.colorBlock = colorBlock;
			UpdateView(colorBlock);
		}

		public void Destroy()
		{
			Destroy(gameObject);
		}

		private void UpdateView(ColorBlock colorBlock)
		{
			var propertyBlock = new MaterialPropertyBlock();
			propertyBlock.SetColor("_BaseColor", colorBlock.Color);
			meshRenderer.SetPropertyBlock(propertyBlock);
			text.text = this.colorBlock.Direction.ToSymbolString();
		}
	}
}