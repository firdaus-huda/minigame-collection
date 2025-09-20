using DG.Tweening;
using PahudProject.Common;
using PahudProject.Enum;
using PahudProject.UI.Input;
using UnityEngine;

namespace PahudProject.UI.MainMenu
{
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
}