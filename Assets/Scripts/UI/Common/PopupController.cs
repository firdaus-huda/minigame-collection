using DG.Tweening;
using UnityEngine;

public class PopupController: MonoBehaviour
{
    [SerializeField] private ButtonController closeButton;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Transform frame;

    private Sequence _sequence;

    private void Awake()
    {
        closeButton.ButtonUp += OnCloseButtonClicked;
    }

    private void Start()
    {
        canvasGroup.alpha = 0f;
        frame.localScale = Vector3.one * 0.9f;
        _sequence = DOTween.Sequence();
        _sequence.Insert(0, frame.DOScale(Vector3.one, 0.1f));
        _sequence.Insert(0, canvasGroup.DOFade(1f, 0.1f));
    }

    private void OnDestroy()
    {
        closeButton.ButtonUp -= OnCloseButtonClicked;
        _sequence?.Kill();
    }

    public void Show(Transform parent)
    {
        Instantiate(this, parent);
    }

    private void OnCloseButtonClicked()
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        _sequence.Insert(0, frame.DOScale(Vector3.one * 0.9f, 0.1f));
        _sequence.Insert(0, canvasGroup.DOFade(0f, 0.1f));
        _sequence.OnComplete(() => Destroy(gameObject));
    }
}