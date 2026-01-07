using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "AnimationView_", menuName = "Animation View", order = 0)]
public class AnimationView : ScriptableObject
{
    [Header("Exit Animation Settings")]
    [SerializeField]
    private float exitDuration = 0.5f;
    [SerializeField]
    private float exitScale = 0.1f;
    [SerializeField]
    private Ease exitEase = Ease.InBack;
    
    public async UniTask PlayExitAnimation(Transform transform)
    {
        var sequence = DOTween.Sequence();
        sequence.Join(transform.DOScale(exitScale, exitDuration).SetEase(exitEase));
        sequence.Join(transform.DOScale(Vector3.zero, exitDuration / 2.0f).SetEase(exitEase).SetDelay(-0.1f));
        await sequence.AsyncWaitForCompletion();
    }

    public void PlayEnterAnimation(GameObject obj)
    {
        Debug.Log("Enter animation played for " + obj.name);
    }

    public void PlayClearAnimation(Transform obj1)
    {
        Debug.Log("Clear animation played for " + obj1.name);
    }
    
    public void PlayGridSpawnAnimation(Transform obj2)
    {
        Debug.Log("Grid spawn animation played for " + obj2.name);
    }
}