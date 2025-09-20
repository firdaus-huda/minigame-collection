using System;
using System.Collections.Generic;
using DG.Tweening;
using PahudProject.Common;
using PahudProject.Enum;
using PahudProject.UI.Input;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace PahudProject.UI
{
    public class UIManager: MonoBehaviour
    {
        [SerializeField] private GameObject backgroundCanvas;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private CanvasGroup mainMenuCanvas;
        [SerializeField] private CanvasGroup loadingCanvas;
        [SerializeField] private CanvasGroup gameCanvas;

        [SerializeField] private ButtonController randomizeBgButton;
        [SerializeField] private ButtonController backButton;

        [SerializeField] private List<Color> bgColors = new();

        public event Action ExitMiniGameClicked;

        private const float FadeDuration = 0.5f;
        private CanvasGroup _currentActiveCanvas;
        private Sequence _transitionSequence;
        
        private void Awake()
        {
            GameManager.GameStateChanged += OnGameStateChanged;
            _currentActiveCanvas = mainMenuCanvas;
            mainMenuCanvas.alpha = 1f;
            loadingCanvas.alpha = 0f;
            gameCanvas.alpha = 0f;
            backgroundCanvas.SetActive(true);
            backButton.ButtonUp += OnBackButtonClicked;
            randomizeBgButton.ButtonUp += OnRandomizeBgButtonClicked;
            backgroundImage.material = Instantiate(backgroundImage.material);
        }

        private void OnDestroy()
        {
            GameManager.GameStateChanged -= OnGameStateChanged;
            backButton.ButtonUp -= OnBackButtonClicked;
            randomizeBgButton.ButtonUp -= OnRandomizeBgButtonClicked;
        }

        private void OnGameStateChanged(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Menu:
                    CreateTransition(_currentActiveCanvas, mainMenuCanvas);
                    _currentActiveCanvas = mainMenuCanvas;
                    break;
                case GameState.Loading:
                    _currentActiveCanvas.alpha = 0f;
                    _currentActiveCanvas.interactable = false;
                    _currentActiveCanvas = loadingCanvas;
                    loadingCanvas.alpha = 1;
                    break;
                case GameState.Playing:
                    CreateTransition(_currentActiveCanvas, gameCanvas);
                    _currentActiveCanvas = gameCanvas;
                    break;
            }
        }

        private void CreateTransition(CanvasGroup from, CanvasGroup to)
        {
            if(from == to) return;
            
            _transitionSequence?.Kill();
            _transitionSequence = DOTween.Sequence();

            _transitionSequence.AppendCallback(() => from.interactable = false);
            _transitionSequence.Append(from.DOFade(0f, FadeDuration));

            _transitionSequence.Append(to.DOFade(1f, FadeDuration));
            _transitionSequence.AppendCallback(() => to.interactable = true);
        }

        private void OnBackButtonClicked()
        {
            ExitMiniGameClicked?.Invoke();
        }

        private void OnRandomizeBgButtonClicked()
        {
            var tiling = Random.Range(0.5f, 1f);
            backgroundImage.material.SetVector("_Tiling", Vector2.one * tiling);
            backgroundImage.material.SetVector("_Direction", new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
            backgroundImage.material.SetColor("_Color", Random.ColorHSV(0f, 1f, 0.25f, 0.5f, 1f, 1f));
        }
    }
}