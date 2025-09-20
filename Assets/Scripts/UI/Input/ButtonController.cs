using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PahudProject.UI.Input
{
	public class ButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
	{
		protected ButtonView view;
        
		public event Action ButtonUp;
		public event Action ButtonDown;

		private bool _isClick;
		protected bool inputEnabled = true;

		protected virtual void Awake()
		{
			view = GetComponent<ButtonView>();
		}

		public virtual void OnPointerDown(PointerEventData eventData)
		{
			if (!inputEnabled) return;
			if (view != null) { view.OnPointerDown(); }

			_isClick = true;
			ButtonDown?.Invoke();
		}

		public virtual void OnPointerUp(PointerEventData eventData)
		{
			if (!inputEnabled) return;
			if (view != null) { view.OnPointerUp(); }

			if (_isClick) ButtonUp?.Invoke();
		}
		
		public virtual void OnPointerExit(PointerEventData eventData)
		{
			if (!inputEnabled) return;
			_isClick = false;
			view.OnPointerExit();
		}
		
		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			if (!inputEnabled) return;
			view.OnPointerEnter();
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
}