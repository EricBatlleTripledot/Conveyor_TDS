using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
	public class GridViewsGenerator : MonoBehaviour
	{
		[SerializeField]
		private List<ColorBlock> colorBlocksToCreate;
		[SerializeField]
		private GridBlockView gridBlockViewPrefab;
		[SerializeField]
		private GameObject gridSpacePrefab;
		[SerializeField]
		private Transform gridTransform;
		[SerializeField]
		private Vector2 gridSpacing = Vector2.one;

		// todo: temporary variable to help offset a level until a proper implementation is done - Canvas
		public Vector2 offset;
		
		private List<GridBlockView> blockViews;

		[ContextMenu("Generate Grid")]
		public void GenerateGrid()
		{
			blockViews = new List<GridBlockView>();
			gridTransform.DestroyAllChildsImmediate();
			int i = 0;
			foreach (var colorBlock in colorBlocksToCreate)
			{
				var gridBlockView = Instantiate(gridBlockViewPrefab, gridTransform);
				gridBlockView.gameObject.transform.position += new Vector3(i * gridSpacing.x, 0, 0);
				gridBlockView.Initialize(colorBlock);
				blockViews.Add(gridBlockView);
				i++;
			}
		}

		public List<GridBlockView> GenerateGrid(GameGrid gameGrid)
		{
			blockViews = new List<GridBlockView>();
			gridTransform.DestroyAllChildsImmediate();
			foreach (var colorBlock in gameGrid)
			{
				if (colorBlock == null)
				{
					continue;
				}
				var gridBlockView = InstantiateBlock(colorBlock);
				blockViews.Add(gridBlockView);


			}

			InstantiateSpaces(gameGrid);

			return blockViews;
		}

		private GridBlockView InstantiateBlock(ColorBlock colorBlock)
		{
			var gridBlockView = Instantiate(gridBlockViewPrefab, gridTransform);
			gridBlockView.gameObject.name = $"Cube_{colorBlock.Position.x}_{colorBlock.Position.y}";
			gridBlockView.gameObject.transform.position = new Vector3(colorBlock.Position.x * gridSpacing.x + offset.x, 0, colorBlock.Position.y * gridSpacing.y + offset.y);
			gridBlockView.Initialize(colorBlock);
			return gridBlockView;
		}
		
		private void InstantiateSpaces(GameGrid gameGrid)
		{
			var w = gameGrid.Width;
			var h = gameGrid.Height;

			for (int x = 0; x < w; x++)
			{
				for (int y = 0; y < h; y++)
				{
					var space = Instantiate(gridSpacePrefab, gridTransform);
					space.gameObject.name = $"Space_{x}_{y}";
					space.gameObject.transform.position = new Vector3(x * gridSpacing.x + offset.x, 0, y * gridSpacing.y + offset.y);
				}
			}
		}
	}
}