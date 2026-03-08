using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Bottle : MonoBehaviour
{
    [SerializeField] private ButtonController button;
    [SerializeField] private BottleView view;
    [SerializeField] private Transform contentParent;

    private readonly Stack<Ball> _content = new();
    
    public Ball TopBall => _content.Count > 0 ? _content.Peek() : null;
    public int ContentCount => _content.Count;
    public int MaxContentCount => 4;
    public bool IsFull => ContentCount >= MaxContentCount;
    public bool IsCompleted => _content.Count == 0 || (IsFull && IsSingleColoredContents());
    
    public event Action<Bottle> Clicked;
    
    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        button.ButtonUp += OnButtonClicked;
    }

    private void OnDestroy()
    {
        button.ButtonUp -= OnButtonClicked;
    }

    public void Clear()
    {
        _content.Clear();
    }

    public async UniTask<Ball> Get()
    {
        if (_content.Count == 0) return null;
        var ball = _content.Pop();
        await ball.OnGetFromBottle();
        return ball;
    }

    public async UniTask Insert(Ball ball, bool immediate = false)
    {
        if (ball == null) return;
        _content.Push(ball);
        ball.transform.SetParent(contentParent);
        if (immediate) return;
        view.OnBallInserted();
        await ball.OnInsertToBottle();
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            transform.DOScale(Vector3.one * 1.1f, 0.2f);
        }
        else
        {
            transform.DOScale(Vector3.one, 0.2f);
        }
        view.OnSelected(selected);
    }

    public bool IsSingleColoredContents()
    {
        if (_content.Count == 0) return true;
        BallColor color = BallColor.None;
        foreach (var content in _content)
        {
            if (color == BallColor.None)
            {
                color = content.Color;
                continue;
            }

            if (color != content.Color)
            {
                return false;
            }
        }
        return true;
    }

    private void OnButtonClicked()
    {
        Clicked?.Invoke(this);
    }
}