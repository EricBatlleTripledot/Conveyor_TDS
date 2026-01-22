using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

namespace Game
{
    public static class SplinesExtensions
    {
        /// Approximates the nearest point on the spline to a world-space positionby uniform sampling.
        /// Returns both the world position and its normalized t.
        public static (Vector3, float) GetNearestPointTo(this SplineContainer spline, float3 point, int steps)
        {
            var timeSlice = 1f / steps;
            var nearest = spline.EvaluatePosition(0);
            var dist = math.distance(nearest, point);
            var t = 0f;

            for (var i = 0; i <= steps; i++) {
                var eval = spline.EvaluatePosition(timeSlice * i);
                var dist2 = math.distance(eval, point);

                if (dist2 < dist) {
                    t = timeSlice * i;
                    nearest = eval;
                    dist = dist2;
                }
            }

            return (nearest, t);
        }
        
        /// Returns the approximate world-space distance (in meters) traveled along the spline when moving forward from tFrom to tTo.
        /// Supports wrap-around for closed splines.
        public static float ApproxLengthForward(this SplineContainer spline, float tFrom, float tTo, int steps)
        {
            var len = 0f;
            Vector3 prev = spline.EvaluatePosition(0, tFrom);

            // how much deltaT forward we need (with wrapp)
            var delta = (tTo - tFrom);
            if (delta < 0f) {
                delta += 1f;
            }

            for (var i = 1; i <= steps; i++) {
                var a = (float)i / steps;
                var tNext = (tFrom + delta * a) % 1f; // %1 to ensure wrap in closed splines

                var p = (Vector3)spline.EvaluatePosition(0, tNext);
                len += Vector3.Distance(prev, p);
                prev = p;
            }

            return len;
        }
    }
}