using System;
using UnityEngine;

namespace _Conveyor.Game.Scripts.TileRendering
{
    /// <summary>
    /// SO responsible for storing the list of materials per color style
    /// </summary>
    [CreateAssetMenu(
        fileName = "TileMaterialsList",
        menuName = "Color Block Arrow/Tile Materials List",
        order = 0)]
    public class TileMaterialsList : ScriptableObject
    {
        [SerializeField]
        private Material[] materials;

        // placeholder for Color Enum
        public Material GetMaterialForId(int colorId)
        {
            return materials[colorId];
        }
    }
}