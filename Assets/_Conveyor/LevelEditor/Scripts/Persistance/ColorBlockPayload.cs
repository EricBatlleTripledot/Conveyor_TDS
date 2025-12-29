using System;
using UnityEngine;

namespace LevelEditor
{
	[Serializable]
	public class ColorBlockPayload
	{
		public Color color;
		public BlockDirection direction;
	}
}