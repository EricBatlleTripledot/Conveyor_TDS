using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

namespace Game
{
    public static class SplinesExtensions
    {
        // Returns the nearest position, and the time eval on the spline
        public static (Vector3, float) GetNearestPointTo(this SplineContainer spline, float3 point, int attempts)
        {
            var timeSlice = 1f / attempts;
            var nearest = spline.EvaluatePosition(0);
            var dist = math.distance(nearest, point);
            var t = 0f;

            for (int i = 0; i <= attempts; i++)
            {
                var eval = spline.EvaluatePosition(timeSlice * i);
                var dist2 = math.distance(eval, point);

                if (dist2 < dist)
                {
                    t = timeSlice * i;
                    nearest = eval;
                    dist = dist2;
                }
            }

            return (nearest, t);
        }
    }
}