using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Game.Tests
{
    public class TestBlockMatchService
    {
        private BlockMatchService blockMatchService;
        private ResolveMatchUseCase resolveMatchUseCase;
        
        [SetUp]
        public void Setup()
        {
            blockMatchService = new BlockMatchService();
            resolveMatchUseCase = new ResolveMatchUseCase();
        }
        
        [Test]
        public void When_ConveyorBlockMatchesColorButChainIsBlocked_Expects_CannotMatch()
        {
            /*
                   G → [ G→ ][ G→ ][ G→ ][ B↓ ]
                       [  . ][  . ][  . ][ B↓ ]
                       [  . ][  . ][  . ][ B↓ ]
            */

            var gameGrid = new GameGrid(4,3);
            gameGrid.Set(new ColorBlock(new Vector2Int(0, 2), Color.green, BlockDirection.Right));
            gameGrid.Set(new ColorBlock(new Vector2Int(1, 2), Color.green, BlockDirection.Right));
            gameGrid.Set(new ColorBlock(new Vector2Int(2, 2), Color.green, BlockDirection.Right));
            gameGrid.Set(new ColorBlock(new Vector2Int(3, 2), Color.blue, BlockDirection.Down));
            gameGrid.Set(new ColorBlock(new Vector2Int(3, 1), Color.blue, BlockDirection.Down));
            gameGrid.Set(new ColorBlock(new Vector2Int(3, 0), Color.blue, BlockDirection.Down));
            var conveyorBlock = new ConveyorBlock(Color.green);
            var matchingBlock = gameGrid.Get(0, 2);
            var canMatch = blockMatchService.CanMatch(conveyorBlock, matchingBlock, gameGrid);
            Assert.IsFalse(canMatch);
        }
        
        [Test]
        public void When_ResolveBlockingChain_Expects_NextMatchBecomesPossible()
        {
            /*
                                             B
                                             ↓
                        [ G→ ][ G→ ][ G→ ][ B↓ ]
                    G → [  . ][  . ][  . ][ B↓ ]
                        [  . ][  . ][  . ][ B↓ ]
            */

            var gameGrid = new GameGrid(4,3);
            gameGrid.Set(new ColorBlock(new Vector2Int(0, 2), Color.green, BlockDirection.Right));
            gameGrid.Set(new ColorBlock(new Vector2Int(1, 2), Color.green, BlockDirection.Right));
            gameGrid.Set(new ColorBlock(new Vector2Int(2, 2), Color.green, BlockDirection.Right));
            gameGrid.Set(new ColorBlock(new Vector2Int(3, 2), Color.blue, BlockDirection.Down));
            gameGrid.Set(new ColorBlock(new Vector2Int(3, 1), Color.blue, BlockDirection.Down));
            gameGrid.Set(new ColorBlock(new Vector2Int(3, 0), Color.blue, BlockDirection.Down));
            var conveyorBlock = new ConveyorBlock(Color.blue);
            var matchingBlock = gameGrid.Get(3, 2);
            var canMatch = blockMatchService.CanMatch(conveyorBlock, matchingBlock, gameGrid);
            Assert.IsTrue(canMatch);
            resolveMatchUseCase.Execute(gameGrid, matchingBlock);
            Assert.IsNull(gameGrid.Get(3, 2));
            Assert.IsNull(gameGrid.Get(3, 1));
            Assert.IsNull(gameGrid.Get(3, 0));
            Assert.IsNotNull(gameGrid.Get(0, 2));
            Assert.IsNotNull(gameGrid.Get(1, 2));
            Assert.IsNotNull(gameGrid.Get(2, 2));
            /*
                   G → [ G→ ][ G→ ][ G→ ][  . ]
                       [  . ][  . ][  . ][  . ]
                       [  . ][  . ][  . ][  . ]
            */
            conveyorBlock = new ConveyorBlock(Color.green);
            matchingBlock = gameGrid.Get(0, 2);
            canMatch = blockMatchService.CanMatch(conveyorBlock, matchingBlock, gameGrid);
            Assert.IsTrue(canMatch);
            resolveMatchUseCase.Execute(gameGrid, matchingBlock);
            Assert.IsNull(gameGrid.Get(0, 2));
            Assert.IsNull(gameGrid.Get(1, 2));
            Assert.IsNull(gameGrid.Get(2, 2));
            Assert.IsTrue(gameGrid.IsEmpty());
        }
        
        [Test]
        public void When_MultipleMatchingChainsExist_Expects_OnlyUnblockedChainCanMatch()
        {
            /*
                   G → [ G→ ][ G→ ][ G→ ][ B↓ ][ G↓ ]
                       [  . ][  . ][  . ][ B↓ ][ G↓ ]
                       [  . ][  . ][  . ][ B↓ ][ G↓ ]
            */

            var gameGrid = new GameGrid(5,3);
            gameGrid.Set(new ColorBlock(new Vector2Int(0, 2), Color.green, BlockDirection.Right));
            gameGrid.Set(new ColorBlock(new Vector2Int(1, 2), Color.green, BlockDirection.Right));
            gameGrid.Set(new ColorBlock(new Vector2Int(2, 2), Color.green, BlockDirection.Right));
            gameGrid.Set(new ColorBlock(new Vector2Int(3, 2), Color.blue, BlockDirection.Down));
            gameGrid.Set(new ColorBlock(new Vector2Int(3, 1), Color.blue, BlockDirection.Down));
            gameGrid.Set(new ColorBlock(new Vector2Int(3, 0), Color.blue, BlockDirection.Down));
            gameGrid.Set(new ColorBlock(new Vector2Int(4, 2), Color.green, BlockDirection.Down));
            gameGrid.Set(new ColorBlock(new Vector2Int(4, 1), Color.green, BlockDirection.Down));
            gameGrid.Set(new ColorBlock(new Vector2Int(4, 0), Color.green, BlockDirection.Down));
            var conveyorBlock = new ConveyorBlock(Color.green);
            var matchingBlock = gameGrid.Get(0, 2);
            var canMatch = blockMatchService.CanMatch(conveyorBlock, matchingBlock, gameGrid);
            Assert.IsFalse(canMatch);

            /*
                                                  G
                                                  ↓
                       [ G→ ][ G→ ][ G→ ][ B↓ ][ G↓ ]
                       [  . ][  . ][  . ][ B↓ ][ G↓ ]
                       [  . ][  . ][  . ][ B↓ ][ G↓ ]
            */
            
            matchingBlock = gameGrid.Get(4, 2);
            canMatch = blockMatchService.CanMatch(conveyorBlock, matchingBlock, gameGrid);
            Assert.IsTrue(canMatch);
        }
        
        [Test]
        public void When_ChainContainsGaps_Expects_ChainCanStillBeResolved()
        {
            /*
                                             B
                                             ↓
                        [ G→ ][ G→ ][ G→ ][ B↓ ][ G↓ ]
                   G →  [  . ][  . ][  . ][ B↓ ][ G↓ ]
                        [  . ][  . ][  . ][ B↓ ][ G↓ ]
            */

            var gameGrid = new GameGrid(5,3);
            gameGrid.Set(new ColorBlock(new Vector2Int(0, 2), Color.green, BlockDirection.Right));
            gameGrid.Set(new ColorBlock(new Vector2Int(1, 2), Color.green, BlockDirection.Right));
            gameGrid.Set(new ColorBlock(new Vector2Int(2, 2), Color.green, BlockDirection.Right));
            gameGrid.Set(new ColorBlock(new Vector2Int(3, 2), Color.blue, BlockDirection.Down));
            gameGrid.Set(new ColorBlock(new Vector2Int(3, 1), Color.blue, BlockDirection.Down));
            gameGrid.Set(new ColorBlock(new Vector2Int(3, 0), Color.blue, BlockDirection.Down));
            gameGrid.Set(new ColorBlock(new Vector2Int(4, 2), Color.green, BlockDirection.Down));
            gameGrid.Set(new ColorBlock(new Vector2Int(4, 1), Color.green, BlockDirection.Down));
            gameGrid.Set(new ColorBlock(new Vector2Int(4, 0), Color.green, BlockDirection.Down));
            var conveyorBlock = new ConveyorBlock(Color.blue);
            var matchingBlock = gameGrid.Get(3, 2);
            var canMatch = blockMatchService.CanMatch(conveyorBlock, matchingBlock, gameGrid);
            Assert.IsTrue(canMatch);
            resolveMatchUseCase.Execute(gameGrid, matchingBlock);

            /*
                   G →  [ G→ ][ G→ ][ G→ ][  . ][ G↓ ]
                        [  . ][  . ][  . ][  . ][ G↓ ]
                        [  . ][  . ][  . ][  . ][ G↓ ]
            */
            
            conveyorBlock = new ConveyorBlock(Color.green);
            matchingBlock = gameGrid.Get(0, 2);
            canMatch = blockMatchService.CanMatch(conveyorBlock, matchingBlock, gameGrid);
            Assert.IsTrue(canMatch);
            resolveMatchUseCase.Execute(gameGrid, matchingBlock);
            Assert.IsNull(gameGrid.Get(0, 2));
            Assert.IsNull(gameGrid.Get(1, 2));
            Assert.IsNull(gameGrid.Get(2, 2));
            Assert.IsNull(gameGrid.Get(4, 2));
            Assert.IsNull(gameGrid.Get(4, 1));
            Assert.IsNull(gameGrid.Get(4, 0));
            Assert.IsTrue(gameGrid.IsEmpty());
        }
        
        [Test]
        public void When_ChainHasBouncePatternAndEnteredFromEdge_Expects_PartialResolution()
        {
            /*
                   G → [ G→ ][ ←G ][ G→ ]
            */

            var gameGrid = new GameGrid(3,1);
            gameGrid.Set(new ColorBlock(new Vector2Int(0, 0), Color.green, BlockDirection.Right));
            gameGrid.Set(new ColorBlock(new Vector2Int(1, 0), Color.green, BlockDirection.Left));
            gameGrid.Set(new ColorBlock(new Vector2Int(2, 0), Color.green, BlockDirection.Right));
            var conveyorBlock = new ConveyorBlock(Color.green);
            var matchingBlock = gameGrid.Get(0, 0);
            // ToDo: This is not working since it calculates the exit with previous blocks, not removing them while calculating
            var canMatch = blockMatchService.CanMatch(conveyorBlock, matchingBlock, gameGrid);
            Assert.IsTrue(canMatch);
            resolveMatchUseCase.Execute(gameGrid, matchingBlock);
            Assert.IsNull(gameGrid.Get(0, 0));
            Assert.IsNull(gameGrid.Get(1, 0));
            Assert.IsNotNull(gameGrid.Get(2, 0));
        }
        
        [Test]
        public void When_ChainHasBouncePatternAndEnteredFromMiddle_Expects_FullResolution()
        {
            /*
                             G
                             ↓
                    [ G→ ][ ←G ][ G→ ]
            */

            var gameGrid = new GameGrid(3,1);
            gameGrid.Set(new ColorBlock(new Vector2Int(0, 0), Color.green, BlockDirection.Right));
            gameGrid.Set(new ColorBlock(new Vector2Int(1, 0), Color.green, BlockDirection.Left));
            gameGrid.Set(new ColorBlock(new Vector2Int(2, 0), Color.green, BlockDirection.Right));
            var conveyorBlock = new ConveyorBlock(Color.green);
            var matchingBlock = gameGrid.Get(1, 0);
            var canMatch = blockMatchService.CanMatch(conveyorBlock, matchingBlock, gameGrid);
            Assert.IsTrue(canMatch);
            resolveMatchUseCase.Execute(gameGrid, matchingBlock);
            Assert.IsNull(gameGrid.Get(0, 0));
            Assert.IsNull(gameGrid.Get(1, 0));
            Assert.IsNull(gameGrid.Get(2, 0));
        }
    }
}