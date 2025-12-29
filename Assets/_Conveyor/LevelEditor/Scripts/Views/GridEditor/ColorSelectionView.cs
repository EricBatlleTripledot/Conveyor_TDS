using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LevelEditor
{
	public class ColorSelectionView : MonoBehaviour
	{
		public event Action<Color> ColorSelected;

		[SerializeField]
		private List<ColorSelectionButtonView> selections;

		[SerializeField]
		private Transform selectionsContentTransform;
		[SerializeField]
		private ColorSelectionButtonView colorSelectionButtonViewPrefab;

		public Color DefaultSelectedColor => selections.Any() ? selections.First().Color : Color.red;

		private void Awake()
		{
			foreach (var colorSelectionButtonView in selections)
			{
				colorSelectionButtonView.ColorSelected += OnColorSelected;
			}
		}

		private void Start()
		{
			SetInitialColorSelection();
		}

		public void SetColorSelections(List<Color> colors)
		{
			foreach (var color in colors)
			{
				var colorSelectionButtonView = Instantiate(colorSelectionButtonViewPrefab, selectionsContentTransform, false);
				colorSelectionButtonView.SetColor(color);
				colorSelectionButtonView.ColorSelected += OnColorSelected;
				selections.Add(colorSelectionButtonView);
			}

			SetInitialColorSelection();
		}

		private void SetInitialColorSelection()
		{
			if (selections.Any())
			{
				OnColorSelected(selections.First());
			}
		}

		private void OnColorSelected(ColorSelectionButtonView selectedColorSelectionButtonView)
		{
			foreach (var colorSelectionButtonView in selections)
			{
				colorSelectionButtonView.HideOutline();
			}
			selectedColorSelectionButtonView.ShowOutline();
			ColorSelected?.Invoke(selectedColorSelectionButtonView.Color);
		}
	}
}