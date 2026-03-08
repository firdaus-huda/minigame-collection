using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private MainMenuCanvas mainMenuCanvas;

    [SerializeField] private BaseMiniGameManager colorSortPrefab;
    [SerializeField] private BaseMiniGameManager nonogramPrefab;
    [SerializeField] private BaseMiniGameManager memoryPrefab;
    
    public static event Action<GameState> GameStateChanged;
    public static GameState CurrentGameState => _instance._currentGameState;

    private GameState _currentGameState = GameState.None;
    private BaseMiniGameManager _currentMiniGame;
    
    private static GameManager _instance;
    private AudioEngine _audioEngine;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _instance = this;
        
        mainMenuCanvas.MiniGameSelected += OnMiniGameSelected;
        uiManager.ExitMiniGameClicked += OnExitMiniGameClicked;
        
        _audioEngine = Service.GetService<AudioEngine>();
    }

    private void Start()
    {
        ChangeGameState(GameState.Menu);
    }

    private void OnDestroy()
    {
        mainMenuCanvas.MiniGameSelected -= OnMiniGameSelected;
        uiManager.ExitMiniGameClicked -= OnExitMiniGameClicked;
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