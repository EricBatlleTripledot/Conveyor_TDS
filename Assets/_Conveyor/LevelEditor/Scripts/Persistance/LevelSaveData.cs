using System;
using System.Collections.Generic;

namespace LevelEditor
{
	// ToDo: Until I know if I can use Newtonsoft or not, this is the best approach so far
	[Serializable]
	public class LevelSaveData
	{
		public int version = 1;
		public string name;
		public int width;
		public int height;
		public List<BlockSaveData> blocks = new();
		public HandSaveData hand;
	}
}