using System;
using PahudProject.Common;
using PahudProject.Enum;
using PahudProject.Game;
using PahudProject.UI;
using PahudProject.UI.MainMenu;
using UnityEngine;

namespace PahudProject.Common
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private UIManager uiManager;
        [SerializeField] private MainMenuCanvas mainMenuCanvas;

        [SerializeField] private BaseMiniGameManager colorSortPrefab;
        [SerializeField] private BaseMiniGameManager mathPuzzlePrefab;
        [SerializeField] private BaseMiniGameManager nonogramPrefab;
        [SerializeField] private BaseMiniGameManager memoryPrefab;
        
        public static event Action<GameState> GameStateChanged;

        private GameState _currentGameState;
        private BaseMiniGameManager _currentMiniGame;
        
        private static GameManager _instance;
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
            mainMenuCanvas.MiniGameSelected += OnMiniGameSelected;
            uiManager.ExitMiniGameClicked += OnExitMiniGameClicked;
        }

        private void OnDestroy()
        {
            mainMenuCanvas.MiniGameSelected -= OnMiniGameSelected;
            uiManager.ExitMiniGameClicked -= OnExitMiniGameClicked;
        }

        private void Start()
        {
            AudioEngine.PlayBGM(BGMType.MainMenu);
        }

        private void OnMiniGameSelected(MiniGameType miniGameType)
        {
            if (_currentGameState != GameState.Menu) return;
            
            StartMiniGame(miniGameType);
        }

        private void OnExitMiniGameClicked()
        {
            _currentMiniGame.Destroy();
        }

        private void StartMiniGame(MiniGameType miniGameType)
        {
            ChangeGameState(GameState.Loading);
            
            switch (miniGameType)
            {
                case MiniGameType.ColorSort:
                    _currentMiniGame = Instantiate(colorSortPrefab);
                    break;
                case MiniGameType.MathPuzzle:
                    _currentMiniGame = Instantiate(mathPuzzlePrefab);
                    break;
                case MiniGameType.Nonogram:
                    _currentMiniGame = Instantiate(nonogramPrefab);
                    break;
                case MiniGameType.Memory:
                    _currentMiniGame = Instantiate(memoryPrefab);
                    break;
            }

            _currentMiniGame.IsReady += OnMiniGameReady;
            _currentMiniGame.IsDestroyed += OnMinigameDestroyed;
            _currentMiniGame.Initialize();
        }
        
        private void OnMiniGameReady()
        {
            ChangeGameState(GameState.Playing);
            _currentMiniGame.StartGame();
        }

        private void OnMinigameDestroyed()
        {
            _currentMiniGame.IsReady -= OnMiniGameReady;
            _currentMiniGame.IsDestroyed -= OnMinigameDestroyed;
            ChangeGameState(GameState.Menu);
        }

        private void ChangeGameState(GameState gameState)
        {
            if (_currentGameState == gameState) return;

            _currentGameState = gameState;
            GameStateChanged?.Invoke(gameState);
        }
    }
}