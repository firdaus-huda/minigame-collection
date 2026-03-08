using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(ButtonController))]
public class ButtonView : MonoBehaviour
{
	protected const float DefaultAnimationDuration = 0.1f;
	private Tweener _scaleTween;
	protected bool inputEnabled = true;
		
	protected AudioEngine AudioEngine;

	private void Awake()
	{
		Initialize();
	}

	protected virtual void Initialize()
	{
		AudioEngine = Service.GetService<AudioEngine>();
	}

	public virtual void OnPointerDown()
	{
		if (!inputEnabled) return;
		_scaleTween?.Kill();
		ResetScale();
		_scaleTween = transform.DOScale(0.9f, DefaultAnimationDuration);
		AudioEngine.PlaySfx("CommonButtonDown");
	}

	public virtual void OnPointerUp()
	{
		if (!inputEnabled) return;
		_scaleTween?.Kill();
		_scaleTween = transform.DOScale(1f, DefaultAnimationDuration);
		AudioEngine.PlaySfx("CommonButtonUp");
	}

	public virtual void OnPointerEnter() { }

	public virtual void OnPointerExit() { }
	private void ResetScale()
	{
		transform.localScale = Vector3.one;
	}

	private void OnDestroy()
	{
		_scaleTween?.Kill();
	}
}