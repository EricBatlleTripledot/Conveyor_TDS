using System;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
	public class ColorSelectionButtonView : CustomButton
	{
		public event Action<ColorSelectionButtonView> ColorSelected;

		[SerializeField]
		private Color color;
		[SerializeField]
		private Image backgroundImage;
		[SerializeField]
		private Outline outline;

		public Color Color => color;
	
		protected override void Awake()
		{
			outline.enabled = false;
			backgroundImage.color = color;
			onLeftClick.AddListener(OnColorSelected);
		}

		public void SetColor(Color color)
		{
			this.color = color;
			backgroundImage.color = color;
		}

		private void OnColorSelected()
		{
			ColorSelected?.Invoke(this);
		}

		public void ShowOutline() => outline.enabled = true;
		public void HideOutline() => outline.enabled = false;
	}
}