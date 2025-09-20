using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PahudProject.UI.Input;
using UnityEngine;

namespace PahudProject.Game.Memory
{
    public class LevelSelectCanvas: MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        
        [SerializeField] private ButtonController easyButton;
        [SerializeField] private ButtonController mediumButton;
        [SerializeField] private ButtonController hardButton;

        private UniTaskCompletionSource<Difficulty> _tcs;

        private void Awake()
        {
            easyButton.ButtonUp += () => SetDifficulty(Difficulty.Easy);
            mediumButton.ButtonUp += () => SetDifficulty(Difficulty.Medium);
            hardButton.ButtonUp += () => SetDifficulty(Difficulty.Hard);
        }

        public UniTask<Difficulty> Show()
        {
            gameObject.SetActive(true);
            canvasGroup.DOFade(1f, 0.5f);
            canvasGroup.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f);
            canvasGroup.interactable = true;
            _tcs = new UniTaskCompletionSource<Difficulty>();
            return _tcs.Task;
        }

        public void Hide()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            gameObject.SetActive(false);
        }

        private void SetDifficulty(Difficulty difficulty)
        {
            _tcs?.TrySetResult(difficulty);
            _tcs = null;
        }

        public UniTask Destroy()
        {
            _tcs?.TrySetCanceled();
            return UniTask.CompletedTask;
        }
    }
}