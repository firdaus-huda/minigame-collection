using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Ball: MonoBehaviour
{
    public BallColor Color { get; private set; } = BallColor.None;
        
    [SerializeField] private Image image;

    [SerializeField] private Color red;
    [SerializeField] private Color green;
    [SerializeField] private Color blue;
    [SerializeField] private Color yellow;
    [SerializeField] private Color orange;
    [SerializeField] private Color purple;
    [SerializeField] private Color cyan;
    [SerializeField] private Color brown;
    [SerializeField] private Color pink;
    [SerializeField] private Color gray;
    [SerializeField] private Color black;
    [SerializeField] private Color white;
    
    private const float AnimationDuration = 0.15f;
    private Sequence _flyOutSequence;
    private Sequence _flyInSequence;
    
    private bool _isInitialized;

    public void Initialize()
    {
        if (_isInitialized) return;
        _isInitialized = true;
        
        _flyInSequence = DOTween.Sequence().SetAutoKill(false).Pause();
        
        _flyInSequence.Append(image.transform.DOLocalMoveY(0f, AnimationDuration));
        _flyInSequence.Join(image.DOFade(1f, AnimationDuration));
        
        _flyOutSequence = DOTween.Sequence().SetAutoKill(false).Pause();
        _flyOutSequence.Append(image.transform.DOLocalMoveY(25f, AnimationDuration));
        _flyOutSequence.Join(image.DOFade(0f, AnimationDuration));
    }

    public void SetColor(BallColor color)
    {
        switch (color)
        {
            case BallColor.Red:
                image.color = red;
                break;
            case BallColor.Green:
                image.color = green;
                break;
            case BallColor.Blue:
                image.color = blue;
                break;
            case BallColor.Yellow:
                image.color = yellow;
                break;
            case BallColor.Orange:
                image.color = orange;
                break;
            case BallColor.Purple:
                image.color = purple;
                break;
            case BallColor.White:
                image.color = white;
                break;
            case BallColor.Brown:
                image.color = brown;
                break;
            case BallColor.Pink:
                image.color = pink;
                break;
            case BallColor.Cyan:
                image.color = cyan;
                break;
            case BallColor.Gray:
                image.color = gray;
                break;
            case BallColor.Black:
                image.color = black;
                break;
        }

        Color = color;
    }

    public async UniTask OnGetFromBottle()
    {
        _flyOutSequence?.Restart();
        await UniTask.WaitUntil(_flyOutSequence.IsComplete);
    }

    public async UniTask OnInsertToBottle()
    {
        transform.localScale = Vector3.one;
        _flyInSequence?.Restart();
        await UniTask.WaitUntil(_flyInSequence.IsComplete);
    }

    private void OnDestroy()
    {
        _flyInSequence?.Kill();
        _flyOutSequence?.Kill();
    }
}