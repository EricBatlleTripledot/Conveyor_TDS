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
		private static readonly int IconRotationID = Shader.PropertyToID("_Icon_Rotation");
		
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
		private BlockViewIconConfig viewIconConfig;

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

		[ContextMenu("Clear")]
		public void DebugClear() => meshRenderer.SetPropertyBlock(null);
		[ContextMenu("Test Left")]
		public void DebugLeft() => UpdateView(new ColorBlock(Vector2Int.zero, Color.blue, BlockDirection.Left));
		[ContextMenu("Test Right")]
		public void DebugRight() => UpdateView(new ColorBlock(Vector2Int.zero, Color.blue, BlockDirection.Right));
		[ContextMenu("Test Up")]
		public void DebugUp() => UpdateView(new ColorBlock(Vector2Int.zero, Color.blue, BlockDirection.Up));
		[ContextMenu("Test Down")]
		public void DebugDown() => UpdateView(new ColorBlock(Vector2Int.zero, Color.blue, BlockDirection.Down));
		
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