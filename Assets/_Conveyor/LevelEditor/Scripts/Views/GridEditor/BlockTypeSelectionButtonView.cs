using System;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
	public class BlockTypeSelectionButtonView : CustomButton
	{
		public event Action<BlockTypeSelectionButtonView> BlockTypeSelected;

		[SerializeField]
		private GridBlockType blockType;
		[SerializeField]
		private Outline outline;

		public GridBlockType BlockType => blockType;

		protected override void Awake()
		{
			outline.enabled = false;
			onLeftClick.AddListener(OnBlockTypeSelected);
		}

		private void OnBlockTypeSelected()
		{
			BlockTypeSelected?.Invoke(this);
		}

		public void ShowOutline() => outline.enabled = true;
		public void HideOutline() => outline.enabled = false;
	}
}