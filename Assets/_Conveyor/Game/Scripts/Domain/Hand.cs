using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
	[Serializable]
	public class Hand
	{
		public const float DEFAULT_COLOR_WEIGHT = 1;

		[SerializeField]
		private List<Color> initialCustomHand;

		// ToDo: use ColorIds instead of Color (floats precision)
		private readonly Dictionary<Color, float> colorWeightsDict;
		
		public IReadOnlyDictionary<Color, float> ColorWeights => colorWeightsDict;

		private Queue<Color> currentInitialHand;

		// ToDo: Should the hand have default weights for the colors when imported?
		public Hand(): this(new List<Color>(), new Dictionary<Color, float>()) { }
		public Hand(HashSet<Color> availableColors) : this(new List<Color>(), availableColors.ToDictionary(c => c, _ => DEFAULT_COLOR_WEIGHT)) { }
		public Hand(List<Color> initialCustomHand, Dictionary<Color, float> colorWeightsDict)
		{
			this.initialCustomHand = initialCustomHand;
			this.colorWeightsDict = colorWeightsDict;
			currentInitialHand = new Queue<Color>(initialCustomHand);
		}

		public bool TryDequeueInitial(out Color color) => currentInitialHand.TryDequeue(out color);
	}
}