using DG.Tweening;
using PahudProject.Common;
using PahudProject.Enum;
using UnityEngine;
using UnityEngine.UI;

namespace PahudProject.UI.Input
{
	[RequireComponent(typeof(ButtonController))]
	public class ButtonView : MonoBehaviour
	{
		protected const float DefaultAnimationDuration = 0.1f;
		private Tweener _scaleTween;
		protected bool inputEnabled = true;
		public virtual void OnPointerDown()
		{
			if (!inputEnabled) return;
			_scaleTween?.Kill();
			ResetScale();
			_scaleTween = transform.DOScale(0.9f, DefaultAnimationDuration);
			AudioEngine.PlaySfx(SFXType.CommonButtonDown);
		}

		public virtual void OnPointerUp()
		{
			if (!inputEnabled) return;
			_scaleTween?.Kill();
			_scaleTween = transform.DOScale(1f, DefaultAnimationDuration);
			AudioEngine.PlaySfx(SFXType.CommonButtonUp);
		}

		public virtual void OnPointerEnter() { }

		public virtual void OnPointerExit() { }
		
		public void SetEnableInput(bool enable)
		{
			inputEnabled = enable;
		}
		private void ResetScale()
		{
			transform.localScale = Vector3.one;
		}
	}
}