using System;
using System.Collections.Generic;
using DG.Tweening;
using PahudProject.UI.Input;
using UnityEngine;
using UnityEngine.UI;

namespace PahudProject.Game.Memory
{
    public class CardView: ButtonView
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image bg;
        [SerializeField] private List<Sprite> sprites = new();
        [SerializeField] private Color32 normalBgColor;
        [SerializeField] private Color32 redBgColor;

        private Tweener _fadeTween;
        private Tweener _shakeTween;
        private Tweener _colorTween;

        private void Awake()
        {
            ResetView();
        }

        public void SetSprite(int index)
        {
            icon.sprite = sprites[index];
        }

        public void Reveal()
        {
            _fadeTween?.Kill();
            icon.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0f), DefaultAnimationDuration * 3f);
            _fadeTween = icon.DOFade(1f, 0.2f);
        }

        public void Shake()
        {
            _shakeTween?.Kill();
            _colorTween?.Kill();
            _shakeTween = icon.transform.DOShakePosition(1f, 10f);
            _colorTween = bg.DOColor(redBgColor, 0.4f).OnComplete(() =>
                _colorTween = bg.DOColor(normalBgColor, 0.4f));
        }

        public void Conceal()
        {
            _fadeTween?.Kill();
            _fadeTween = icon.DOFade(0f, 0.1f);
        }

        public void ResetView()
        {
            var color = (Color32) icon.color;
            icon.color = new Color32(color.r, color.g, color.b, 0);
            bg.color = normalBgColor;
        }

        private void OnDestroy()
        {
            _fadeTween.Kill();
            _shakeTween.Kill();
        }
    }
}