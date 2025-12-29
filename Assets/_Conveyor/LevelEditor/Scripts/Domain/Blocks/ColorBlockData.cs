using System;
using UnityEngine;

namespace LevelEditor
{
	[Serializable]
	public class ColorBlockData : GridBlockData
	{
		public override GridBlockType BlockType => GridBlockType.Color;

		[SerializeField]
		private Color color;
		[SerializeField]
		private BlockDirection direction;

		public Color Color => color;
		public BlockDirection Direction => direction;
		
		public ColorBlockData(Vector2Int position, Color color, BlockDirection direction) : base(position)
		{
			this.color = color;
			this.direction = direction;
		}
	}
}