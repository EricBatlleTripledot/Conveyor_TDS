using System;
using UnityEngine;

namespace Game
{
	public static class BlockDirectionExtensions
	{
		public static Vector2Int ToVector2Int(this BlockDirection blockDirection)
		{
			return blockDirection switch
			{
				BlockDirection.None => new Vector2Int(0, 0),
				BlockDirection.Up => new Vector2Int(0, 1),
				BlockDirection.Down => new Vector2Int(0, -1),
				BlockDirection.Left => new Vector2Int(-1, 0),
				BlockDirection.Right => new Vector2Int(1, 0),
				_ => throw new ArgumentOutOfRangeException(nameof(blockDirection), blockDirection, null)
			};
		}
		
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
		
		
		public static float ToUvRotation(this BlockDirection blockDirection)
		{
			return blockDirection switch
			{
				BlockDirection.None => 0,
				BlockDirection.Up => 90,
				BlockDirection.Down => 270,
				BlockDirection.Left => 180,
				BlockDirection.Right => 0,
				_ => throw new ArgumentOutOfRangeException(nameof(blockDirection), blockDirection, null)
			};
		}
		
		
		public static Vector3 ToEuler(this BlockDirection blockDirection)
		{
			
			return blockDirection switch
			{
				BlockDirection.None => Vector3.zero,
				BlockDirection.Up => new Vector3(0, 270f, 0),
				BlockDirection.Down => new Vector3(0, 90f, 0),
				BlockDirection.Left => new Vector3(0, 180f, 0),
				BlockDirection.Right => Vector3.zero,
				_ => throw new ArgumentOutOfRangeException(nameof(blockDirection), blockDirection, null)
			};
		}
	}
}