using UnityEngine;
using DG.Tweening;

public class Transition : MonoBehaviour
{
    private static Transition _instance;
    [SerializeField] private CanvasGroup image;
    [SerializeField] private float fadeDuration;

    private Sequence _fadeSequence;
    private bool _isTransitioning;
    
    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void FadeTo(TweenCallback callback)
    {
        _instance.InternalFadeTo(callback);
    }
    
    private void InternalFadeTo(TweenCallback callback)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
        _fadeSequence?.Kill();
        _fadeSequence = DOTween.Sequence();
        _fadeSequence.AppendCallback(() => image.blocksRaycasts = true);
        _fadeSequence.Append(image.DOFade(1f, fadeDuration));
        _fadeSequence.AppendCallback(callback);
        _fadeSequence.Append(image.DOFade(0f, fadeDuration));
        _fadeSequence.AppendCallback(() => image.blocksRaycasts = false);
        _fadeSequence.AppendCallback(() => _isTransitioning = false);
    }
}   

