using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExtendedSlider: Slider
{
    public event Action OnPointerDownEvent; 
    public event Action OnPointerUpEvent;
        
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        OnPointerDownEvent?.Invoke();
    }
        
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        OnPointerUpEvent?.Invoke();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        OnPointerDownEvent = null;
        OnPointerUpEvent = null;
    }
}