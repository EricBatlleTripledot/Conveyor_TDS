using System;
using UnityEngine;

namespace LevelEditor
{
	[Serializable]
	public class EditableBlockData
	{
		[SerializeReference]
		private GridBlockData gridBlockData;

		public GridBlockData BlockData
		{
			get => gridBlockData;
			set => gridBlockData = value;
		}
		
		public EditableBlockData(GridBlockData gridBlockData)
		{
			this.gridBlockData = gridBlockData;
		}
	}
}