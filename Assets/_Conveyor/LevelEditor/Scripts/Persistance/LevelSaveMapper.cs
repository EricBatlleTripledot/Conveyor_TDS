using System;
using UnityEngine;

namespace LevelEditor
{
    public static class LevelSaveMapper
    {
        public static LevelSaveData ToSaveData(Level level)
        {
            var save = new LevelSaveData
            {
                name = level.Name,
                width = level.Width,
                height = level.Height,
                hand = HandSaveMapper.ToHandSaveData(level.Hand)
            };
            

            var grid = level.Grid;

            for (var y = 0; y < grid.Height; y++)
            {
                for (var x = 0; x < grid.Width; x++)
                {
                    var block = grid.Get(x, y)?.BlockData ?? new EmptyBlockData(new Vector2Int(x, y));

                    var cell = new BlockSaveData
                    {
                        x = x,
                        y = y,
                        type = block.BlockType.ToString(),
                        payload = null
                    };

                    if (block is ColorBlockData color)
                    {
                        cell.payload = JsonUtility.ToJson(new ColorBlockPayload
                        {
                            color = color.Color,
                            direction = color.Direction
                        });
                    }

                    save.blocks.Add(cell);
                }
            }

            return save;
        }

        public static Level FromSaveData(LevelSaveData save)
        {
            var grid = new Grid<EditableBlockData>(save.width, save.height);

            var i = 0;
            for (var y = 0; y < save.height; y++)
            {
                for (var x = 0; x < save.width; x++)
                {
                    var cell = save.blocks[i++];
                    var pos = new Vector2Int(x, y);

                    Enum.TryParse<GridBlockType>(cell.type, out var type);

                    var block = type switch
                    {
                        GridBlockType.Color => CreateColorBlock(pos, cell.payload),
                        GridBlockType.ConveyorBelt => new ConveyorBeltBlockData(pos),
                        _ => new EmptyBlockData(pos)
                    };

                    grid.Set(x, y, new EditableBlockData(block));
                }
            }

            var hand = HandSaveMapper.FromHandSaveData(save.hand);
            var level = new Level(save.name, grid, hand);
            return level;
        }

        private static GridBlockData CreateColorBlock(Vector2Int pos, string payloadJson)
        {
            if (string.IsNullOrEmpty(payloadJson))
            {
                return new EmptyBlockData(pos);
            }

            var payload = JsonUtility.FromJson<ColorBlockPayload>(payloadJson);
            if (payload == null)
            {
                return new EmptyBlockData(pos);
            }

            return new ColorBlockData(pos, payload.color, payload.direction);
        }
    }
}