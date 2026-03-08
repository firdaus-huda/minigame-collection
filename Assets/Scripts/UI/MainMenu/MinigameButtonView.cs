using DG.Tweening;
using UnityEngine;

public class MinigameButtonView: ButtonView
{
    private Tweener _scaleTween;

    public override void OnPointerEnter()
    {
        _scaleTween?.Kill();
        ResetScale();
        _scaleTween = transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0f), DefaultAnimationDuration * 3f);
    }

    private void ResetScale()
    {
        transform.localScale = Vector3.one;
    }
}