using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
	public static class HandSaveMapper
	{
		public static HandSaveData ToHandSaveData(Hand hand)
		{
			if (hand == null)
			{
				return null;
			}

			var save = new HandSaveData
			{
				initialCustomHand = new List<Color>(hand.InitialCustomHand)
			};

			foreach (var kv in hand.ColorWeightsDict)
			{
				save.colorWeights.Add(new ColorWeightSaveData
				{
					color = kv.Key,
					weight = kv.Value
				});
			}

			return save;
		}
		
		public static Hand FromHandSaveData(HandSaveData save)
		{
			if (save == null)
			{
				return null;
			}

			var dict = new Dictionary<Color, float>();
			if (save.colorWeights != null)
			{
				foreach (var w in save.colorWeights)
				{
					dict[w.color] = w.weight;
				}
			}

			var initial = save.initialCustomHand ?? new List<Color>();
			return new Hand(initial, dict);
		}
	}
}