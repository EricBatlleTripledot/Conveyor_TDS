using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Game
{
	[Serializable]
	public class GameGrid : Grid<ColorBlock>
	{
		public GameGrid(int width, int height) : base(width, height)
		{
		}

		public void Remove(int x, int y)
		{
			Set(x, y, null);
		}
		
		public void Remove(ColorBlock colorBlock)
		{
			Set(colorBlock.Position.x, colorBlock.Position.y, null);
		}

		public void Set(ColorBlock colorBlock) => Cells[colorBlock.Position.x, colorBlock.Position.y] = colorBlock;

		public HashSet<Color> GetUniqueColors()
		{
			var uniqueColors = new HashSet<Color>();
			foreach (var colorBlock in this)
			{
				// ToDo: I'm not a huge fan of this null approach for color blocks inside grid
				if (colorBlock != null)
				{
					uniqueColors.Add(colorBlock.Color);
				}
			}

			return uniqueColors;
		}

		[CanBeNull]
		private ColorBlock GetNextVisibleBlock(ColorBlock colorBlock)
		{
			var direction = colorBlock.Direction.ToVector2Int();
			if (direction == Vector2Int.zero)
			{
				throw new System.InvalidOperationException("BlockDirection cannot be zero.");
			}

			var nextBlockPosition = new Vector2Int(colorBlock.Position.x + direction.x, colorBlock.Position.y + direction.y);
			while (IsValidPosition(nextBlockPosition.x, nextBlockPosition.y))
			{
				var nextBlock = Get(nextBlockPosition.x, nextBlockPosition.y);
				if (nextBlock != null)
				{
					return nextBlock;
				}

				nextBlockPosition.x += direction.x;
				nextBlockPosition.y += direction.y;
			}

			return null;
		}

		public bool BlockChainHasValidExitPath(ColorBlock startColorBlock)
		{
			var visited = new HashSet<Vector2Int>();

			var currentColorBlock = startColorBlock;

			while (true)
			{
				// cyclic
				if (!visited.Add(currentColorBlock.Position))
				{
					return false;
				}

				var next = GetNextVisibleBlock(currentColorBlock);

				if (next == null)
				{
					return true;
				}

				if (next.Color != startColorBlock.Color)
				{
					return false;
				}

				currentColorBlock = next;
			}
		}

		public ColorBlocksChain GetBlockChain(ColorBlock startColorBlock)
		{
			var colorBlocksChain = new ColorBlocksChain(new List<ColorBlock> {startColorBlock});
			var visited = new HashSet<Vector2Int>();

			var currentColorBlock = startColorBlock;

			while (true)
			{
				// cyclic
				if (!visited.Add(currentColorBlock.Position))
				{
					return colorBlocksChain;
				}

				var next = GetNextVisibleBlock(currentColorBlock);

				if (next == null)
				{
					return colorBlocksChain;
				}

				if (next.Color != startColorBlock.Color)
				{
					return colorBlocksChain;
				}

				colorBlocksChain.AddBlock(next);
				currentColorBlock = next;
			}
		}
	}
}