using System;

namespace LevelEditor
{
	[Serializable]
	public class BlockSaveData
	{
		public int x;
		public int y;
		public string type;
		public string payload;
	}
}