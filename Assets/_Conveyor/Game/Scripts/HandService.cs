using UnityEngine;

namespace Game
{
	public class HandService
	{
		private readonly IRandomProvider randomProvider;

		public HandService(IRandomProvider randomProvider)
		{
			this.randomProvider = randomProvider;
		}

		public Color DequeueNextColor(Hand hand)
		{
			return hand.TryDequeueInitial(out var nextColor) ? nextColor : randomProvider.WeightedPick(hand.ColorWeights);
		}
	}
}