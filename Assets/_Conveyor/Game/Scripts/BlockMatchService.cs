namespace Game
{
	public class BlockMatchService
	{
		public bool CanMatch(ConveyorBlock conveyorBeltBlock, ColorBlock gridBlock, GameGrid gameGrid)
		{
			return HasSameColor(conveyorBeltBlock, gridBlock) && BlockChainHasValidExitPath(gameGrid, gridBlock);
		}

		private bool HasSameColor(ConveyorBlock conveyorBeltBlock, ColorBlock otherBlock)
		{
			return conveyorBeltBlock.Color == otherBlock.Color;
		}

		private bool BlockChainHasValidExitPath(GameGrid gameGrid, ColorBlock gridBlock)
		{
			return gameGrid.BlockChainHasValidExitPath(gridBlock);
		}
	}
}