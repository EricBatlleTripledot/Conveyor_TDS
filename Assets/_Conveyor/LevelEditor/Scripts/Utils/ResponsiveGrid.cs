using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
	[RequireComponent(typeof(GridLayoutGroup))]
	public class ResponsiveGrid : MonoBehaviour
	{
		[SerializeField] 
		private int columns = 4;

		private GridLayoutGroup grid;
		private RectTransform rectTransform;

		private void Awake()
		{
			grid = GetComponent<GridLayoutGroup>();
			rectTransform = GetComponent<RectTransform>();
		}

		private void OnRectTransformDimensionsChange()
		{
			UpdateCellSize();
		}

		[ContextMenu("UpdateCellSize")]
		public void UpdateCellSize()
		{
			var totalWidth =
				rectTransform.rect.width
				- grid.padding.left
				- grid.padding.right
				- grid.spacing.x * (columns - 1);
			var totalHeight =
				rectTransform.rect.height
				- grid.padding.top
				- grid.padding.bottom
				- grid.spacing.y * (columns - 1);

			var cellWidth = totalWidth / columns;
			var cellHeight = totalHeight / columns;
			var maxCellWidth = Mathf.Min(cellWidth, cellHeight);
			grid.cellSize = new Vector2(maxCellWidth, maxCellWidth);
		}
		
		public void UpdateCellSize(int columns)
		{
			this.columns = columns;
			UpdateCellSize();
		}
	}
}