using System;
using UnityEngine;

namespace Game
{
	[Serializable]
	public class ColorBlockPayload
	{
		public Color color;
		public BlockDirection direction;
	}
}