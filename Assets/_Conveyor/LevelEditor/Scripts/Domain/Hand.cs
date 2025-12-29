using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
	[Serializable]
	public class Hand
	{
		public const float DEFAULT_COLOR_WEIGHT = 1;

		[SerializeField]
		private List<Color> initialCustomHand;
		[SerializeField]
		private Dictionary<Color, float> colorWeightsDict;

		public List<Color> InitialCustomHand => initialCustomHand;
		public Dictionary<Color, float> ColorWeightsDict => colorWeightsDict;

		public Hand(List<Color> initialCustomHand, Dictionary<Color, float> colorWeightsDict)
		{
			this.initialCustomHand = initialCustomHand;
			this.colorWeightsDict = colorWeightsDict;
		}

		public Hand()
		{
			initialCustomHand = new List<Color>();
			colorWeightsDict = new Dictionary<Color, float>();
		}
	}
}