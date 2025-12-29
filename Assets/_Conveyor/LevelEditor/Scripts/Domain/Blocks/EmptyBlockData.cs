using System;
using UnityEngine;

namespace LevelEditor
{
	[Serializable]
	public class EmptyBlockData : GridBlockData
	{
		public override GridBlockType BlockType => GridBlockType.Empty;

		public EmptyBlockData(Vector2Int position) : base(position)
		{
		}
	}
}