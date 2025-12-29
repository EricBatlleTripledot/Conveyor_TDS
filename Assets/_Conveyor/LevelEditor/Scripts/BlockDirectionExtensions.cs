using System;

namespace LevelEditor
{
	public static class BlockDirectionExtensions
	{
		public static string ToSymbolString(this BlockDirection blockDirection)
		{
			return blockDirection switch
			{
				BlockDirection.None => "",
				BlockDirection.Up => "^",
				BlockDirection.Down => "v",
				BlockDirection.Left => "<",
				BlockDirection.Right => ">",
				_ => throw new ArgumentOutOfRangeException(nameof(blockDirection), blockDirection, null)
			};
		}
	}
}