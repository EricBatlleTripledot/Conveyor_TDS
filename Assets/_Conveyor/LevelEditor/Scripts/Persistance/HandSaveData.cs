using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
	[Serializable]
	public class HandSaveData
	{
		public List<Color> initialCustomHand = new();
		public List<ColorWeightSaveData> colorWeights = new();
	}
}