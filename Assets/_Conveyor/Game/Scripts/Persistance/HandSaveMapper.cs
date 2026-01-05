using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class HandSaveMapper
	{
		public Hand FromSaveData(HandSaveData save)
		{
			if (save == null)
			{
				return new Hand();
			}

			var initial = save.initialCustomHand ?? new List<Color>();

			var dict = new Dictionary<Color, float>();

			if (save.colorWeights == null)
			{
				return new Hand(initial, dict);
			}

			foreach (var colorWeightSaveData in save.colorWeights)
			{
				if (colorWeightSaveData.weight <= 0)
				{
					Debug.LogWarning("Color Weight with value <= 0");
				}
				dict[colorWeightSaveData.color] = colorWeightSaveData.weight;
			}

			return new Hand(initial, dict);
		}
	}
}