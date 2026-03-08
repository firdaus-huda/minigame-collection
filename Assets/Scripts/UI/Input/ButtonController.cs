using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    protected ButtonView View;
        
    public event Action ButtonUp;
    public event Action ButtonDown;

    private bool _isClick;
    protected bool inputEnabled = true;

    protected virtual void Awake()
    {
        View = GetComponent<ButtonView>();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (!inputEnabled) return;
        if (View != null) { View.OnPointerDown(); }

        _isClick = true;
        ButtonDown?.Invoke();
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (!inputEnabled) return;
        if (View != null) { View.OnPointerUp(); }

        if (_isClick) ButtonUp?.Invoke();
    }
		
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!inputEnabled) return;
        _isClick = false;
        if (View == null) return; 
        View.OnPointerExit();
    }
		
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!inputEnabled) return;
        if (View == null) return; 
        View.OnPointerEnter();
    }

    public void SetEnableInput(bool enable)
    {
        inputEnabled = enable;
    }
		
    protected virtual void OnDestroy()
    {
        ButtonUp = null;
        ButtonDown = null;
    }
}