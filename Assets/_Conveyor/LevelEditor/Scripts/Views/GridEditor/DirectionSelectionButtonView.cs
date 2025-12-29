using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
	public class DirectionSelectionButtonView : CustomButton
	{
		public event Action<DirectionSelectionButtonView> DirectionSelected;

		[SerializeField]
		private BlockDirection blockDirection;
		[SerializeField]
		private Image backgroundImage;
		[SerializeField]
		private TextMeshProUGUI symbolText;
		[SerializeField]
		private Outline outline;

		public BlockDirection BlockDirection => blockDirection;

		protected override void Awake()
		{
			outline.enabled = false;
			onLeftClick.AddListener(OnDirectionSelected);
			symbolText.text = blockDirection.ToSymbolString();
		}

		private void OnDirectionSelected()
		{
			DirectionSelected?.Invoke(this);
		}
		
		public void ShowOutline() => outline.enabled = true;
		public void HideOutline() => outline.enabled = false;
	}
}