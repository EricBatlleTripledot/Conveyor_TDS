using System;
using UnityEngine;

namespace LevelEditor
{
	[Serializable]
	public class ConveyorBeltBlockData : GridBlockData
	{
		public override GridBlockType BlockType => GridBlockType.ConveyorBelt;

		public ConveyorBeltBlockData(Vector2Int position) : base(position)
		{
		}
	}
}