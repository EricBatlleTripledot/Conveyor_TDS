using UnityEngine;

namespace Game.BlockRendering
{
    /// <summary>
    /// SO responsible for storing the list of materials per color style
    /// </summary>
    [CreateAssetMenu(
        fileName = "BlockMaterialsList",
        menuName = "Color Block Arrow/Block Materials List",
        order = 0)]
    public class BlockMaterialsList : ScriptableObject
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