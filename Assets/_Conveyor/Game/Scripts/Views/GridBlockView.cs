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
		[SerializeField]
		private ArrowTileAnimationSettings tileAnimationSettings;

		private MaterialPropertyBlock propertyBlock;

		public ColorBlock ColorBlock => colorBlock;
		public ArrowTileMotions TileMotions => tileMotions;

		public bool IsCascading { get; set; }
		public float LastRejectTime { get; private set; }

		private float RejectThreshold => Time.timeSinceLevelLoad - tileAnimationSettings.RejectOnBoardDuration;

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

		public void DoRejectShake()
		{
			if (LastRejectTime >= RejectThreshold)
			{
				return;
			}
			
			LastRejectTime = Time.timeSinceLevelLoad;
			
			tileMotions.DoRejectOnBoard();
		}
	}
}