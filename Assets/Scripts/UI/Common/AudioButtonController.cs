using DG.Tweening;
using PahudProject.Common;
using PahudProject.UI.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PahudProject.UI.Common
{
    public class AudioButtonController: ButtonController
    {
        [SerializeField] private AudioVolumeSlider volumeSlider;

        private const float AnimationDuration = 0.25f;
        
        [SerializeField] private bool _isHoveringButton;
        [SerializeField] private bool _isHoveringSlider;
        [SerializeField] private bool _isClickingSlider;

        private Tweener _scaleTween;
        protected override void Awake()
        {
            base.Awake();
            volumeSlider.IsHovered += OnSliderHoverChanged;
            volumeSlider.IsClicked += OnSliderClickChanged;
            
            volumeSlider.gameObject.SetActive(false);
            GameSettings.VolumeChanged += OnVolumeChanged;
            if (view is AudioButtonView audioButtonView) audioButtonView.UpdateSprite(GameSettings.GetVolume());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            volumeSlider.IsHovered -= OnSliderHoverChanged;
            volumeSlider.IsClicked -= OnSliderClickChanged;
            GameSettings.VolumeChanged -= OnVolumeChanged;
        }

        private void OnVolumeChanged(float volume)
        {
            if (view is AudioButtonView audioView)
            {
                audioView.UpdateSprite(volume);
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            _isHoveringButton = true;
            _isHoveringSlider = true;
            ShowSlider();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            _isHoveringButton = false;

            if (!_isHoveringSlider && !_isClickingSlider) HideSlider();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            volumeSlider.SetValue(GameSettings.GetVolume() > 0f ? 0f : 0.5f);
        }

        private void OnSliderHoverChanged(bool hovered)
        {
            _isHoveringSlider = hovered;

            if (!_isHoveringSlider && !_isHoveringButton && !_isClickingSlider) HideSlider();
        }

        private void OnSliderClickChanged(bool clicked)
        {
            _isClickingSlider = clicked;
            
            if (!_isHoveringSlider && !_isHoveringButton && !_isClickingSlider) HideSlider();
        }

        private void ShowSlider()
        {
            if (volumeSlider.gameObject.activeSelf) return;
            _scaleTween?.Kill();
            volumeSlider.gameObject.SetActive(true);
            volumeSlider.transform.localScale = Vector2.one * 0.9f;
            _scaleTween = volumeSlider.transform.DOScale(1f, AnimationDuration).SetEase(Ease.OutElastic);
        }

        private void HideSlider()
        {
            if (!volumeSlider.gameObject.activeSelf) return;
            _scaleTween?.Kill();
            _scaleTween = volumeSlider.transform.DOScale(0.5f, AnimationDuration).SetEase(Ease.InQuart).OnComplete(() => volumeSlider.gameObject.SetActive(false));
        }
    }
}