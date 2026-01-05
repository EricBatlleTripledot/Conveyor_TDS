using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        [Header("GameGrid")]
        [SerializeField]
        private GameGridView gameGridView;

        [Header("Hand")]
        [SerializeField]
        private HandView handView;

        [Header("ConveyorBlockView")]
        [SerializeField]
        private ConveyorBlockView conveyorBlockViewPrefab;
        [SerializeField]
        private SplineContainer splineContainer;

        [Header("DEBUG")]
        [SerializeField]
        private TextAsset serializedLevel;

        private readonly BlockMatchService blockMatchService = new BlockMatchService();
        private readonly HandSaveMapper handSaveMapper = new HandSaveMapper();
        private readonly IRandomProvider randomProvider = new UnityRandomProvider();

        private LevelImporter levelImporter;
        private ConveyorBlockViewFactory conveyorBlockViewFactory;
        private HandService handService;

        [SerializeField]
        private Level level;

        private void Awake()
        {
            levelImporter = new LevelImporter(handSaveMapper);
            handService = new HandService(randomProvider);
            conveyorBlockViewFactory = new ConveyorBlockViewFactory(conveyorBlockViewPrefab, splineContainer);

            handView.ColorSelected += OnHandColorSelected;
            
            if (serializedLevel == null)
            {
                var gameGrid = new GameGrid(3,2);
                gameGrid.Set(new ColorBlock(new Vector2Int(0, 0), Color.red, BlockDirection.Up));
                gameGrid.Set(new ColorBlock(new Vector2Int(1, 0), Color.blue, BlockDirection.Right));
                gameGrid.Set(new ColorBlock(new Vector2Int(2, 0), Color.blue, BlockDirection.Up));
                gameGrid.Set(new ColorBlock(new Vector2Int(0, 1), Color.red, BlockDirection.Up));
                gameGrid.Set(new ColorBlock(new Vector2Int(2, 1), Color.blue, BlockDirection.Up));

                level = new Level("defaultName", gameGrid);
            }
            else
            {
                level = levelImporter.FromJson(serializedLevel.text);
            }
        }

        private void Start()
        {
            gameGridView.GenerateGrid(level.Grid);
            while (handView.HasEmptySlots)
            {
                AddNextConveyorBlockButton();
            }
        }

        private void OnHandColorSelected(HandButtonView handButtonView, Color color)
        {
            handButtonView.Clear();
            CreateConveyorBlockView(color);
            AddNextConveyorBlockButton();
        }

        private void AddNextConveyorBlockButton()
        {
            var nextConveyorButtonColor = handService.DequeueNextColor(level.Hand);
            handView.AddConveyorBlockButton(new ConveyorBlock(nextConveyorButtonColor));
        }

        private void Update()
        {
            if (Keyboard.current[Key.R].wasPressedThisFrame)
            {
                CreateConveyorBlockView(Color.red);
                return;
            }
            
            if (Keyboard.current[Key.B].wasPressedThisFrame)
            {
                CreateConveyorBlockView(Color.blue);
                return;
            }
            
            if (Keyboard.current[Key.G].wasPressedThisFrame)
            {
                CreateConveyorBlockView(Color.green);
                return;
            }
        }
        
        private void CreateConveyorBlockView(Color color)
        {
            var conveyorBlockView = conveyorBlockViewFactory.Create(new ConveyorBlock(color));
            conveyorBlockView.GridBlockDetected += CheckBlockViewMatch;
            conveyorBlockView.Launch();
        }

        private void CheckBlockViewMatch(ConveyorBlockView conveyorBlockView, GridBlockView gridBlockView)
        {
            var canMatch = blockMatchService.CanMatch(conveyorBlockView.ConveyorBlock, gridBlockView.ColorBlock, level.Grid);
            if (!canMatch)
            {
                return;
            }

            OnConveyorBlockMatch(conveyorBlockView, gridBlockView.ColorBlock);
        }

        private void OnConveyorBlockMatch(ConveyorBlockView conveyorBlockView, ColorBlock colorBlock)
        {
            var chain = level.Grid.GetBlockChain(colorBlock);
            RemoveGridBlocks(chain);
            RemoveConveyorBlock(conveyorBlockView);
            RemoveGridBlocksView(chain);
        }

        private void RemoveConveyorBlock(ConveyorBlockView conveyorBlockView)
        {
            conveyorBlockView.GridBlockDetected -= CheckBlockViewMatch;
            conveyorBlockView.Destroy();
        }

        private void RemoveGridBlocks(ColorBlocksChain chain)
        {
            foreach (var colorBlock in chain)
            {
                level.Grid.Remove(colorBlock);
            }
        }

        private async Task RemoveGridBlocksView(ColorBlocksChain chain)
        {
            foreach (var colorBlock in chain)
            {
                gameGridView.DestroyGridBlockView(colorBlock);
                await Task.Delay(500);
            }
        }
    }
}
