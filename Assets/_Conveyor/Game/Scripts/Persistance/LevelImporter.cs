using System;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class LevelImporter
    {
        // Must have the SAME names as the level editor
        private enum GridBlockType
        {
            Empty, ConveyorBelt, Color,
        }

        private readonly HandSaveMapper handSaveMapper;

        public LevelImporter(HandSaveMapper handSaveMapper)
        {
            this.handSaveMapper = handSaveMapper;
        }

        public Level FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("Level json is null/empty.", nameof(json));
            }

            var save = JsonUtility.FromJson<LevelSaveData>(json);
            if (save == null)
            {
                throw new InvalidOperationException("Could not deserialize LevelSaveData from json.");
            }

            return FromSaveData(save);
        }

        private Level FromSaveData(LevelSaveData save)
        {
            if (save.width <= 0 || save.height <= 0)
            {
                throw new InvalidOperationException($"Invalid grid size: {save.width}x{save.height}");
            }

            var grid = new GameGrid(save.width, save.height);

            if (save.blocks == null)
            {
                return new Level(save.name, grid);
            }

            foreach (var cell in save.blocks)
            {
                if (!grid.IsValidPosition(cell.x, cell.y))
                {
                    continue;
                }

                if (!Enum.TryParse(cell.type, out GridBlockType type))
                {
                    continue;
                }

                switch (type)
                {
                    case GridBlockType.Color:
                    {
                        var block = CreateColorBlock(cell.x, cell.y, cell.payload);
                        if (block != null)
                        {
                            grid.Set(block);
                        }
                        break;
                    }
                    case GridBlockType.ConveyorBelt:
                    case GridBlockType.Empty:
                    default:
                        break;
                }
            }

            var hand = handSaveMapper.FromSaveData(save.hand);
            // ToDo: I have to think a better way to handle situations where weights are not defined
            if (!hand.ColorWeights.Any())
            {
                hand = new Hand(grid.GetUniqueColors());
            }

            return new Level(save.name, grid, hand);
        }

        private ColorBlock CreateColorBlock(int x, int y, string payloadJson)
        {
            if (string.IsNullOrEmpty(payloadJson))
            {
                return null;
            }

            ColorBlockPayload payload;
            try
            {
                payload = JsonUtility.FromJson<ColorBlockPayload>(payloadJson);
            }
            catch
            {
                return null;
            }

            return new ColorBlock(new Vector2Int(x, y), payload.color, payload.direction);
        }
    }
}
