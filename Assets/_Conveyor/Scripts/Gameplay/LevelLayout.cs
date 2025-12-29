using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level_", menuName = "Level Layout", order = 0)]
public class LevelLayout : ScriptableObject
{
    [SerializeField]
    private Dictionary<CellView, Vector2Int> arrowBlocks;

    public Dictionary<CellView, Vector2Int> ArrowBlocks => arrowBlocks;
}