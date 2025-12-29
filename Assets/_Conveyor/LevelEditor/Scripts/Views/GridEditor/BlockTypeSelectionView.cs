using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LevelEditor
{
	public class BlockTypeSelectionView : MonoBehaviour
	{
		public event Action<GridBlockType> BlockTypeSelected;

		[SerializeField]
		private List<BlockTypeSelectionButtonView> selections;

		public GridBlockType DefaultSelectedBlockType => GridBlockType.Color;

		private void Awake()
		{
			foreach (var blockTypeSelectionButtonView in selections)
			{
				blockTypeSelectionButtonView.BlockTypeSelected += OnBlockTypeSelected;
			}
		}

		private void Start()
		{
			OnBlockTypeSelected(selections.First(view => view.BlockType == DefaultSelectedBlockType));
		}

		private void OnBlockTypeSelected(BlockTypeSelectionButtonView selectedBlockTypeSelectionButtonView)
		{
			foreach (var blockTypeSelectionButtonView in selections)
			{
				blockTypeSelectionButtonView.HideOutline();
			}
			selectedBlockTypeSelectionButtonView.ShowOutline();
			BlockTypeSelected?.Invoke(selectedBlockTypeSelectionButtonView.BlockType);
		}
	}
}