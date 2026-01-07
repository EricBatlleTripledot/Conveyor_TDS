using System;
using UnityEngine;

namespace Game
{
	[Serializable]
	public class ConveyorBlock
	{
		[SerializeField]
		private Color color;
     
		public Color Color => color;

		public ConveyorBlock(Color color)
		{
			this.color = color;
		}
	}
}