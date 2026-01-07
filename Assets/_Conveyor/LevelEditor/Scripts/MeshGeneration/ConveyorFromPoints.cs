using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ConveyorFromPoints : MonoBehaviour
    {
        [Header("JSON Input")]
        [SerializeField]
        private LevelPointsFromJson levelPointsFromJson;
        
        [Header("Shape")]
        [SerializeField]
        private float halfExtentX = 5f;
        [SerializeField]
        private float halfExtentZ = 5f;

        [SerializeField]
        private float yOffset;

        [Header("Belt")]
        [SerializeField]
        private float width = 1f;

        private Mesh mesh;
        private readonly List<Vector3> verts = new();
        private readonly List<int> tris = new();
        
        [Header("Rounded Corners")]
        [SerializeField]
        private bool roundCorners = true;
        [SerializeField]
        private float cornerRadius = 0.5f; 
        [SerializeField] 
        private int cornerSegments = 6;
        
        private void Awake()
        {
            if (!mesh) {
                mesh = new Mesh { name = "ConveyorBelt" };
                mesh.MarkDynamic();
            }

            GetComponent<MeshFilter>().sharedMesh = mesh;
        }

        private void OnValidate()
        {
            if (mesh) {
                return;
            }
            mesh = new Mesh { name = "ConveyorBelt" };
            mesh.MarkDynamic();
            var mf = GetComponent<MeshFilter>();
            mf.sharedMesh = mesh;
        }
        
        public void ReadConveyorPointsFromJson()
        {
            levelPointsFromJson.LoadLevelPointsFromJson();
        }
        
        [ContextMenu("Build Conveyor From Points")]
        public void BuildConveyorFromPoints()
        {
            if (!mesh) {
                return;
            }
            
            var pts = BuildFromNormalizedPoints(levelPointsFromJson.ConveyorPoints, halfExtentX, halfExtentZ, yOffset);

            pts = RoundRightAngleCornersXZ(pts, cornerRadius, cornerSegments, closed: true);
            
            BuildConveyor(pts, width);
        }

        private static List<Vector3> RoundRightAngleCornersXZ(
            IReadOnlyList<Vector3> pts,
            float radius,
            int segments,
            bool closed)
        {
            var n = pts.Count;
            if (n < (closed ? 3 : 2)) {
                return new List<Vector3>(pts);
            }

            radius = Mathf.Max(0f, radius);
            segments = Mathf.Max(1, segments);

            var output = new List<Vector3>(n * (segments + 2));

            var start = closed ? 0 : 1;
            var end = closed ? n : n - 1;

            if (!closed) {
                output.Add(pts[0]);
            }

            for (int i = start; i < end; i++) {
                var A = pts[(i - 1 + n) % n];
                Vector3 B = pts[i % n];
                Vector3 C = pts[(i + 1) % n];

                Vector2 inVec2 = new Vector2(B.x - A.x, B.z - A.z);
                Vector2 outVec2 = new Vector2(C.x - B.x, C.z - B.z);

                float inLen = inVec2.magnitude;
                float outLen = outVec2.magnitude;

                if (inLen < 1e-6f || outLen < 1e-6f || radius < 1e-6f) {
                    output.Add(B);
                    continue;
                }

                Vector2 dirIn = inVec2 / inLen; // A->B in XZ
                Vector2 dirOut = outVec2 / outLen; // B->C in XZ

                // If essentially straight, keep the vertex
                if (Vector2.Dot(dirIn, dirOut) > 0.999f) {
                    output.Add(B);
                    continue;
                }

                // We only expect right-angle turns for a grid conveyor.
                // Clamp radius so we don't eat past half of either segment.
                float r = Mathf.Min(radius, inLen * 0.49f, outLen * 0.49f);

                // Cut points
                Vector3 P = new Vector3(B.x - dirIn.x * r, B.y, B.z - dirIn.y * r);
                Vector3 Q = new Vector3(B.x + dirOut.x * r, B.y, B.z + dirOut.y * r);

                // Center for an orthogonal fillet: move from B by -dirIn*r then +dirOut*r
                Vector3 center = new Vector3(B.x - dirIn.x * r + dirOut.x * r, B.y, B.z - dirIn.y * r + dirOut.y * r);

                float a0 = Mathf.Atan2(P.z - center.z, P.x - center.x);
                float a1 = Mathf.Atan2(Q.z - center.z, Q.x - center.x);

                // Determine turn direction (ccw in XZ)
                float cross = dirIn.x * dirOut.y - dirIn.y * dirOut.x; // z-component in 2D

                // Make angle travel the correct way around
                if (cross > 0f) // ccw
                {
                    while (a1 < a0) {
                        a1 += Mathf.PI * 2f;
                    }
                }
                else // cw
                {
                    while (a1 > a0) {
                        a1 -= Mathf.PI * 2f;
                    }
                }

                // Emit P -> arc -> Q (don’t emit B)
                output.Add(P);
                for (int s = 1; s < segments; s++) {
                    float t = s / (float)segments;
                    float a = Mathf.Lerp(a0, a1, t);
                    float x = center.x + Mathf.Cos(a) * r;
                    float z = center.z + Mathf.Sin(a) * r;
                    output.Add(new Vector3(x, B.y, z));
                }
                output.Add(Q);
            }

            if (!closed) {
                output.Add(pts[n - 1]);
            }

            if (closed && output.Count > 2 && (output[0] - output[^1]).sqrMagnitude < 1e-10f) {
                output.RemoveAt(output.Count - 1);
            }

            return output;
        }

        private List<Vector3> BuildFromNormalizedPoints(List<Vector2> pts01, float hx, float hz, float y)
        {
            var outPts = new List<Vector3>(pts01.Count);

            foreach (var t in pts01) {
                var p = t;

                var x = Mathf.Lerp(-hx, hx, p.x);
                var z = Mathf.Lerp(-hz, hz, p.y);
                outPts.Add(new Vector3(x, y, z));
            }

            return outPts;
        }

        private void BuildConveyor(List<Vector3> pts, float width)
        {
            ClearMesh(mesh);

            var count = pts.Count;
            if (count < 3) {
                return;
            }

            var halfW = width * 0.5f;
            var miterLimit = 4f;
            var maxMiter = halfW * miterLimit;

            for (int i = 0; i < count; i++) {
                var p = pts[i];
                var pPrev = pts[(i - 1 + count) % count];
                var pNext = pts[(i + 1) % count];

                var d0 = p - pPrev;
                var d1 = pNext - p;

                var d0m = d0.magnitude;
                var d1m = d1.magnitude;

                if (!(d0m < 1e-6f) && !(d1m < 1e-6f)) {
                    d0 /= d0m;
                    d1 /= d1m;

                    var n0 = LeftNormalXZ(d0).normalized;
                    var n1 = LeftNormalXZ(d1).normalized;

                    var m = n0 + n1;

                    if (m.sqrMagnitude < 1e-8f) {
                        verts.Add(p + n1 * halfW);
                        verts.Add(p - n1 * halfW);
                        continue;
                    }

                    m.Normalize();

                    if (Vector3.Dot(m, n1) < 0f) {
                        m = -m;
                    }

                    var denom = Vector3.Dot(m, n1);
                    if (Mathf.Abs(denom) < 1e-5f) {
                        verts.Add(p + n1 * halfW);
                        verts.Add(p - n1 * halfW);
                        continue;
                    }

                    var miterLen = halfW / denom;

                    if (Mathf.Abs(miterLen) > maxMiter) {
                        verts.Add(p + n1 * halfW);
                        verts.Add(p - n1 * halfW);
                    }
                    else {
                        verts.Add(p + m * miterLen);
                        verts.Add(p - m * miterLen);
                    }
                    static Vector3 LeftNormalXZ(Vector3 d) => new Vector3(-d.z, 0f, d.x);
                }
            }

            var pairs = verts.Count / 2;
            if (pairs < 3) {
                SetMesh(mesh);
                return;
            }

            for (var i = 0; i < pairs; i++) {
                var i0 = i * 2;
                var i1 = i0 + 1;
                var j0 = ((i + 1) % pairs) * 2;
                var j1 = j0 + 1;

                tris.Add(i0);
                tris.Add(j0);
                tris.Add(i1);
                tris.Add(i1);
                tris.Add(j0);
                tris.Add(j1);
            }

            SetMesh(mesh);
        }

        private void ClearMesh(Mesh mesh)
        {
            mesh.Clear();
            verts.Clear();
            tris.Clear();
        }

        private void SetMesh(Mesh mesh)
        {
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateBounds();
        }
    }
}