using System;
using UnityEngine;

namespace _Conveyor.Game.Scripts.TileRendering
{
    /// <summary>
    /// SO responsible for storing the list of materials per color style
    /// </summary>
    [CreateAssetMenu(
        fileName = "BlockViewMaterialsList",
        menuName = "Color Block Arrow/BlockView Materials List",
        order = 0)]
    public class BlockViewMaterialsList : ScriptableObject
    {
        [SerializeField]
        private Material[] materials;

        // placeholder for Color Enum
        public Material GetMaterialForColor(int colorId)
        {
            return materials[colorId];
        }
    }
}