using System;
using System.Linq;
using UnityEngine;

namespace LevelEditor
{
	[Serializable]
	public class Level
	{
		[SerializeField]
		private string name;
		[SerializeField]
		private Grid<EditableBlockData> grid;
		[SerializeField]
		private Hand hand;

		public string Name => name;
		public Grid<EditableBlockData> Grid => grid;
		public Hand Hand => hand;

		public int Width => grid.Width;
		public int Height => grid.Height;

		public Level(string name, Vector2Int gridSize)
		{
			this.name = name;
			var width = gridSize.x;
			var height = gridSize.y;
			grid = new Grid<EditableBlockData>(width, height);

			for (var y = 0; y < height; y++)
			{
				for (var x = 0; x < width; x++)
				{
					grid.Set(x, y, new EditableBlockData(new EmptyBlockData(new Vector2Int(x, y))));
				}
			}
		}

		public Level(string name, Grid<EditableBlockData> grid, Hand hand)
		{
			this.name = name;
			this.grid = grid ?? throw new ArgumentNullException(nameof(grid));
			this.hand = hand;
		}
		
		public Level(string name, Grid<EditableBlockData> grid)
		{
			this.name = name;
			this.grid = grid ?? throw new ArgumentNullException(nameof(grid));
			this.hand = new Hand();
		}

		public Level(string name)
		{
			this.name = name;
		}

		// ToDo: This method means that something in the model is wrong, take a look to it
		public bool HandIsSet()
		{
			if (hand == null)
			{
				return false;
			}

			return Hand.InitialCustomHand.Any() && hand.ColorWeightsDict.Any();
		}
	}
}