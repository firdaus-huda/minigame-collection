using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class AudioVolumeSlider: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ExtendedSlider slider;
        
    public event Action<bool> IsHovered;
    public event Action<bool> IsClicked;
        
    private GameSettings _gameSettings;

    private void Awake()
    {
        _gameSettings = Service.GetService<GameSettings>();
            
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        slider.OnPointerDownEvent += OnSliderPointerDown;
        slider.OnPointerUpEvent += OnSliderPointerUp;
        slider.value = _gameSettings.GetVolume();
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
        _gameSettings.SetVolume(value);
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
        _gameSettings.SetVolume(slider.value, true);
    }
}