using System.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Game
{
    public static class DoTweenExtensions
    {
        public static TweenerCore<Vector2, Vector2, VectorOptions> DOAnchorPos(
            this RectTransform rectTransform,
            Vector2 endValue,
            float duration,
            bool snapping = false)
        {
            if (rectTransform == null) {
                return null;
            }

            var t = (TweenerCore<Vector2, Vector2, VectorOptions>)DOTween.To(
                    () => rectTransform != null ? rectTransform.anchoredPosition : Vector2.zero,
                    x => {
                        if (rectTransform != null) {
                            rectTransform.anchoredPosition = x;
                        }
                    },
                    endValue,
                    duration
                )
                .SetOptions(snapping)
                .SetAutoKill(true);

            if (rectTransform != null && rectTransform.gameObject != null) {
                t.SetLink(rectTransform.gameObject, LinkBehaviour.KillOnDestroy);
            }

            return t;
        }

        public static Task AsyncWaitForCompletion(this Tween t)
        {
            var tcs = new TaskCompletionSource<bool>();
            t.OnComplete(() => tcs.TrySetResult(true));
            t.OnKill(() => tcs.TrySetResult(true));
            return tcs.Task;
        }
        
        public static Task AsyncWaitForCompletion(this Sequence t)
        {
            var tcs = new TaskCompletionSource<bool>();
            t.OnComplete(() => tcs.TrySetResult(true));
            t.OnKill(() => tcs.TrySetResult(true));
            return tcs.Task;
        }

        public static Task AsyncWaitForRewind(this Sequence t)
        {
            var tcs = new TaskCompletionSource<bool>();
            t.OnRewind(() => tcs.TrySetResult(true));
            t.OnKill(() => tcs.TrySetResult(true));
            return tcs.Task;
        }

        public static Task AsyncWaitForKill(this Sequence t)
        {
            var tcs = new TaskCompletionSource<bool>();
            t.OnKill(() => tcs.TrySetResult(true));
            return tcs.Task;
        }

        public static Task AsyncWaitForElapsedLoops(this Sequence t, int elapsedLoops)
        {
            var tcs = new TaskCompletionSource<bool>();
            t.OnStepComplete(() => {
                if (t.CompletedLoops() >= elapsedLoops) {
                    tcs.TrySetResult(true);
                }
            });
            t.OnKill(() => tcs.TrySetResult(true));
            return tcs.Task;
        }

        public static Task AsyncWaitForPosition(this Sequence t, float position)
        {
            var tcs = new TaskCompletionSource<bool>();
            t.OnUpdate(() => {
                if (t.Elapsed() >= position) {
                    tcs.TrySetResult(true);
                }
            });
            t.OnKill(() => tcs.TrySetResult(true));
            return tcs.Task;
        }

        public static Task AsyncWaitForStart(this Sequence t)
        {
            var tcs = new TaskCompletionSource<bool>();
            t.OnStart(() => tcs.TrySetResult(true));
            t.OnKill(() => tcs.TrySetResult(true));
            return tcs.Task;
        }
    }
}