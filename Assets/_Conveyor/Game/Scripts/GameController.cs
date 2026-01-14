using System.Threading.Tasks;
using Game.BlockAnimation;
using _Conveyor.Scripts.Gameplay.VFX;
using Game.MeshGeneration;
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

        [Header("ConveyorBelt")]
        [SerializeField]
        private ConveyorFromPoints conveyorFromPoints;
        
        [Header("Animation/VFX")]
        [SerializeField]
        private BlockViewSpawnerView spawnerView;
        [SerializeField]
        private BlockViewStackView stackView;
        [SerializeField]
        private BlockAnimationSettings tileAnimationSettings;
        [SerializeField]
        private GameVFXSpawner vfxSpawner;
        
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

            var splineEval = splineContainer.GetNearestPointTo(stackView.StackPoint, 30);
            stackView.Initialize(splineEval.Item1, splineEval.Item2);
            
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
                
                // todo: call with level json
                conveyorFromPoints.ReadConveyorPointsFromJson();
                conveyorFromPoints.BuildConveyorFromPoints();
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

            spawnerView.AddBlockToSpawnQueue(color, conveyorBlockView);
        }

        private void CheckBlockViewMatch(ConveyorBlockView conveyorBlockView, GridBlockView gridBlockView)
        {
            // Prevent triggering a new animation when the block is already being removed
            if (gridBlockView.IsCascading)
            {
                return;
            }
            
            var canMatch = blockMatchService.CanMatch(conveyorBlockView.ConveyorBlock, gridBlockView.ColorBlock, level.Grid);
            if (!canMatch)
            {
                CheckAndPerformRejectAnim(conveyorBlockView, gridBlockView);
                return;
            }

            OnConveyorBlockMatch(conveyorBlockView, gridBlockView.ColorBlock);
        }

        private void CheckAndPerformRejectAnim(ConveyorBlockView conveyorBlockView, GridBlockView gridBlockView)
        {
            if (!blockMatchService.HasSameColor(conveyorBlockView.ConveyorBlock, gridBlockView.ColorBlock))
            {
                return;
            }
            
            var chain = level.Grid.GetBlockChain(gridBlockView.ColorBlock);
            foreach (var block in chain)
            {
                var view = gameGridView.GetViewForBlock(block);
                view.DoRejectShake();
            }
        }

        private async Task OnConveyorBlockMatch(ConveyorBlockView conveyorBlockView, ColorBlock colorBlock)
        {
            var chain = level.Grid.GetBlockChain(colorBlock);
            RemoveGridBlocks(chain);

            PreEmptCascade(chain);

            await RemoveConveyorBlock(conveyorBlockView, chain.Blocks[0]);
            await RemoveGridBlocksView(chain);
        }

        private async Task RemoveConveyorBlock(ConveyorBlockView conveyorBlockView, ColorBlock firstBlockInChain)
        {
            conveyorBlockView.GridBlockDetected -= CheckBlockViewMatch;
            conveyorBlockView.ToggleSplineMovement(false);

            var firstChainView = gameGridView.GetViewForBlock(firstBlockInChain);
            var firstChainPos = firstChainView.transform.position;

            var tween = conveyorBlockView.TileMotions.DoMoveOntoBoard(firstChainPos);

            await tween.AsyncWaitForCompletion();
            
            // technically this is the first block in the cascade
            vfxSpawner.SpawnCascadeLanding(conveyorBlockView.transform.position, 0);
            
            conveyorBlockView.Destroy();
        }

        private void RemoveGridBlocks(ColorBlocksChain chain)
        {
            foreach (var colorBlock in chain)
            {
                level.Grid.Remove(colorBlock);
            }
        }

        private void PreEmptCascade(ColorBlocksChain chain)
        {
            var i = 0;

            foreach (var colorBlock in chain)
            {
                var view = gameGridView.GetViewForBlock(colorBlock);
                view.TileMotions.DoPreEmptCascade(i, view.ColorBlock.Direction.ToPreEmptIndex());
                i++;
            }
        }
        
        private async Task RemoveGridBlocksView(ColorBlocksChain chain)
        {
            // mark all blocks in chain that they will remove themselves
            foreach (ColorBlock block in chain)
            {
                var view = gameGridView.GetViewForBlock(block);
                view.IsCascading = true;
            }

            var count = chain.Blocks.Count;
            for (int i = 0; i < count; i++)
            {
                var block = chain.Blocks[i];
                var view = gameGridView.GetViewForBlock(block);
                var nextView = i + 1 < count
                    ? gameGridView.GetViewForBlock(chain.Blocks[i + 1])
                    : null;

                var nextPos = GetCascadePosition(i, count, view, nextView);

                var isFinalInCascade = i + 1 == count;
                var tween = view.TileMotions.DoCascade(nextPos, i, isFinalInCascade);
                view.UpdateViewForCascade();

                await tween.AsyncWaitForCompletion();

                HandleVfxAfterCascade(nextPos, i, count);
                
                gameGridView.DestroyGridBlockView(block);
            }
        }

        private Vector3 GetCascadePosition(int i, int count, GridBlockView view, GridBlockView nextView)
        {
            if (i + 1 < count)
            {
                return nextView.transform.position;
            }

            return view.transform.position 
                   + view.ColorBlock.Direction.ToVector3Direction() * tileAnimationSettings.FinalCascadeJumpDistance;
        }

        private void HandleVfxAfterCascade(Vector3 point, int chainIndex, int count)
        {
            if (chainIndex < count - 1)
            {
                vfxSpawner.SpawnCascadeLanding(point, chainIndex);
            }
            else
            {
                if (tileAnimationSettings.ShouldDoShorterCascade(chainIndex))
                {
                    vfxSpawner.SpawnTileFinishShort(point);
                }
                else
                {
                    vfxSpawner.SpawnTileFinish(point);
                }
            }
        }
    }
}
