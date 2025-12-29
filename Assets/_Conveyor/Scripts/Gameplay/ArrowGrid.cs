using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.Serialization;

public class ArrowGrid : MonoBehaviour
{
    public class ArrowCell
    {
        public Vector2Int Coord;
        public BlockColor Color;
        public ArrowDirection Direction;
        public bool Cleared;
    }

    [Header("Grid Settings")]
    [SerializeField]
    private float cellSize = 1f;
    [SerializeField]
    private float alignmentThreshold = 0.05f;

    [Header("Visuals")]
    [SerializeField]
    private CellView arrowPrefab;
    [SerializeField]
    private GameObject conveyorBlockPrefab;
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private Transform[] conveyorPathPoints;
    [SerializeField]
    private Transform conveyorStart;
    [SerializeField]
    private float conveyorDuration = 0.25f;
    [SerializeField]
    private float clearingDuration = 0.12f;

    [Header("Animation")]
    [SerializeField]
    private AnimationView animationView;

    private readonly Dictionary<Vector2Int, ArrowCell> arrowBlocks = new();
    private readonly Dictionary<Vector2Int, CellView> cellViews = new();

    private static readonly Dictionary<ArrowDirection, Vector2Int> DirToOffset = new() {
        { ArrowDirection.Up, Vector2Int.up },
        { ArrowDirection.Down, Vector2Int.down },
        { ArrowDirection.Left, Vector2Int.left },
        { ArrowDirection.Right, Vector2Int.right },
    };
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    private Vector3 CoordToWorld(Vector2Int coord) =>
        transform.position + new Vector3(coord.x * cellSize, coord.y * cellSize, 0f);

    private void Start()
    {
        SeedDemoUnlockThenClear();
    }

    private void Update()
    {
        //HACKY WAY OF SPAWNING FOR NOW
        if (Input.GetKeyDown(KeyCode.B)) {
            ApplyConveyor(BlockColor.Blue);
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            ApplyConveyor(BlockColor.Red);
        }
        if (Input.GetKeyDown(KeyCode.G)) {
            ApplyConveyor(BlockColor.Green);
        }
    }

    private void AddArrow(Vector2Int coord, BlockColor color, ArrowDirection direction)
    {
        var cell = new ArrowCell { Coord = coord, Color = color, Direction = direction, Cleared = false };
        arrowBlocks[coord] = cell;

        if (arrowPrefab) {
            var view = Instantiate(arrowPrefab, CoordToWorld(coord), Quaternion.identity, transform);
            view.SetRotation(direction);
            view.SetColor(ToUnityColor(color));
            cellViews[coord] = view;
        }
    }

    // TODO REPLACE WITH LEVEL DATA LOADING
    private void SeedDemoUnlockThenClear()
    {
        arrowBlocks.Clear();

        foreach (var view in cellViews.Values) {
            if (view) {
                Destroy(view.gameObject);
            }
        }
        cellViews.Clear();

        // RED 
        AddArrow(new Vector2Int(2, 0), BlockColor.Red, ArrowDirection.Up);
        AddArrow(new Vector2Int(2, 1), BlockColor.Red, ArrowDirection.Up);
        AddArrow(new Vector2Int(2, 2), BlockColor.Red, ArrowDirection.Up);

        // BLUE
        AddArrow(new Vector2Int(2, 3), BlockColor.Blue, ArrowDirection.Up);
        AddArrow(new Vector2Int(2, 4), BlockColor.Blue, ArrowDirection.Right);
        AddArrow(new Vector2Int(3, 4), BlockColor.Blue, ArrowDirection.Right);
        AddArrow(new Vector2Int(4, 4), BlockColor.Blue, ArrowDirection.Up);
        
        // GREEN
        AddArrow(new Vector2Int(4, 5), BlockColor.Green, ArrowDirection.Up);
    }

    public void ApplyConveyor(BlockColor color)
    {
        ApplyConveyorAnimation(color).Forget();
    }

    private bool IsAligned(Vector3 a, Vector3 b, float tolerance)
    {
        return Mathf.Abs(a.x - b.x) <= tolerance
               || Mathf.Abs(a.y - b.y) <= tolerance;
    }

    private async UniTask ApplyConveyorAnimation(BlockColor color)
    {
        var block = Instantiate(conveyorBlockPrefab, spawnPoint.position, Quaternion.identity);

        var mat = Instantiate(conveyorBlockPrefab.GetComponent<MeshRenderer>().sharedMaterial);
        mat.SetColor(BaseColor, ToUnityColor(color));
        block.GetComponent<MeshRenderer>().material = mat;

        var token = this.GetCancellationTokenOnDestroy();

        await MoveTo(block.transform, conveyorStart.position, conveyorDuration, token);
        while (!token.IsCancellationRequested) {
            for (int i = 0; i < conveyorPathPoints.Length; i++) {
                //CONVEYOR ANIMATION
                var conveyorPosition = conveyorPathPoints[i].position;
                await MoveTo(block.transform, conveyorPosition, conveyorDuration, token);

                if (conveyorPathPoints[i] == conveyorPathPoints.Last()) {
                    await animationView.PlayExitAnimation(block.transform);
                    Destroy(block);
                    return;
                }

                if (TryFindEntry(color, out var entry)) {
                    var entryPos = CoordToWorld(entry.Coord);

                    if (IsAligned(block.transform.position, entryPos, alignmentThreshold)) {
                        await AnimateAndClearFromEntry(block.transform, entry, token);
                        Destroy(block);
                        return;
                    }
                }
            }
        }

        if (block) {
            Destroy(block);
        }
    }

    private bool TryFindEntry(BlockColor color, out ArrowCell entry)
    {
        foreach (var arrowCell in arrowBlocks.Values.Where(arrowCell => !arrowCell.Cleared && arrowCell.Color == color && IsClearable(arrowCell))) {
            entry = arrowCell;
            return true;
        }

        entry = null;
        return false;
    }

    private bool IsClearable(ArrowCell a)
    {
        var direction = DirToOffset[a.Direction];
        var coord = a.Coord + direction;

        while (arrowBlocks.TryGetValue(coord, out var other)) {
            if (!other.Cleared && other.Color != a.Color) {
                return false;
            }

            coord += direction;
        }
        return true;
    }

    private void ClearAlongPath(ArrowCell start)
    {
        var current = start;

        while (current is { Cleared: false }) {
            if (!IsClearable(current)) {
                break;
            }

            current.Cleared = true;

            if (cellViews.TryGetValue(current.Coord, out var view) && view) {
                Destroy(view.gameObject);
                cellViews.Remove(current.Coord);
            }

            var nextCoord = current.Coord + DirToOffset[current.Direction];

            if (!arrowBlocks.TryGetValue(nextCoord, out var next)) {
                break;
            }
            if (next.Cleared) {
                break;
            }
            if (next.Color != current.Color) {
                break;
            }

            current = next;
        }
    }

    //TODO MOVE INTO SEPARATE VIEW
    private async UniTask MoveTo(Transform t, Vector3 pos, float duration, CancellationToken token)
    {
        t.DOKill();

        if (token.IsCancellationRequested) {
            return;
        }

        var tween = t.DOMove(pos, duration).SetEase(Ease.Linear);

        await tween.AsyncWaitForCompletion();
    }

    private async UniTask MoveAxisOnly(Transform t, Vector3 target, float duration, CancellationToken token, bool xFirst = true)
    {
        t.DOKill();
        if (token.IsCancellationRequested) {
            return;
        }

        Vector3 start = t.position;

        var dx = Mathf.Abs(target.x - start.x);
        var dy = Mathf.Abs(target.y - start.y);
        var total = dx + dy;

        var tx = total > 0f ? duration * (dx / total) : 0f;
        var ty = total > 0f ? duration * (dy / total) : 0f;

        if (xFirst) {
            if (dx > 0f) {
                await t.DOMoveX(target.x, tx).SetEase(Ease.Linear).AsyncWaitForCompletion();
            }

            if (token.IsCancellationRequested) {
                return;
            }

            if (dy > 0f) {
                await t.DOMoveY(target.y, ty).SetEase(Ease.Linear).AsyncWaitForCompletion();
            }
        }
        else {
            if (dy > 0f) {
                await t.DOMoveY(target.y, ty).SetEase(Ease.Linear).AsyncWaitForCompletion();
            }

            if (token.IsCancellationRequested) {
                return;
            }

            if (dx > 0f) {
                await t.DOMoveX(target.x, tx).SetEase(Ease.Linear).AsyncWaitForCompletion();
            }
        }
    }

    private List<Vector3> BuildClearPath(ArrowCell start)
    {
        var result = new List<Vector3>();

        var current = start;

        while (current is { Cleared: false }) {
            if (!IsClearable(current)) {
                break;
            }

            result.Add(CoordToWorld(current.Coord));

            var nextCoord = current.Coord + DirToOffset[current.Direction];

            if (!arrowBlocks.TryGetValue(nextCoord, out var next) || next.Cleared || next.Color != current.Color) {
                break;
            }

            current = next;
        }

        return result;
    }

    private async UniTask AnimateAndClearFromEntry(Transform block, ArrowCell entry, CancellationToken token)
    {
        var startPos = CoordToWorld(entry.Coord);
        await MoveAxisOnly(block, startPos, conveyorDuration, token);

        var path = BuildClearPath(entry);

        for (int i = 1; i < path.Count; i++) {
            await MoveTo(block, path[i], clearingDuration, token);
            animationView.PlayClearAnimation(block);
        }

        ClearAlongPath(entry);
    }

    private static Color ToUnityColor(BlockColor c) =>
        c switch {
            BlockColor.Red => Color.red,
            BlockColor.Blue => Color.blue,
            BlockColor.Green => Color.green,
            _ => Color.white
        };

    private void OnDrawGizmos()
    {
        foreach (var point in conveyorPathPoints) {
            Gizmos.DrawSphere(point.position, 0.1f);
            Gizmos.color = Color.yellow;
        }
    }
}