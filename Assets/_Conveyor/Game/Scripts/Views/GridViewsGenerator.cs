using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class GridViewsGenerator : MonoBehaviour
	{
		[SerializeField]
		private List<ColorBlock> colorBlocksToCreate;
		[SerializeField]
		private GridBlockView gridBlockViewPrefab;
		[SerializeField]
		private Transform gridTransform;
		[SerializeField]
		private int gridSpacing;

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
				gridBlockView.gameObject.transform.position += new Vector3(i * gridSpacing, 0, 0);
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

			return blockViews;
		}

		private GridBlockView InstantiateBlock(ColorBlock colorBlock)
		{
			var gridBlockView = Instantiate(gridBlockViewPrefab, gridTransform);
			gridBlockView.gameObject.name = $"Cube_{colorBlock.Position.x}_{colorBlock.Position.y}";
			gridBlockView.gameObject.transform.position = new Vector3(colorBlock.Position.x * gridSpacing, 0, colorBlock.Position.y * gridSpacing);
			gridBlockView.Initialize(colorBlock);
			return gridBlockView;
		}
	}
}