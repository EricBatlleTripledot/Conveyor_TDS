using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LevelEditor
{
	public class DirectionSelectionView : MonoBehaviour
	{
		public event Action<BlockDirection> DirectionSelected;

		[SerializeField]
		private List<DirectionSelectionButtonView> selections;

		private void Awake()
		{
			foreach (var directionSelectionButtonView in selections)
			{
				directionSelectionButtonView.DirectionSelected += OnDirectionSelected;
			}
		}

		private void Start()
		{
			OnDirectionSelected(selections.First());
		}

		private void OnDirectionSelected(DirectionSelectionButtonView selectedDirectionSelectionButtonView)
		{
			foreach (var directionSelectionButtonView in selections)
			{
				directionSelectionButtonView.HideOutline();
			}
			selectedDirectionSelectionButtonView.ShowOutline();
			DirectionSelected?.Invoke(selectedDirectionSelectionButtonView.BlockDirection);
		}
	}
}