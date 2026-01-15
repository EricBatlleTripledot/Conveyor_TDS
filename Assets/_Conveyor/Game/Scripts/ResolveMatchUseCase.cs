namespace Game
{
    public class ResolveMatchUseCase
    {
        public void Execute(GameGrid gameGrid, ColorBlock startBlock)
        {
            var chain = gameGrid.GetBlockChain(startBlock);
            gameGrid.Remove(chain);
        }
    }
}