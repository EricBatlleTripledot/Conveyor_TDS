using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Game
{
	[Serializable]
	public class Grid<T> : IEnumerable<T>
	{
		protected T[,] Cells;
    
		public int Width { get; private set; }
		public int Height { get; private set;}
    
		public Grid(int width, int height)
		{
			Width = width;
			Height = height;
			Cells = new T[width, height];
		}
    
		[CanBeNull]
		public T Get(int x, int y) => Cells[x, y];
    
		public void Set(int x, int y, T value) => Cells[x, y] = value;
        
		public void Resize(int newWidth, int newHeight)
		{
			var newCells = new T[newWidth, newHeight];

			for (var x = 0; x < Math.Min(newWidth, Width); x++)
			{
				for (var y = 0; y < Math.Min(newHeight, Height); y++)
				{
					newCells[x, y] = Cells[x, y];
				}
			}
    
			Cells = newCells;
			Width = newWidth;
			Height = newHeight;
		}
    
		public List<T> GetNeighborsAt(int x, int y)
		{
			var neighbors = new List<T>();
    		
			TryAddNeighbor(x - 1, y + 1, neighbors);
			TryAddNeighbor(x - 1, y, neighbors);
			TryAddNeighbor(x - 1, y - 1, neighbors);
    		
			TryAddNeighbor(x, y + 1, neighbors);
			TryAddNeighbor(x, y - 1, neighbors);
    		
			TryAddNeighbor(x + 1, y + 1, neighbors);
			TryAddNeighbor(x + 1, y, neighbors);
			TryAddNeighbor(x + 1, y - 1, neighbors);
    
			return neighbors;
		}
    
		public IEnumerator<T> GetEnumerator()
		{
			for (var x = 0; x < Width; x++)
			{
				for (var y = 0; y < Height; y++)
				{
					yield return Cells[x, y];
				}
			}
		}
    
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
    	
		public bool IsValidPosition(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
    	
		private bool TryGet(int x, int y, out T value)
		{
			if (IsValidPosition(x, y))
			{
				value = Cells[x, y];
				return value != null;
			}
    
			value = default;
			return false;
		}
    	
		private void TryAddNeighbor(int x, int y, List<T> neighbors)
		{
			if (TryGet(x, y, out var cell))
			{
				neighbors.Add(cell);
			}
		}
		
		public bool HasValue(int x, int y) => IsValidPosition(x,y) && Cells[x,y] != null;
	}
}