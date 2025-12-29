using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace LevelEditor
{
    public class LevelEditorController : MonoBehaviour
    {
        [Space]
        [SerializeField]
        private CreateOrLoadView createOrLoadViewPrefab;
        [SerializeField]
        private GridSizeSelectorView gridSizeSelectorViewPrefab;
        [SerializeField]
        private GridEditorView gridEditorViewPrefab;
        [SerializeField]
        private HandEditorView handEditorViewPrefab;

        private CreateOrLoadView createOrLoadView;
        private GridSizeSelectorView gridSizeSelectorView;
        private GridEditorView gridEditorView;
        private HandEditorView handEditorView;

        private Level level;

        private void Awake()
        {
            ShowCreateOrLoad();
        }

        private void ShowCreateOrLoad()
        {
            createOrLoadView = Instantiate(createOrLoadViewPrefab, transform, false);
            createOrLoadView.CreateClicked += OnCreateLevel;
            createOrLoadView.LoadClicked += OnLoadLevel;
        }

        private void OnCreateLevel(string levelName)
        {
            level = new Level(levelName);
            Destroy(createOrLoadView.gameObject);
            ShowGridSizeSelector();
        }

        private void ShowGridSizeSelector()
        {
            gridSizeSelectorView = Instantiate(gridSizeSelectorViewPrefab, transform, false);
            gridSizeSelectorView.OnGridSizeConfirmed += OnGridSizeConfirmed;
            gridSizeSelectorView.OnBack += OnGridSizeBackButtonClicked;
        }

        private void InternalShowGridEditor()
        {
            gridEditorView = Instantiate(gridEditorViewPrefab, transform, false);
            gridEditorView.EditHandButtonClicked += OnEditHand;
            gridEditorView.SaveButtonClicked += OnSaveLevel;
            gridEditorView.BackButtonClicked += OnGridEditorBackButtonClicked;
        }

        private void OnEditHand(Grid<EditableBlockData> grid)
        {
            var uniqueColors = GetGridUniqueColors(grid);
            if (uniqueColors.Count == 0)
            {
                return;
            }
            
            // Validate if hand colors were removed or added
            if (level.HandIsSet())
            {
                var updatedHand = UpdateHandData(level.Hand, grid);
                level = new Level(level.Name, grid, updatedHand);
                Destroy(gridEditorView.gameObject);
                ShowHandEditor(level.Hand, uniqueColors);
            }
            else
            {
                level = new Level(level.Name, grid, level.Hand);
                Destroy(gridEditorView.gameObject);
                ShowHandEditor(uniqueColors);
            }
        }

        private static HashSet<Color> GetGridUniqueColors(Grid<EditableBlockData> grid)
        {
            var uniqueColors = new HashSet<Color>();
            foreach (var editableBlockData in grid)
            {
                if (editableBlockData.BlockData is ColorBlockData colorBlockData)
                {
                    uniqueColors.Add(colorBlockData.Color);
                }
            }

            return uniqueColors;
        }

        private Hand UpdateHandData(Hand hand, Grid<EditableBlockData> editedGrid)
        {
            var newUniqueColors = GetGridUniqueColors(editedGrid);

            // update colorWeights
            foreach (var color in newUniqueColors.Except(hand.ColorWeightsDict.Keys))
            {
                hand.ColorWeightsDict.TryAdd(color, Hand.DEFAULT_COLOR_WEIGHT);
            }

            foreach (var color in hand.ColorWeightsDict.Keys.Except(newUniqueColors).ToList())
            {
                hand.ColorWeightsDict.Remove(color);
            }
            
            // update initialHand
            for (var i = hand.InitialCustomHand.Count - 1; i >= 0; i--)
            {
                if (!newUniqueColors.Contains(hand.InitialCustomHand[i]))
                {
                    hand.InitialCustomHand.RemoveAt(i);
                }
            }

            return hand;
        }

        private void ShowHandEditor(HashSet<Color> uniqueColors)
        {
            InternalShowHandEditor();
            handEditorView.Initialize(uniqueColors);
        }
        
        private void ShowHandEditor(Hand hand, HashSet<Color> uniqueColors)
        {
            InternalShowHandEditor();
            handEditorView.Initialize(hand, uniqueColors);
        }

        private void InternalShowHandEditor()
        {
            handEditorView = Instantiate(handEditorViewPrefab, transform, false);
            handEditorView.GoBack += (editedHand) =>
            {
                level = new Level(level.Name, level.Grid, editedHand);
                Destroy(handEditorView.gameObject);
                ShowGridEditor(level.Grid);
            };
            handEditorView.Save += async (editedHand) =>
            {
                level = new Level(level.Name, level.Grid, editedHand);
                await SaveLevel(level);
            };
        }

        private void ShowGridEditor(Vector2Int gridSize)
        {
            InternalShowGridEditor();
            gridEditorView.GenerateGrid(gridSize);
        }
    
        private void ShowGridEditor(Grid<EditableBlockData> grid)
        {
            InternalShowGridEditor();
            gridEditorView.GenerateGrid(grid);
        }

        private void OnGridSizeConfirmed(Vector2Int gridSize)
        {
            level = new Level(level.Name, gridSize);
            Destroy(gridSizeSelectorView.gameObject);
            ShowGridEditor(gridSize);
        }

        private void OnGridSizeBackButtonClicked()
        {
            Destroy(gridSizeSelectorView.gameObject);
            ShowCreateOrLoad();
        }

        private void OnGridEditorBackButtonClicked()
        {
            Destroy(gridEditorView.gameObject);
            ShowGridSizeSelector();
        }

        private async void OnSaveLevel(Grid<EditableBlockData> grid)
        {
            level = new Level(level.Name, grid, level.Hand);
            await SaveLevel(level);
        }
    
        private async void OnLoadLevel()
        {
            level = await LoadLevel();
            if (level == null)
            {
                Debug.LogWarning("Error: can't correctly parse selected level");
                return;
            }

            Destroy(createOrLoadView.gameObject);
            ShowGridEditor(level.Grid);
        }

        private async Task SaveLevel(Level level)
        {
            var folder = await FileBrowserUtils.PickFolderToSaveAsync();
            if (string.IsNullOrEmpty(folder))
            {
                return;
            }

            var saveData = LevelSaveMapper.ToSaveData(level);
            var json = JsonUtility.ToJson(saveData, true);
            JsonFileUtils.SaveJsonToFile(Path.Combine(folder, $"{level.Name}.json"), json);
        }

        private async Task<Level> LoadLevel()
        {
            var file = await FileBrowserUtils.PickFileToLoadAsync();
            if (string.IsNullOrEmpty(file))
            {
                return null;
            }

            var json = JsonFileUtils.LoadJsonFromFile(file);
            var levelSaveData = JsonUtility.FromJson<LevelSaveData>(json);
            if (levelSaveData == null)
            {
                return null;
            }
        
            var parsedLevel = LevelSaveMapper.FromSaveData(levelSaveData);
            return parsedLevel;
        }
    }
}
