using System;
using UnityEngine;

namespace Game
{
	// ToDo: Should the level have the list of the available colors from JSON, even if it can retrieve them from the grid?
	[Serializable]
	public class Level
	{
		[SerializeField]
		private string name;
		[SerializeField]
		private GameGrid grid;
		[SerializeField]
		private Hand hand;

		public GameGrid Grid => grid;
		public Hand Hand => hand;

		public Level(string name, GameGrid grid, Hand hand)
		{
			this.name = name;
			this.grid = grid;
			this.hand = hand;
		}
		
		public Level(string name, GameGrid grid)
		{
			this.name = name;
			this.grid = grid;
			hand = new Hand(grid.GetUniqueColors());
		}
	}
}