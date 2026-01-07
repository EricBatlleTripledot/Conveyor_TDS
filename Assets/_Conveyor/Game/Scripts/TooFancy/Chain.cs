using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class Chain : IEnumerable<ColorBlock>
	{
		private readonly List<ColorBlock> blocks;

		public Color Color { get; }
		public bool IsCyclic { get; private set; }

		public int Count => blocks.Count;
		public IReadOnlyList<ColorBlock> Blocks => blocks;

		public ColorBlock First => blocks[0];
		public ColorBlock Last => blocks[blocks.Count - 1];

		public Chain(Color color, List<ColorBlock> blocks, bool isCyclic)
		{
			if (blocks == null) throw new ArgumentNullException(nameof(blocks));
			if (blocks.Count == 0) throw new ArgumentException("A chain cannot be empty.", nameof(blocks));

			Color = color;
			this.blocks = blocks;
			IsCyclic = isCyclic;
		}

		public bool Contains(ColorBlock block) => blocks.Contains(block);

		public int IndexOf(ColorBlock block) => blocks.IndexOf(block);

		/// <summary>
		/// Removes from the first occurrence of 'fromInclusive' to the end of the chain.
		/// Returns the removed blocks in order.
		/// Example: [A,B,C,D,E], RemoveForwardFrom(C) -> removed [C,D,E], remaining [A,B]
		/// </summary>
		public List<ColorBlock> RemoveForwardFrom(ColorBlock fromInclusive)
		{
			var startIndex = blocks.IndexOf(fromInclusive);
			if (startIndex < 0)
				throw new InvalidOperationException("Block not found in this chain.");

			var removedCount = blocks.Count - startIndex;
			var removed = blocks.GetRange(startIndex, removedCount);
			blocks.RemoveRange(startIndex, removedCount);

			// If we removed part of a cycle, it’s no longer cyclic
			if (IsCyclic) IsCyclic = false;

			return removed;
		}

		public IEnumerator<ColorBlock> GetEnumerator() => blocks.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}