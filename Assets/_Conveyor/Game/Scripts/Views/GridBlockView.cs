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
			propertyBlock.SetColor("_Color", colorBlock.Color);
			meshRenderer.SetPropertyBlock(propertyBlock);
			
			UpdateRotationForDirection(colorBlock.Direction);
		}

		private void UpdateRotationForDirection(BlockDirection blockDirection)
		{
			switch (blockDirection)
			{
				case BlockDirection.None:
				case BlockDirection.Right:
					// 0Y
					break;
				case BlockDirection.Up:
					// 270Y
					transform.localEulerAngles = new Vector3(0, 270f, 0);
					break;
				case BlockDirection.Down:
					// 90Y
					transform.localEulerAngles = new Vector3(0, 90f, 0);
					break;
				case BlockDirection.Left:
					// 180Y
					transform.localEulerAngles = new Vector3(0, 180f, 0);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(blockDirection), blockDirection, null);
			}
		}
	}
}