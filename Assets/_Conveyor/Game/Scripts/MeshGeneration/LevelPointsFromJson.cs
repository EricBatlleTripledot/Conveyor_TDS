using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        public List<Vector2> ConveyorPoints;
        [SerializeField]
        private TextAsset jsonFile;
        
        public void LoadLevelPointsFromJson()
        {
            if (!jsonFile) {
                Debug.LogError("No file assigned! Check the jsonFile field.");
                return;
            }

            var level = JsonUtility.FromJson<LevelJson>(jsonFile.text);

            var isConv = new bool[level.width, level.height];
            foreach (var b in level.blocks.Where(b => b.type == "ConveyorBelt")) {
                isConv[b.x, b.y] = true;
            }

            var loop = TraceSingleLoop(isConv, level.width, level.height);

            ConveyorPoints.Clear();
            foreach (var c in loop) {
                var u = (c.x + 0.5f) / level.width;
                var v = (c.y + 0.5f) / level.height;
                ConveyorPoints.Add(new Vector2(u, v));
            }

            Debug.Log($"Loaded {ConveyorPoints.Count} ordered conveyor points from JSON.");
        }

        private static List<Vector2Int> TraceSingleLoop(bool[,] isConveyor, int width, int height)
        {
            static bool InBounds(Vector2Int p, int w, int h) => (uint)p.x < (uint)w && (uint)p.y < (uint)h;

            // Pick deterministic start: lowest y, then lowest x
            Vector2Int start = new(-1, -1);
            for (int y = 0; y < height && start.x < 0; y++) {
                for (int x = 0; x < width; x++) {
                    if (!isConveyor[x, y]) {
                        continue;
                    }
                    start = new Vector2Int(x, y);
                    break;
                }
            }

            var result = new List<Vector2Int>();
            if (start.x < 0) {
                return result;
            }

            // Prefer a consistent initial direction order
            Vector2Int[] dirs = {
                new(1, 0), // right
                new(0, 1), // up
                new(-1, 0), // left
                new(0, -1) // down
            };

            Vector2Int curr = start;

            // Find first neighbor
            Vector2Int next = default;
            bool found = false;
            foreach (var d in dirs) {
                var n = curr + d;
                if (InBounds(n, width, height) && isConveyor[n.x, n.y]) {
                    next = n;
                    found = true;
                    break;
                }
            }
            if (!found) {
                return result;
            }

            result.Add(curr);

            int safety = width * height * 4;
            while (safety-- > 0) {
                var prev = curr;
                curr = next;

                if (curr == start)
                    break;
                result.Add(curr);

                // Choose next neighbor that isn't prev (single loop => exactly one)
                Vector2Int candidate = default;
                int candidates = 0;

                foreach (var d in dirs) {
                    var n = curr + d;
                    if (!InBounds(n, width, height) || !isConveyor[n.x, n.y])
                        continue;
                    if (n == prev)
                        continue;

                    candidate = n;
                    candidates++;
                }

                if (candidates == 0)
                    break;

                // If candidates > 1 (shouldn't happen for a simple loop), we still pick deterministically.
                next = candidate;
            }

            return result;
        }
    }
}