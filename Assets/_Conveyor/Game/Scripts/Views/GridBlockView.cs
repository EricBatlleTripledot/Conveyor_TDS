using System;
using _2025.ColourBlockArrowProto.Scripts;
using TMPro;
using UnityEngine;

namespace Game
{
	public class GridBlockView : MonoBehaviour
	{
		[SerializeField]
		private ColorBlock colorBlock;
		[SerializeField]
		private MeshRenderer meshRenderer;
		[SerializeField]
		private ArrowTileMotions tileMotions;

		public ColorBlock ColorBlock => colorBlock;
		public ArrowTileMotions TileMotions => tileMotions;

		private MaterialPropertyBlock propertyBlock;
		
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
			propertyBlock = new MaterialPropertyBlock();
			propertyBlock.SetColor("_Color", colorBlock.Color);
			propertyBlock.SetFloat("_Icon_Rotation", colorBlock.Direction.ToUvRotation());
			meshRenderer.SetPropertyBlock(propertyBlock);
		}

		public void UpdateViewForCascade()
		{
			propertyBlock.SetFloat("_Icon_Rotation", 0);
			meshRenderer.SetPropertyBlock(propertyBlock);

			transform.eulerAngles = colorBlock.Direction.ToEuler();
		}
	}
}