using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
	public class GameGridView : MonoBehaviour
	{
		[SerializeField]
		private GridViewsGenerator gridViewsGenerator;

		public List<GridBlockView> gridBlockViews;
		public void GenerateGrid(GameGrid gameGrid)
		{
			gridBlockViews = gridViewsGenerator.GenerateGrid(gameGrid);
		}

		public GridBlockView GetViewForBlock(ColorBlock block)
		{
			return gridBlockViews.Find(gridBlockView => gridBlockView.ColorBlock.Position == block.Position);
		}
		
		public void DestroyGridBlockView(ColorBlock colorBlock)
		{
			var gridBlockView = gridBlockViews.FirstOrDefault(gridBlockView => gridBlockView.ColorBlock.Position == colorBlock.Position);
			gridBlockView?.Destroy();
		}
	}
}