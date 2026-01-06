using System.Linq;
using System.Threading.Tasks;
using _2025.ColourBlockArrowProto.Scripts;
using _Conveyor.Scripts.Gameplay.VFX;
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

        [Header("Animation/VFX")]
        [SerializeField]
        private ArrowTileAnimationSettings tileAnimationSettings;
        [SerializeField]
        private GameVFXSpawner vfxSpawner;
        
        // temporary variable, here to demo reject animation before a proper solution - Canvas
        // note that as of typing, the reject animation is 0.68f long
        public float delayBeforeNewReject = 1f;
        
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
            
            var threshold = Time.timeSinceLevelLoad - delayBeforeNewReject;
            if (gridBlockView.ColorBlock.LastRejectTime >= threshold)
            {
                return;
            }
            
            OnConveyorBlockReject(conveyorBlockView, gridBlockView.ColorBlock);
        }

        private async Task OnConveyorBlockReject(ConveyorBlockView conveyorBlockView, ColorBlock colorBlock)
        {
            var chain = level.Grid.GetBlockChain(colorBlock);

            var threshold = Time.timeSinceLevelLoad - delayBeforeNewReject;
            if (chain.Any(x => x.LastRejectTime >= threshold))
            {
                return;
            }
            
            foreach (var block in chain)
            {
                block.LastRejectTime = Time.timeSinceLevelLoad;
                
                var view = gameGridView.GetViewForBlock(block);
                view.TileMotions.DoRejectOnBoard();
            }

            await RejectConveyorBlock(conveyorBlockView, chain.Blocks[0]);
        }

        private async Task RejectConveyorBlock(ConveyorBlockView conveyorBlockView, ColorBlock firstBlockInChain)
        {
            conveyorBlockView.ToggleSplineMovement(false);
            conveyorBlockView.ToggleDetection(false);

            var firstChainView = gameGridView.GetViewForBlock(firstBlockInChain);
            var firstChainPos = firstChainView.transform.position;

            var initPos = conveyorBlockView.transform.position;
            
            var sequence = conveyorBlockView.TileMotions.DoRejectFromBoard(
                firstChainPos,
                initPos);

            await sequence.AsyncWaitForCompletion();
            
            conveyorBlockView.ToggleSplineMovement(true);
            conveyorBlockView.ToggleDetection(true);
        }

        private async Task OnConveyorBlockMatch(ConveyorBlockView conveyorBlockView, ColorBlock colorBlock)
        {
            var chain = level.Grid.GetBlockChain(colorBlock);
            RemoveGridBlocks(chain);

            PreEmptCascade(chain);

            var beltPoint = conveyorBlockView.transform.position;
            
            await RemoveConveyorBlock(conveyorBlockView, chain.Blocks[0]);
            await RemoveGridBlocksView(beltPoint, chain);
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
                view.TileMotions.DoPreEmptCascade(i);
                i++;
            }
        }
        
        private async Task RemoveGridBlocksView(Vector3 beltInitialPoint, ColorBlocksChain chain)
        {
            Vector3 lastViewPosition = beltInitialPoint;
            var count = chain.Blocks.Count;
            for (int i = 0; i < count; i++)
            {
                var block = chain.Blocks[i];
                var view = gameGridView.GetViewForBlock(block);
                
                // todo: define next position as block pos + the direction of the block
                Vector3 nextPos;
                if (i + 1 < count)
                {
                    var nextView = gameGridView.GetViewForBlock(chain.Blocks[i + 1]);
                    nextPos = nextView.transform.position;
                }
                else
                {
                    var pos = view.transform.position;
                    var dir = (pos - lastViewPosition).normalized;
                    
                    nextPos = pos + dir * tileAnimationSettings.FinalCascadeJumpDistance;
                }

                var isFinalInCascade = i + 1 == count;
                var tween = view.TileMotions.DoCascade(nextPos, i, isFinalInCascade);

                lastViewPosition = view.transform.position;

                await tween.AsyncWaitForCompletion();
                
                if(i < count - 1)
                    vfxSpawner.SpawnCascadeLanding(nextPos, i);
                else
                    vfxSpawner.SpawnTileFinish(nextPos);
                
                gameGridView.DestroyGridBlockView(block);
            }
        }
    }
}
