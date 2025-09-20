using System;
using PahudProject.Common;
using PahudProject.UI.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PahudProject.UI.Common
{
    public class AudioVolumeSlider: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private ExtendedSlider slider;
        
        public event Action<bool> IsHovered;
        public event Action<bool> IsClicked;

        private void Awake()
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
            slider.OnPointerDownEvent += OnSliderPointerDown;
            slider.OnPointerUpEvent += OnSliderPointerUp;
            slider.value = GameSettings.GetVolume();
        }

        private void OnDestroy()
        {
            slider.OnPointerDownEvent -= OnSliderPointerDown;
            slider.OnPointerUpEvent -= OnSliderPointerUp;
            IsHovered = null;
        }

        public void SetValue(float value)
        {
            slider.value = value;
        }

        private void OnSliderValueChanged(float value)
        {
            GameSettings.SetVolume(value);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsHovered?.Invoke(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsHovered?.Invoke(false);
        }

        private void OnSliderPointerDown()
        {
            IsClicked?.Invoke(true);
        }
        
        private void OnSliderPointerUp()
        {
            IsClicked?.Invoke(false);
            GameSettings.SetVolume(slider.value, true);
        }
    }
}