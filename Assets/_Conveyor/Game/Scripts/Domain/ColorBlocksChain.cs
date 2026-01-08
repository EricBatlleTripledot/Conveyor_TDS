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
		public int Count { get; private set; }

		public ColorBlocksChain(List<ColorBlock> blocks)
		{
			this.blocks = blocks;
			Count = this.blocks.Count;
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