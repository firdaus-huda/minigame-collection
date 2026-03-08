using System;
using UnityEngine.EventSystems;

public class Card: ButtonController
{
    public event Action<Card> CardClicked;
        
    private CardView CardView => View as CardView;
    public int Value { get; private set; }
    public bool Matched { get; set; }
    private bool _isRevealed;
    public void SetData(int value)
    {
        Value = value;
        CardView.SetSprite(value);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        CardView.ResetView();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Reveal()
    {
        CardView.Reveal();
    }

    public void TriggerUnmatchedEvent()
    {
        CardView.Shake();
    }

    public void Conceal()
    {
        CardView.Conceal();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!inputEnabled) return;
        base.OnPointerUp(eventData);
        CardClicked?.Invoke(this);
    }
}