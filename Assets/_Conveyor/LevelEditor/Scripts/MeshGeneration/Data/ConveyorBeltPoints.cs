using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    [CreateAssetMenu(fileName = "ConveyorBeltPoints", menuName = "LevelEditor/ConveyorBeltPoints", order = 1)]
    public class ConveyorBeltPoints : ScriptableObject 
    {
        public List<Vector2> ConveyorPoints;
    }
}
