using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	[Serializable]
	public class ColorBlocksChain : IEnumerable<ColorBlock>
	{
		[SerializeField]
		private List<ColorBlock> blocks;

		public List<ColorBlock> Blocks => blocks;

		public ColorBlocksChain(List<ColorBlock> blocks)
		{
			this.blocks = blocks;
		}

		public void AddBlock(ColorBlock colorBlock)
		{
			blocks.Add(colorBlock);
		}

		public void RemoveBlock(ColorBlock colorBlock)
		{
			blocks.Remove(colorBlock);
		}

		public IEnumerator<ColorBlock> GetEnumerator() => blocks.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}