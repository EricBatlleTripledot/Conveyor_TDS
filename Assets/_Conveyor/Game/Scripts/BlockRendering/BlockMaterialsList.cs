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
        [SerializeField]
        private Material[] particleMaterials;

        public Material GetMaterialForId(int colorId, bool particleShader)
        {
            return particleShader ? particleMaterials[colorId] : materials[colorId];
        }
    }
}