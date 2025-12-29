using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LevelEditor
{
	public class CustomButton : Selectable, IPointerClickHandler
	{
		[Header("Click Events")] 
		public UnityEvent onLeftClick;
		public UnityEvent onMiddleClick;
		public UnityEvent onRightClick;
        
		private Coroutine resetCoroutine;

		public void OnPointerClick(PointerEventData eventData)
		{
			if (!IsActive() || !IsInteractable())
			{
				return;
			}

			DoStateTransition(SelectionState.Pressed, true);

			switch (eventData.button)
			{
				default:
				case PointerEventData.InputButton.Left:
					onLeftClick?.Invoke();
					break;
				case PointerEventData.InputButton.Middle:
					onMiddleClick?.Invoke();
					break;
				case PointerEventData.InputButton.Right:
					onRightClick?.Invoke();
					break;
			}

			if (resetCoroutine != null)
			{
				StopCoroutine(resetCoroutine);
			}

			// This is needed in case the click on this button destroyed the button itself
			if (!this)
			{
				return;
			}

			resetCoroutine = StartCoroutine(OnFinishSubmit());
		}

		private IEnumerator OnFinishSubmit()
		{
			const float fadeTime = 0.01f;
			var elapsedTime = 0f;

			while (elapsedTime < fadeTime)
			{
				elapsedTime += Time.unscaledDeltaTime;
				yield return null;
			}

			DoStateTransition(currentSelectionState, false);
		}
	}
}