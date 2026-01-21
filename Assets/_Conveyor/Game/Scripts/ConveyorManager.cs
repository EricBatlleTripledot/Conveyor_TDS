using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace Game
{
    [ExecuteInEditMode]
    public class ConveyorManager : MonoBehaviour
    {
        [Header("Spline Container")]
        [SerializeField]
        private SplineContainer splineContainer;

        [Header("Belt Prefabs")]
        [SerializeField]
        private GameObject beltSocketPrefab;
        [SerializeField]
        private GameObject beltArrowPrefab;

        [Header("Animation Settings")]
        [SerializeField]
        private float speed = 1;
        
        [SerializeField]
        private bool alignRotationToSpline = true;
        [SerializeField]
        private Transform visualsParentTransform;
        [SerializeField]
        private Vector3 arrowForwardLocal = Vector3.forward;

        [SerializeField]
        private List<SplineAnimate> spawnedSplineAnimatedObjects = new();
        [SerializeField]
        private BlockViewStackView stackView;

        public int socketsCount = 6;

        public event Action<SplineAnimate> SocketInsideLaunchWindow;
        
        [Space]
        public float jumpDurationInSeconds = 2;
        public float triggerMarginMeters = 0.15f;
        public float steps = 64;
        public int attempts = 30;
        // private void Update()
        // {
        //     foreach (var splineAnimate in spawnedSplineAnimatedObjects) {
        //         var (_, tLanding) = splineContainer.GetNearestPointTo(stackView.StackPoint, attempts);
        //         bool shouldTriggerSocket = ShouldTriggerSocket(splineContainer, splineAnimate.NormalizedTime, tLanding, ,jumpDurationInSeconds, triggerMarginMeters, steps);
        //         if (shouldTriggerSocket) {
        //             SocketInsideLaunchWindow?.Invoke(splineAnimate);
        //         }
        //     }
        // }
        
        [SerializeField] private Transform stackLandingWorldPoint; // o el punto del stack
        [SerializeField] private float jumpDurationSec = 3f;        // ideal: asignar desde tween/clip, no hardcode
        // [SerializeField] private float triggerMarginMeters = 0.1f;
        [SerializeField] private int nearestAttempts = 64;
        [SerializeField] private int lengthSteps = 64;

        private float _tLanding;
        private bool _landingCached;
        private readonly Dictionary<SplineAnimate, float> _prevTBySocket = new();
        private readonly Dictionary<SplineAnimate, float> _measuredSpeedBySocket = new();
        
        private readonly Dictionary<SplineAnimate, float> _lastDToLanding = new();
        private readonly Dictionary<SplineAnimate, float> _lastWindowMeters = new();
        private readonly Dictionary<SplineAnimate, bool> _lastInside = new();

        private void Update()
        {
            if (splineContainer == null || spawnedSplineAnimatedObjects.Count == 0) return;

            CacheLandingTIfNeeded();

            // 1) medir t y velocidad real (m/s)
            MeasureSocketsSpeed();

            // 2) decidir triggers
            foreach (var socket in spawnedSplineAnimatedObjects)
            {
                if (socket == null) continue;

                // float tSocketNow = GetSocketTByPosition(socket);
                float tSocketNow = socket.NormalizedTime;
                float speedMps = _measuredSpeedBySocket.TryGetValue(socket, out var v) ? v : 0f;

                float dToLanding = splineContainer.ApproxLengthForward(tSocketNow, _tLanding, lengthSteps);
                float dNeeded = speedMps * jumpDurationSec;
                float window = dNeeded + triggerMarginMeters;

                bool inside = dToLanding <= window;

                _lastDToLanding[socket] = dToLanding;
                _lastWindowMeters[socket] = window;
                _lastInside[socket] = inside;

                if (inside)
                {
                    Debug.LogWarning($"INSIDE? {socket.name} d={dToLanding:F2}m window={window:F2}m speed={speedMps:F2}m/s t={tSocketNow:F3}");
                    SocketInsideLaunchWindow?.Invoke(socket);
                }

                if (ShouldTriggerSocket(
                        splineContainer,
                        tSocketNow,
                        _tLanding,
                        speedMps,
                        jumpDurationSec,
                        triggerMarginMeters,
                        lengthSteps))
                {
                    Debug.LogWarning("EBC SOCKET INSIDE LAUNCH WINDOW");
                    SocketInsideLaunchWindow?.Invoke(socket);
                }
            }
            CacheBeltSpeedFromFirstSocket();
        }
        
        private Vector3 _prevPos;
        private float _prevTime;
        private bool _hasPrev;
        private void CacheBeltSpeedFromFirstSocket()
        {
            if (!Application.isPlaying) return;
            if (spawnedSplineAnimatedObjects == null || spawnedSplineAnimatedObjects.Count == 0) return;
            var s = spawnedSplineAnimatedObjects[0];
            if (s == null) return;

            float now = Time.time;
            Vector3 pos = s.transform.position;

            if (!_hasPrev)
            {
                _hasPrev = true;
                _prevPos = pos;
                _prevTime = now;
                return;
            }

            float dt = now - _prevTime;
            if (dt <= 0f) return;

            cachedBeltSpeedMps = Vector3.Distance(_prevPos, pos) / dt; // aproximación (euclídea)
            _prevPos = pos;
            _prevTime = now;
        }

        private void CacheLandingTIfNeeded()
        {
            if (_landingCached) return;
            // if (stackLandingWorldPoint == null) return;

            var (_, t) = splineContainer.GetNearestPointTo(stackView.StackPoint, nearestAttempts);
            // var (_, t) = splineContainer.GetNearestPointTo(stackLandingWorldPoint.position, nearestAttempts);
            _tLanding = t;
            _landingCached = true;
        }

        private float GetSocketTByPosition(SplineAnimate socket)
        {
            var (_, t) = splineContainer.GetNearestPointTo(socket.transform.position, nearestAttempts);
            return t;
        }

        private void MeasureSocketsSpeed()
        {
            float dt = Time.deltaTime;
            if (dt <= 0f) return;

            foreach (var socket in spawnedSplineAnimatedObjects)
            {
                if (socket == null) continue;

                // float tNow = GetSocketTByPosition(socket);
                float tNow = socket.NormalizedTime;

                if (_prevTBySocket.TryGetValue(socket, out float tPrev))
                {
                    // distancia real recorrida sobre spline en este frame:
                    float d = splineContainer.ApproxLengthForward(tPrev, tNow, lengthSteps);
                    float v = d / dt;
                    _measuredSpeedBySocket[socket] = v;
                }

                _prevTBySocket[socket] = tNow;
            }
        }
        
        public bool ShouldTriggerSocket(
            SplineContainer spline,
            float tSocketNow,
            float tLanding,
            float beltSpeedMetersPerSec,
            float jumpDurationSec,
            float triggerMarginMeters,
            int steps)
        {
            float dToLanding = spline.ApproxLengthForward(tSocketNow, tLanding, steps);

            float dNeeded = beltSpeedMetersPerSec * jumpDurationSec;

            return dToLanding <= (dNeeded + triggerMarginMeters);
        }

        public void InstantiateBeltPrefabs(int socketsCount)
        {
            if (socketsCount <= 0) return;
            if (splineContainer == null) return;
            if (beltSocketPrefab == null || beltArrowPrefab == null) return;
            
            ClearSpawnedGameObjects();
            
            for (var i = 0; i < socketsCount; i++) {
                var tSocket = i / (float)socketsCount;
                SpawnAtT(beltSocketPrefab, tSocket, $"Socket_{i}");

                var tArrow = (i + 0.5f) / socketsCount; // between socket i & i+1
                // safety net
                if (tArrow >= 1f) {
                    tArrow -= 1f;
                }

                SpawnAtT(beltArrowPrefab, tArrow, $"Arrow_{i}");
            }
        }
        
        private void SpawnAtT(GameObject prefab, float t, string name)
        {
            // Don't use spline.Evaluate, use splineContainer.Evaluate.
            // First one operates in local space, second one in world space!
            const int splineIndex = 0;
            var position = (Vector3)splineContainer.EvaluatePosition(splineIndex, t);

            var quaternion = Quaternion.identity;
            if (alignRotationToSpline) {
                var tangent = splineContainer.EvaluateTangent(splineIndex, t);
                var up = splineContainer.EvaluateUpVector(splineIndex, t);
                quaternion = Quaternion.LookRotation(tangent, up);
                quaternion *= Quaternion.FromToRotation(Vector3.forward, arrowForwardLocal);
            }

            var parent = visualsParentTransform != null ? visualsParentTransform : transform;
            var go = Instantiate(prefab, position, quaternion, parent);
            var splineAnimate = go.GetComponent<SplineAnimate>();
            ConfigureSplineAnimate(splineAnimate, splineContainer, speed, t);
            go.name = name;
            spawnedSplineAnimatedObjects.Add(splineAnimate);
        }

        private void ConfigureSplineAnimate(SplineAnimate splineAnimate, SplineContainer splineContainer, float speed, float startOffset)
        {
            splineAnimate.Container = splineContainer;
            splineAnimate.AnimationMethod = SplineAnimate.Method.Speed;
            splineAnimate.MaxSpeed = speed;
            splineAnimate.StartOffset = startOffset;
        }

        [ContextMenu("Play")]
        public void Play()
        {
            foreach (var splineAnimate in spawnedSplineAnimatedObjects) {
                splineAnimate.Play();
            }
        }
        
        [ContextMenu("Pause")]
        public void Pause()
        {
            foreach (var splineAnimate in spawnedSplineAnimatedObjects) {
                splineAnimate.Pause();
            }
        }

        [ContextMenu("ClearSpawnedGameObjects")]
        private void ClearSpawnedGameObjects()
        {
            for (var i = spawnedSplineAnimatedObjects.Count - 1; i >= 0; i--) {
                if (spawnedSplineAnimatedObjects[i] != null) {
                    DestroyImmediate(spawnedSplineAnimatedObjects[i].gameObject);
                }
            }
            spawnedSplineAnimatedObjects.Clear();
        }
        
        [ContextMenu("InstantiateBeltPrefabs")]
        private void DEBUG_InstantiateBeltPrefabs()
        {
            InstantiateBeltPrefabs(socketsCount);
        }

#region DEBUG
        private float cachedBeltSpeedMps;

        [Header("Launch Debug")]
        [SerializeField] private bool drawLaunchGizmos = true;
        // [SerializeField] private Transform stackLandingWorldPoint;
        [SerializeField] private float debugJumpDurationSec = 3f;        // ideal: leer del tween/clip
        [SerializeField] private float debugTriggerMarginMeters = 0.1f;
        [SerializeField] private int debugSamples = 128;
        [SerializeField] private float gizmoCubeSize = 0.18f;

        [SerializeField] private bool debugOverrideSpeed = true;
        [SerializeField] private float debugSpeedMps = 1.5f; // prueba con 1..3 m/s

        private void OnDrawGizmos()
        {
            if (!drawLaunchGizmos) return;
            if (splineContainer == null) return;
            // if (stackLandingWorldPoint == null) return;

            // 1) Landing t
            var (landingPos, tLanding) = splineContainer.GetNearestPointTo(stackView.StackPoint, debugSamples);

            // Landing marker
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(landingPos, Vector3.one * gizmoCubeSize * 2);

            // Si no hay sockets, al menos dibujamos landing
            if (spawnedSplineAnimatedObjects == null || spawnedSplineAnimatedObjects.Count == 0) return;

            // 2) Usamos el primer socket como referencia para estimar velocidad real
            // (o podrías calcular un promedio).
            var firstSocket = spawnedSplineAnimatedObjects[0];
            if (firstSocket == null) return;

            // float beltSpeedMps = EstimateSpeedMpsFromSocket(firstSocket, tLanding);
            // float dNeeded = beltSpeedMps * debugJumpDurationSec;
            float beltSpeedMps = debugOverrideSpeed ? debugSpeedMps : cachedBeltSpeedMps;
            float dNeeded = beltSpeedMps * debugJumpDurationSec;


            // 3) Punto trigger (dNeeded metros antes de landing, siguiendo el sentido inverso)
            float tTrigger = FindTBackwardFrom(splineContainer, tLanding, dNeeded, debugSamples);
            Vector3 triggerPos = (Vector3)splineContainer.EvaluatePosition(0, tTrigger);

            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(triggerPos, Vector3.one * gizmoCubeSize);

            // 4) Ventana por margen (opcional)
            // Ventana completa
            DrawSplineWindow(
                splineContainer,
                tTrigger,
                tLanding,
                points: 30,
                size: gizmoCubeSize * 0.35f
            );
            float tTriggerOuter = FindTBackwardFrom(splineContainer, tLanding, dNeeded + debugTriggerMarginMeters, debugSamples);
            Vector3 triggerOuterPos = (Vector3)splineContainer.EvaluatePosition(0, tTriggerOuter);

            Gizmos.color = new Color(1f, 0.5f, 0f, 1f); // naranja
            Gizmos.DrawWireCube(triggerOuterPos, Vector3.one * gizmoCubeSize * 1.4f);

            // Etiqueta visual extra: línea entre trigger y landing
            Gizmos.color = Color.white;
            Gizmos.DrawLine(triggerPos, landingPos);
        }
        
        private float _prevTForDebug;
        private float _prevTimeForDebug;
        private bool _debugHasPrev;

        private float EstimateSpeedMpsFromSocket(SplineAnimate socket, float tFallback)
        {
            // Si no estamos en play mode, no podemos medir velocidad real con dt fácilmente.
            // Devolvemos una aproximación: 0 => te dibuja trigger casi encima del landing.
            if (!Application.isPlaying)
                return 0f;

            float tNow = GetSocketTByPosition(socket);
            float now = Time.time;

            if (!_debugHasPrev)
            {
                _debugHasPrev = true;
                _prevTForDebug = tNow;
                _prevTimeForDebug = now;
                return 0f;
            }

            float dt = now - _prevTimeForDebug;
            if (dt <= 0f) return 0f;

            float d = splineContainer.ApproxLengthForward(_prevTForDebug, tNow, debugSamples);
            float v = d / dt;

            _prevTForDebug = tNow;
            _prevTimeForDebug = now;

            return v;
        }

        private float FindTBackwardFrom(SplineContainer spline, float tStart, float metersBack, int steps)
        {
            if (metersBack <= 0f) return tStart;

            // Recorremos hacia atrás en t en pequeños pasos, acumulando distancia hasta llegar a metersBack.
            float t = tStart;
            Vector3 prev = (Vector3)spline.EvaluatePosition(0, t);
            float accumulated = 0f;

            for (int i = 0; i < steps * 4; i++) // *4 por seguridad en wraps
            {
                float tNext = t - (1f / steps);
                if (tNext < 0f) tNext += 1f;

                Vector3 p = (Vector3)spline.EvaluatePosition(0, tNext);
                accumulated += Vector3.Distance(prev, p);

                if (accumulated >= metersBack)
                    return tNext;

                prev = p;
                t = tNext;
            }

            return t; // fallback
        }

        private void DrawSplineWindow(
            SplineContainer spline,
            float tStart,
            float tEnd,
            int points,
            float size)
        {
            Gizmos.color = new Color(1f, 0f, 1f, 0.9f); // magenta

            float delta = tEnd - tStart;
            if (delta < 0f)
                delta += 1f; // wrap forward

            for (int i = 0; i <= points; i++)
            {
                float a = i / (float)points;
                float t = (tStart + delta * a) % 1f;

                Vector3 p = (Vector3)spline.EvaluatePosition(0, t);
                Gizmos.DrawSphere(p, size);
            }
        }


#endregion
    }
}