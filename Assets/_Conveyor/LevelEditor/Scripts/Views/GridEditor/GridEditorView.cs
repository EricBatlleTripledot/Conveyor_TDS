using System;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    public class GridEditorView : MonoBehaviour
    {
        public event Action<Grid<EditableBlockData>> SaveButtonClicked;
        public event Action<Grid<EditableBlockData>> EditHandButtonClicked;
        public event Action BackButtonClicked;

        [SerializeField]
        private EditableBlockView editableBlockPrefab;
        [Space]
        [SerializeField]
        private GridLayoutGroup gridLayoutGroup;
        [SerializeField]
        private ResponsiveGrid responsiveGrid;
        [SerializeField]
        private BlockTypeSelectionView blockTypeSelectionView;
        [SerializeField]
        private ColorSelectionView colorSelectionView;
        [SerializeField]
        private DirectionSelectionView directionSelectionView;
        [Header("Other Buttons")]
        [SerializeField]
        private Button editHandButton;
        [SerializeField]
        private Button saveButton;
        [SerializeField]
        private Button backButton;

        private GridBlockType currentSelectedBlockType;
        private Color currentSelectedColor;
        private BlockDirection currentSelectedDirection = BlockDirection.None;
        private Grid<EditableBlockView> editableBlockViewsGrid;
    
        private void Awake()
        {
            colorSelectionView.ColorSelected += OnColorSelected;
            directionSelectionView.DirectionSelected += OnDirectionSelected;
            blockTypeSelectionView.BlockTypeSelected += OnBlockTypeSelected;

            currentSelectedColor = colorSelectionView.DefaultSelectedColor;
            editHandButton.onClick.AddListener(OnEditHandButton);
            saveButton.onClick.AddListener(OnSaveButton);
            backButton.onClick.AddListener(OnBackButton);
        }

        private void OnEditHandButton()
        {
            var editableBlockDataGrid = ToEditableBlockDataGrid(editableBlockViewsGrid);
            EditHandButtonClicked?.Invoke(editableBlockDataGrid);
        }

        private void OnBackButton()
        {
            BackButtonClicked?.Invoke();
        }

        private void OnSaveButton()
        {
            var editableBlockDataGrid = ToEditableBlockDataGrid(editableBlockViewsGrid);
            SaveButtonClicked?.Invoke(editableBlockDataGrid);
        }

        private Grid<EditableBlockData> ToEditableBlockDataGrid(Grid<EditableBlockView> editableBlockViewsGrid)
        {
            var editableBlockDataGrid = new Grid<EditableBlockData>(editableBlockViewsGrid.Width, editableBlockViewsGrid.Height);
            foreach (var editableBlockView in editableBlockViewsGrid)
            {
                editableBlockDataGrid.Set(editableBlockView.CurrentData.BlockData.Position.x, editableBlockView.CurrentData.BlockData.Position.y, editableBlockView.CurrentData);
            }

            return editableBlockDataGrid;
        }

        private void OnColorSelected(Color color)
        {
            currentSelectedColor = color;
        }

        private void OnDirectionSelected(BlockDirection direction)
        {
            currentSelectedDirection = direction;
        }
        
        private void OnBlockTypeSelected(GridBlockType blockType)
        {
            currentSelectedBlockType = blockType;
        }

        public void GenerateGrid(Vector2Int gridSize)
        {
            editableBlockViewsGrid = new Grid<EditableBlockView>(gridSize.x, gridSize.y);
            AdjustLayout(gridSize.x);
            for (var y = 0; y < gridSize.y; y++)
            {
                for (var x = 0; x < gridSize.x; x++)
                {
                    var editableBlockView = Instantiate(editableBlockPrefab, gridLayoutGroup.transform, false);
                    editableBlockView.UpdateView(new EditableBlockData(new EmptyBlockData(new Vector2Int(x, y))));
                    editableBlockView.BlockSelected += OnBlockSelected;
                    editableBlockViewsGrid.Set(x, y, editableBlockView);
                }
            }
        }
    
        public void GenerateGrid(Grid<EditableBlockData> grid)
        {
            editableBlockViewsGrid = new Grid<EditableBlockView>(grid.Width, grid.Height);
            AdjustLayout(grid.Width);
            for (var y = 0; y < grid.Height; y++)
            {
                for (var x = 0; x < grid.Width; x++)
                {
                    var editableBlockView = Instantiate(editableBlockPrefab, gridLayoutGroup.transform, false);
                    editableBlockView.UpdateView(grid.Get(x, y));
                    editableBlockView.BlockSelected += OnBlockSelected;
                    editableBlockViewsGrid.Set(x, y, editableBlockView);
                }
            }
        }

        private void AdjustLayout(int columns)
        {
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            gridLayoutGroup.constraintCount = columns;
            responsiveGrid.UpdateCellSize(columns);
        }

        private void OnBlockSelected(EditableBlockView editableBlockView)
        {
            var position = editableBlockView.CurrentData.BlockData.Position;
            GridBlockData gridBlockData = currentSelectedBlockType switch
            {
                GridBlockType.Empty => new EmptyBlockData(position),
                GridBlockType.ConveyorBelt => new ConveyorBeltBlockData(position),
                GridBlockType.Color => new ColorBlockData(position, currentSelectedColor, currentSelectedDirection),
                _ => throw new ArgumentOutOfRangeException()
            };
            editableBlockView.UpdateView(new EditableBlockData(gridBlockData));
        }

    }
}