using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace Game
{
    [ExecuteInEditMode]
    public class ConveyorManager : MonoBehaviour
    {
        public event Action<SplineAnimate> SocketEnteredLaunchWindow;

        [Header("Spline Container")]
        [SerializeField] private SplineContainer splineContainer;

        [Header("Belt Prefabs")]
        [SerializeField] private GameObject beltSocketPrefab;
        [SerializeField] private GameObject beltArrowPrefab;

        [Header("Runtime Animation Settings")]
        [SerializeField] private float speed = 1f;
        [SerializeField] private bool alignRotationToSpline = true;
        [SerializeField] private Transform visualsParentTransform;
        [SerializeField] private Vector3 arrowForwardLocal = Vector3.forward;

        [Header("Sockets")]
        [SerializeField] private int socketsCount = 6;
        [SerializeField] private List<SplineAnimate> spawnedSplineAnimatedObjects = new();
        [SerializeField] private List<SplineAnimate> spawnedSplineAnimatedSockets = new();

        [Header("Landing")]
        [SerializeField] private BlockViewStackView stackView;

        [Header("Launch Window")]
        [SerializeField]
        private AnimationClip jumpAnimationClip;
        [SerializeField] private float launchWindowToleranceMeters = 0.15f;

        [Header("Sampling Quality")]
        [SerializeField] private int nearestPointAttempts = 64;
        [SerializeField] private int lengthSteps = 64;

        [Header("Event Behaviour")]
        [SerializeField] private bool triggerOnlyOncePerLap = true;
        [SerializeField] private float eligibilityReentryMarginMeters = 0.30f;

        [Header("Debug Draw")]
        [SerializeField] private bool drawLaunchGizmos = true;
        [SerializeField] private float gizmoSize = 0.18f;
        [SerializeField] private int gizmoWindowPoints = 30;
        
        private float splineLengthMeters;
        private float landingTimeOnSpline;
        private bool landingTimeIsValid;

        private readonly Dictionary<SplineAnimate, SocketLaunchState> socketLaunchStates = new();
        private readonly HashSet<SplineAnimate> socketsEligibleForLaunch = new();

        [ContextMenu("InstantiateBeltPrefabs")]
        public void foo()
        {
            InstantiateBeltPrefabs(socketsCount);
        }
        
        private void Awake()
        {
            RecalculateSplineLength();
        }

        private void OnValidate()
        {
            if (splineContainer != null)
            {
                RecalculateSplineLength();
            }
        }

        private void Update()
        {
            if (splineContainer == null) return;
            if (stackView == null) return;
            if (spawnedSplineAnimatedSockets == null || spawnedSplineAnimatedSockets.Count == 0) return;

            UpdateLandingTimeOnSpline();
            if (!landingTimeIsValid) return;

            UpdateSocketStatesAndTriggerEvents();
        }

        private void UpdateLandingTimeOnSpline()
        {
            var (landingPosition, landingTime) = splineContainer.GetNearestPointTo(stackView.StackPoint, nearestPointAttempts);
            landingTimeOnSpline = landingTime;
            landingTimeIsValid = true;
        }

        private void UpdateSocketStatesAndTriggerEvents()
        {
            foreach (var socket in spawnedSplineAnimatedSockets)
            {
                if (socket == null) continue;

                float socketTimeOnSpline = GetSocketTimeOnSpline(socket);
                SocketLaunchState state = GetOrCreateSocketLaunchState(socket);

                state.PreviousTimeOnSpline = state.CurrentTimeOnSpline;
                state.CurrentTimeOnSpline = socketTimeOnSpline;

                float deltaTimeNormalized = GetForwardDeltaNormalized(state.PreviousTimeOnSpline, state.CurrentTimeOnSpline);
                float socketSpeedMetersPerSecond = EstimateSpeedMetersPerSecond(deltaTimeNormalized);

                float distanceToLandingMeters = splineContainer.ApproxLengthForward(state.CurrentTimeOnSpline, landingTimeOnSpline, lengthSteps);
                var jumpDurationInSeconds = jumpAnimationClip.length;
                float neededDistanceMeters = socketSpeedMetersPerSecond * jumpDurationInSeconds;
                float windowMeters = neededDistanceMeters + launchWindowToleranceMeters;

                state.DistanceFromSocketToLandingMeters = distanceToLandingMeters;
                state.SpeedMetersPerSecond = socketSpeedMetersPerSecond;
                state.LaunchWindowSizeMeters = windowMeters;
                state.IsInsideLaunchWindow = distanceToLandingMeters <= windowMeters;

                bool shouldFire = state.IsInsideLaunchWindow;

                if (triggerOnlyOncePerLap) {
                    if (socketsEligibleForLaunch.Contains(socket)) {
                        if (state.IsInsideLaunchWindow) {
                            socketsEligibleForLaunch.Remove(socket);
                            shouldFire = true;
                        }
                    }
                    else {
                        float distanceToBecomeEligibleAgainMeters = windowMeters + eligibilityReentryMarginMeters;
                        if (distanceToLandingMeters > distanceToBecomeEligibleAgainMeters) {
                            socketsEligibleForLaunch.Add(socket);
                        }

                        shouldFire = false;
                    }
                }

                socketLaunchStates[socket] = state;

                if (shouldFire)
                {
                    Debug.LogWarning("SOCKET INSIDE LAUNCH WINDOW");
                    SocketEnteredLaunchWindow?.Invoke(socket);
                }
            }
        }

        private SocketLaunchState GetOrCreateSocketLaunchState(SplineAnimate socket)
        {
            if (socketLaunchStates.TryGetValue(socket, out var existing))
            {
                return existing;
            }

            var created = new SocketLaunchState
            {
                PreviousTimeOnSpline = GetSocketTimeOnSpline(socket),
                CurrentTimeOnSpline = GetSocketTimeOnSpline(socket)
            };

            socketLaunchStates[socket] = created;

            if (triggerOnlyOncePerLap)
            {
                socketsEligibleForLaunch.Add(socket);
            }

            return created;
        }

        private float GetSocketTimeOnSpline(SplineAnimate socket)
        {
            // avoids noise from nearest-point by position
            // + offset is necessary to make the system works for all sockets, not only the first one
            return Mathf.Repeat(socket.NormalizedTime + socket.StartOffset, 1f);
        }

        private float GetForwardDeltaNormalized(float previousTime, float currentTime)
        {
            float delta = currentTime - previousTime;
            if (delta < 0f) delta += 1f;
            return delta;
        }

        private float EstimateSpeedMetersPerSecond(float deltaTimeNormalized)
        {
            float deltaMeters = deltaTimeNormalized * splineLengthMeters;
            float deltaSeconds = Time.deltaTime;

            if (deltaSeconds <= 0f) return 0f;
            return deltaMeters / deltaSeconds;
        }

        private void RecalculateSplineLength()
        {
            if (splineContainer == null)
            {
                splineLengthMeters = 0f;
                return;
            }

            splineLengthMeters = ApproxSplineLength(splineContainer, 512);
        }

        private float ApproxSplineLength(SplineContainer spline, int steps)
        {
            float length = 0f;

            Vector3 previousPosition = (Vector3)spline.EvaluatePosition(0, 0f);
            for (int i = 1; i <= steps; i++)
            {
                float time = i / (float)steps;
                Vector3 position = (Vector3)spline.EvaluatePosition(0, time);
                length += Vector3.Distance(previousPosition, position);
                previousPosition = position;
            }

            return length;
        }

        public void InstantiateBeltPrefabs(int socketsCountToSpawn)
        {
            if (socketsCountToSpawn <= 0) return;
            if (splineContainer == null) return;
            if (beltSocketPrefab == null || beltArrowPrefab == null) return;

            ClearSpawnedGameObjects();

            for (int i = 0; i < socketsCountToSpawn; i++)
            {
                float socketTime = i / (float)socketsCountToSpawn;
                SpawnSocketAtTime(socketTime, $"Socket_{i}");

                float arrowTime = (i + 0.5f) / socketsCountToSpawn;
                if (arrowTime >= 1f) arrowTime -= 1f;

                SpawnAtTime(beltArrowPrefab, arrowTime, $"Arrow_{i}");
            }
        }

        private SplineAnimate SpawnAtTime(GameObject prefab, float timeOnSpline, string objectName)
        {
            const int splineIndex = 0;

            Vector3 position = (Vector3)splineContainer.EvaluatePosition(splineIndex, timeOnSpline);

            Quaternion rotation = Quaternion.identity;
            if (alignRotationToSpline)
            {
                Vector3 tangent = splineContainer.EvaluateTangent(splineIndex, timeOnSpline);
                Vector3 up = splineContainer.EvaluateUpVector(splineIndex, timeOnSpline);
                rotation = Quaternion.LookRotation(tangent, up);
                rotation *= Quaternion.FromToRotation(Vector3.forward, arrowForwardLocal);
            }

            Transform parent = visualsParentTransform != null ? visualsParentTransform : transform;
            GameObject spawned = Instantiate(prefab, position, rotation, parent);
            spawned.name = objectName;

            var splineAnimate = spawned.GetComponent<SplineAnimate>();
            if (splineAnimate != null) {
                ConfigureSplineAnimate(splineAnimate, splineContainer, speed, timeOnSpline);
                spawnedSplineAnimatedObjects.Add(splineAnimate);
            }
            return splineAnimate;
        }

        private void SpawnSocketAtTime(float timeOnSpline, string objectName)
        {
            var splineAnimate = SpawnAtTime(beltSocketPrefab, timeOnSpline, objectName);
            if (splineAnimate == null) {
                return;
            }

            spawnedSplineAnimatedSockets.Add(splineAnimate);
            if (triggerOnlyOncePerLap) {
                socketsEligibleForLaunch.Add(splineAnimate);
            }
        }

        private void ConfigureSplineAnimate(SplineAnimate splineAnimate, SplineContainer container, float maxSpeed, float startOffset)
        {
            if (splineAnimate == null) return;

            splineAnimate.Container = container;
            splineAnimate.AnimationMethod = SplineAnimate.Method.Speed;
            splineAnimate.MaxSpeed = maxSpeed;
            splineAnimate.StartOffset = startOffset;
        }

        [ContextMenu("Play")]
        public void Play()
        {
            foreach (var splineAnimate in spawnedSplineAnimatedObjects)
            {
                if (splineAnimate != null) splineAnimate.Play();
            }
        }

        [ContextMenu("Pause")]
        public void Pause()
        {
            foreach (var splineAnimate in spawnedSplineAnimatedObjects)
            {
                if (splineAnimate != null) splineAnimate.Pause();
            }
        }

        [ContextMenu("ClearSpawnedGameObjects")]
        private void ClearSpawnedGameObjects()
        {
            for (var i = spawnedSplineAnimatedObjects.Count - 1; i >= 0; i--) {
                var splineAnimate = spawnedSplineAnimatedObjects[i];
                if (splineAnimate != null) {
                    DestroyImmediate(splineAnimate.gameObject);
                }
            }

            spawnedSplineAnimatedSockets.Clear();
            spawnedSplineAnimatedObjects.Clear();
            socketLaunchStates.Clear();
            socketsEligibleForLaunch.Clear();
        }

        private void OnDrawGizmos()
        {
            if (!drawLaunchGizmos) return;
            if (splineContainer == null) return;
            if (stackView == null) return;

            var (landingPosition, landingTime) = splineContainer.GetNearestPointTo(stackView.StackPoint, nearestPointAttempts);

            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(landingPosition, Vector3.one * gizmoSize * 2f);

            if (!Application.isPlaying) return;
            if (spawnedSplineAnimatedSockets == null || spawnedSplineAnimatedSockets.Count == 0) return;

            foreach (var socket in spawnedSplineAnimatedSockets)
            {
                if (socket == null) continue;

                if (!socketLaunchStates.TryGetValue(socket, out var state))
                {
                    Gizmos.color = Color.gray;
                    Gizmos.DrawSphere(socket.transform.position, gizmoSize * 0.35f);
                    continue;
                }

                Gizmos.color = state.IsInsideLaunchWindow ? Color.green : Color.red;
                Gizmos.DrawSphere(socket.transform.position, gizmoSize * 0.45f);

                // Draw start matching window
                float triggerDistanceMeters = Mathf.Max(state.LaunchWindowSizeMeters - launchWindowToleranceMeters, 0f);
                float triggerTime = FindTimeBackwardFrom(splineContainer, landingTime, triggerDistanceMeters, 256);
                Vector3 triggerPosition = splineContainer.EvaluatePosition(0, triggerTime);

                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(triggerPosition, Vector3.one * gizmoSize);

                // Draw matching window
                DrawSplineWindow(splineContainer, triggerTime, landingTime, gizmoWindowPoints, gizmoSize * 0.25f);
            }
        }

        private float FindTimeBackwardFrom(SplineContainer spline, float startTime, float metersBack, int steps)
        {
            if (metersBack <= 0f) return startTime;

            float time = startTime;
            Vector3 previous = spline.EvaluatePosition(0, time);
            float accumulated = 0f;

            for (int i = 0; i < steps * 4; i++)
            {
                float nextTime = time - (1f / steps);
                if (nextTime < 0f) nextTime += 1f;

                Vector3 position = spline.EvaluatePosition(0, nextTime);
                accumulated += Vector3.Distance(previous, position);

                if (accumulated >= metersBack)
                {
                    return nextTime;
                }

                previous = position;
                time = nextTime;
            }

            return time;
        }

        private void DrawSplineWindow(SplineContainer spline, float startTime, float endTime, int points, float size)
        {
            Gizmos.color = new Color(1f, 0f, 1f, 0.9f);

            float delta = endTime - startTime;
            if (delta < 0f) delta += 1f;

            for (int i = 0; i <= points; i++)
            {
                float alpha = i / (float)points;
                float time = (startTime + delta * alpha) % 1f;

                Vector3 position = spline.EvaluatePosition(0, time);
                Gizmos.DrawSphere(position, size);
            }
        }

        private struct SocketLaunchState
        {
            public float PreviousTimeOnSpline;
            public float CurrentTimeOnSpline;

            // Cached derived values for debug & visualization purposes.
            // Not required for core trigger logic.
            public float SpeedMetersPerSecond;
            public float DistanceFromSocketToLandingMeters;
            public float LaunchWindowSizeMeters;

            public bool IsInsideLaunchWindow;
        }
    }
}
