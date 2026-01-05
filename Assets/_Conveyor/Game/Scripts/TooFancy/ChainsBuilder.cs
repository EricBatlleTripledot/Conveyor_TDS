using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T> where T : class
	{
		public static readonly ReferenceEqualityComparer<T> Instance = new();

		public bool Equals(T x, T y) => ReferenceEquals(x, y);

		public int GetHashCode(T obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
	}
	public sealed class ChainsBuilder
	{
		/// <summary>
		/// Finds all maximal chains in the grid following the rule:
		/// - each block points in one of 4 directions
		/// - its "next" is the first non-empty block in that rayCastDirection (skipping nulls)
		/// - chain continues while next exists and has the same color as the chain's color
		/// Handles monochrome cycles (closed loops) by collecting remaining unassigned blocks.
		/// </summary>
		public List<Chain> BuildAllChains(Grid<ColorBlock> grid)
		{
			var allBlocks = EnumerateBlocks(grid);

			// next[b] = first visible block in rayCastDirection (or null)
			var next = new Dictionary<ColorBlock, ColorBlock>(ReferenceEqualityComparer<ColorBlock>.Instance);

			// preds[v] = all blocks that have v as next
			var preds = new Dictionary<ColorBlock, List<ColorBlock>>(ReferenceEqualityComparer<ColorBlock>.Instance);

			foreach (var b in allBlocks)
				preds[b] = new List<ColorBlock>();

			foreach (var b in allBlocks)
			{
				var n = FindNextVisibleBlock(grid, b);
				if (n != null)
					preds[n].Add(b);

				next[b] = n; // n can be null; Dictionary allows null values for reference types
			}

			bool HasSameColorPredecessor(ColorBlock b)
			{
				// We need "exists any predecessor with same color"
				foreach (var p in preds[b])
				{
					if (p.Color == b.Color)
						return true;
				}
				return false;
			}

			var chains = new List<Chain>();
			var assigned = new HashSet<ColorBlock>(ReferenceEqualityComparer<ColorBlock>.Instance);

			// Pass 1: chains that have a valid start (no same-color predecessor)
			foreach (var start in allBlocks)
			{
				if (assigned.Contains(start)) continue;
				if (HasSameColorPredecessor(start)) continue;

				var chainBlocks = new List<ColorBlock>();
				var chainColor = start.Color;

				var cur = start;

				while (cur != null && !assigned.Contains(cur) && cur.Color == chainColor)
				{
					chainBlocks.Add(cur);
					assigned.Add(cur);

					var n = next[cur];
					if (n == null || n.Color != chainColor) break;

					cur = n;
				}

				if (chainBlocks.Count > 0)
					chains.Add(new Chain(chainColor, chainBlocks, isCyclic: false));
			}

			// Pass 2: remaining blocks (typically monochrome cycles)
			foreach (var start in allBlocks)
			{
				if (assigned.Contains(start)) continue;

				var chainBlocks = new List<ColorBlock>();
				var chainColor = start.Color;
				var isCyclic = false;

				var cur = start;

				while (cur != null && !assigned.Contains(cur) && cur.Color == chainColor)
				{
					chainBlocks.Add(cur);
					assigned.Add(cur);

					var n = next[cur];
					if (n == null || n.Color != chainColor)
					{
						// it ended; not a full monochrome cycle
						break;
					}

					if (ReferenceEquals(n, start))
					{
						// closes the loop back to start; cycle
						isCyclic = true;
						break;
					}

					cur = n;
				}

				if (chainBlocks.Count > 0)
					chains.Add(new Chain(chainColor, chainBlocks, isCyclic));
			}

			return chains;
		}

		// ---------------------------------------------------------------------
		// Helpers
		// ---------------------------------------------------------------------

		private static List<ColorBlock> EnumerateBlocks(Grid<ColorBlock> grid)
		{
			var blocks = new List<ColorBlock>();

			for (var y = 0; y < grid.Height; y++)
			for (var x = 0; x < grid.Width; x++)
			{
				var b = grid.Get(x, y);
				if (b != null)
					blocks.Add(b);
			}

			return blocks;
		}

		private static ColorBlock FindNextVisibleBlock(Grid<ColorBlock> grid, ColorBlock from)
		{
			var dir = from.Direction.ToVector2Int();

			// Defensive: ensure rayCastDirection is a cardinal step.
			if (!IsCardinal(dir))
				throw new InvalidOperationException($"Direction must be cardinal (±1,0) or (0,±1). Got: {dir}");

			var p = from.Position + dir;

			while (InBounds(grid, p))
			{
				var b = grid.Get(p.x, p.y);
				if (b != null)
					return b; // first non-empty blocks blocks line-of-sight

				p += dir; // skip empty
			}

			return null; // nothing visible in that rayCastDirection
		}

		private static bool InBounds(Grid<ColorBlock> grid, Vector2Int p)
		{
			return p.x >= 0 && p.x < grid.Width && p.y >= 0 && p.y < grid.Height;
		}

		private static bool IsCardinal(Vector2Int dir)
		{
			var ax = Math.Abs(dir.x);
			var ay = Math.Abs(dir.y);
			return (ax == 1 && ay == 0) || (ax == 0 && ay == 1);
		}
	}
}