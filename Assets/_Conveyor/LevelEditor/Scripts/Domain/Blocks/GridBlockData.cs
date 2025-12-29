using System;
using UnityEngine;

namespace LevelEditor
{
	[Serializable]
	public abstract class GridBlockData
	{
		public abstract GridBlockType BlockType { get; }

		[SerializeField]
		private Vector2Int position;

		public Vector2Int Position => position;

		protected GridBlockData(Vector2Int position)
		{
			this.position = position;
		}
	}
}