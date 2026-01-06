using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class ColorBlock
    {
        [SerializeField]
        private Vector2Int position;
        [SerializeField]
        private Color color;
        [SerializeField]
        private BlockDirection direction;

        public Vector2Int Position => position;
        public Color Color => color;
        public BlockDirection Direction => direction;

        public float LastRejectTime;
        
        public ColorBlock(Vector2Int position, Color color, BlockDirection direction)
        {
            this.position = position;
            this.color = color;
            this.direction = direction;
        }

        public override string ToString()
        {
            return $"ColorBlock_{color}_{position}_{direction}";
        }
    }
}
