using System;
using UnityEngine;

namespace _Conveyor.Game.Scripts.TileRendering
{
    /// <summary>
    /// SO in charge of holding the UVs needed for our different icons on a BlockView's Tile
    /// </summary>
    [CreateAssetMenu(
        fileName = "TileIconConfig", 
        menuName = "Color Block Arrow/Tile Icon Config",
        order = 0)]
    public class TileIconConfig : ScriptableObject
    {
        private static readonly int TextureID = Shader.PropertyToID("_IconTex");
        private static readonly int TilingID = Shader.PropertyToID("_Icon_Tiling");
        private static readonly int OffsetID = Shader.PropertyToID("_Icon_Offset");
        private static readonly int RotationID = Shader.PropertyToID("_Icon_Rotation");
        
        [Header("Arrows")]
        [SerializeField]
        private Texture2D arrowTexture;

        [Space(15)]
        [SerializeField]
        private IconConfig upArrowConfig;
        [SerializeField]
        private IconConfig downArrowConfig;
        [SerializeField]
        private IconConfig leftArrowConfig;
        [SerializeField]
        private IconConfig rightArrowConfig;
        
        [Header("Dot")]
        [SerializeField]
        private Texture2D dotTexture;

        [Space(15)]
        [SerializeField]
        private IconConfig dotConfig;
        
        [Serializable]
        public struct IconConfig
        {
            public Vector2 tiling;
            public Vector2 offset;
            public float rotation;
        }
        
        // None, Up, Down, Left, Right
        public void SetupPropertyBlockForArrow( MaterialPropertyBlock propertyBlock, int dir)
        {
            var config = dir switch
            {
                // 0 == none
                1 => upArrowConfig,
                2 => downArrowConfig,
                3 => leftArrowConfig,
                4 => rightArrowConfig,
                _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid dir index")
            };
            
            propertyBlock.SetTexture(TextureID, arrowTexture);
            propertyBlock.SetVector(TilingID, config.tiling);
            propertyBlock.SetVector(OffsetID, config.offset);
            propertyBlock.SetFloat(RotationID, config.rotation);
        }

        public void SetupPropertyBlockForDot(MaterialPropertyBlock propertyBlock)
        {
            propertyBlock.SetTexture(TextureID, dotTexture);
            propertyBlock.SetVector(TilingID, dotConfig.tiling);
            propertyBlock.SetVector(OffsetID, dotConfig.offset);
            propertyBlock.SetFloat(RotationID, dotConfig.rotation);
        }
    }
}