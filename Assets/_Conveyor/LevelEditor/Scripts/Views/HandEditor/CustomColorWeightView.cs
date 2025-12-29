using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LevelEditor
{
	public class CustomColorWeightView : MonoBehaviour
	{
		public event Action<Color, float> ColorWeightChanged;
		[SerializeField]
		private Image colorImage;
		[SerializeField]
		private TMP_InputField weightInputField;

		private Color selectedColor;

		private void Awake()
		{
			weightInputField.onValueChanged.AddListener(OnWeightChanged);
		}

		public void SetColor(Color color)
		{
			colorImage.color = color;
			selectedColor = color;
		}
		
		public void SetWeight(float weight)
		{
			weightInputField.text = weight.ToString(CultureInfo.InvariantCulture);
		}

		private void OnWeightChanged(string weightText)
		{
			if (float.TryParse(weightText, out var weight))
			{
				ColorWeightChanged?.Invoke(selectedColor, weight);
			}
		}
	}
}