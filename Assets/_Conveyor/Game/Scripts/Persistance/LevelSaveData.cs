using System;
using System.Collections.Generic;

namespace Game
{
	[Serializable]
	public class LevelSaveData
	{
		public string name;
		public int width;
		public int height;

		public List<BlockSaveData> blocks = new();
		public HandSaveData hand;
	}
}