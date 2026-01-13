using System;
using _2025.ColourBlockArrowProto.Scripts;
using _Conveyor.Game.Scripts.TileRendering;
using TMPro;
using UnityEngine;

namespace Game
{
	public class GridBlockView : MonoBehaviour
	{
		private static readonly int ColorID = Shader.PropertyToID("_Color");
		
		[SerializeField]
		private ColorBlock colorBlock;
		[SerializeField]
		private MeshRenderer meshRenderer;
		[SerializeField]
		private ArrowTileMotions tileMotions;
		
		[Header("ScriptableObjects")]
		[SerializeField]
		private ArrowTileAnimationSettings tileAnimationSettings;
		[SerializeField]
		private TileIconConfig viewIconConfig;

		private MaterialPropertyBlock propertyBlock;

		public ColorBlock ColorBlock => colorBlock;
		public ArrowTileMotions TileMotions => tileMotions;

		public bool IsCascading { get; set; }
		public float LastRejectTime { get; private set; }

		private float RejectThreshold => Time.timeSinceLevelLoad - tileAnimationSettings.RejectOnBoardDuration;

		public void Initialize(ColorBlock colorBlock)
		{
			this.colorBlock = colorBlock;
			tileMotions.Initialise();
			
			UpdateView(colorBlock);
		}

		public void Destroy()
		{
			Destroy(gameObject);
		}
		
		private void UpdateView(ColorBlock colorBlock)
		{
			propertyBlock = new MaterialPropertyBlock();
			propertyBlock.SetColor(ColorID, colorBlock.Color);
			viewIconConfig.SetupPropertyBlockForArrow(propertyBlock, (int)colorBlock.Direction);
			
			meshRenderer.SetPropertyBlock(propertyBlock);
		}

		public void UpdateViewForCascade()
		{
			viewIconConfig.SetupPropertyBlockForArrow(propertyBlock, (int)BlockDirection.Right);
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