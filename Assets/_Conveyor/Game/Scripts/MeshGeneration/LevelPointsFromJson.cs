using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.MeshGeneration
{
    [Serializable]
    public class LevelJson
    {
        public int width;
        public int height;
        public List<BlockJson> blocks;
    }

    [Serializable]
    public class BlockJson
    {
        public int x;
        public int y;
        public string type;
    }

    [CreateAssetMenu(fileName = "ConveyorBeltPoints", menuName = "LevelEditor/ConveyorPointsFromJSON", order = 1)]
    public class LevelPointsFromJson : ScriptableObject
    {
        private static readonly Vector2Int[] CardinalDirections = {
            new(1, 0),
            new(0, 1),
            new(-1, 0),
            new(0, -1)
        };

        [SerializeField]
        public List<Vector2> conveyorPoints = new List<Vector2>();
        [SerializeField]
        public int gridWidth;
        [SerializeField]
        public int gridHeight;
        [SerializeField]
        private TextAsset jsonFile;
        
        [ContextMenu("LoadLevelPointsFromJson")]
        public void LoadLevelPointsFromJson()
        {
            if (!jsonFile) {
                Debug.LogError("No file assigned! Check the jsonFile field.");
                return;
            }

            var level = JsonUtility.FromJson<LevelJson>(jsonFile.text);
            gridWidth = level.width;
            gridHeight = level.height;

            var conveyorBeltPositions = new HashSet<Vector2Int>();
            foreach (var b in level.blocks.Where(b => b.type == "ConveyorBelt")) {
                conveyorBeltPositions.Add(new Vector2Int(b.x, b.y));
            }

            var orderedConveyorBeltPositions = TraceSingleLoop(conveyorBeltPositions, gridWidth, gridHeight);
            // To have the points of the conveyor "centered" in the "cell"
            conveyorPoints.Clear();
            foreach (var conveyorBeltPosition in orderedConveyorBeltPositions) {
                var u = (conveyorBeltPosition.x + 0.5f) / gridWidth;
                var v = (conveyorBeltPosition.y + 0.5f) / gridHeight;
                conveyorPoints.Add(new Vector2(u, v));
            }

            Debug.Log($"Loaded {conveyorPoints.Count} ordered conveyor points from JSON.");
        }

        private List<Vector2Int> TraceSingleLoop(HashSet<Vector2Int> conveyorBeltPositions, int gridWidth, int gridHeight)
        {
            if (conveyorBeltPositions.Count == 0) {
                Debug.LogWarning("Error: No positions for conveyor belt");
                return new List<Vector2Int>();
            }

            var conveyorStartPosition = conveyorBeltPositions
                .OrderBy(p => p.y)
                .ThenBy(p => p.x)
                .First();
            
            bool InBounds(Vector2Int p) => (uint)p.x < (uint)gridWidth && (uint)p.y < (uint)gridHeight;

            if (!TryPickInitialNeighbor(conveyorBeltPositions, conveyorStartPosition, InBounds, out var currentNeighbor)) {
                Debug.LogWarning("Error: ConveyorBelt has some unlinked positions!");
                return new List<Vector2Int>();
            }
            
            var ordered = new List<Vector2Int>(conveyorBeltPositions.Count) { conveyorStartPosition };

            var previousNeighbor = conveyorStartPosition;
            var safety = conveyorBeltPositions.Count + 1; // a simple loop can't exceed its cell count before closing

            while (safety-- > 0) {
                if (currentNeighbor == conveyorStartPosition) {
                    break;
                }

                ordered.Add(currentNeighbor);

                if (!TryPickNextNeighborInLoop(conveyorBeltPositions, currentNeighbor, previousNeighbor, InBounds, out var nextNeighbor)) {
                    Debug.LogWarning($"Conveyor loop invalid around {currentNeighbor}: expected exactly 2 neighbors.");
                    return new List<Vector2Int>();
                }

                previousNeighbor = currentNeighbor;
                currentNeighbor = nextNeighbor;
            }

            // Validate closure + completeness
            if (currentNeighbor != conveyorStartPosition) {
                Debug.LogWarning("Conveyor loop invalid: did not close back to start (safety break).");
                return new List<Vector2Int>();
            }

            if (ordered.Count != conveyorBeltPositions.Count) {
                Debug.LogWarning($"Conveyor loop invalid: visited {ordered.Count}/{conveyorBeltPositions.Count} cells. " + "Likely disconnected parts or a branching/crossing.");
                return new List<Vector2Int>();
            }

            return ordered;
        }
        
        private static bool TryPickNextNeighborInLoop(
            HashSet<Vector2Int> conveyorCells,
            Vector2Int currentConveyorBeltPosition,
            Vector2Int previousConveyorCell,
            Func<Vector2Int, bool> inBounds,
            out Vector2Int nextConveyorCell)
        {
            nextConveyorCell = default;

            // In a valid loop, curr must have exactly 2 neighbors.
            // One is prev; the other is next.
            var neighborCount = 0;
            var foundNext = false;

            foreach (var direction in CardinalDirections) {
                var nextDirection = currentConveyorBeltPosition + direction;
                if (!inBounds(nextDirection) || !conveyorCells.Contains(nextDirection)) {
                    continue;
                }

                neighborCount++;

                if (nextDirection != previousConveyorCell) {
                    nextConveyorCell = nextDirection;
                    foundNext = true;
                }
            }

            return neighborCount == 2 && foundNext;
        }
        
        private bool TryPickInitialNeighbor(HashSet<Vector2Int> conveyorCells, Vector2Int conveyorBeltPosition, Func<Vector2Int, bool> inBounds, out Vector2Int neighbor)
        {
            foreach (var direction in CardinalDirections) {
                var nextDirection = conveyorBeltPosition + direction;
                if (inBounds(nextDirection) && conveyorCells.Contains(nextDirection)) {
                    neighbor = nextDirection;
                    return true;
                }
            }

            neighbor = default;
            return false;
        }
    }
}