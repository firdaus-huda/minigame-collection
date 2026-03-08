using System;
using UnityEngine;

public class MainMenuCanvas : MonoBehaviour
{
    [SerializeField] private ButtonController colorSortButton;
    [SerializeField] private ButtonController nonogramButton;
    [SerializeField] private ButtonController memoryButton;

    public event Action<MiniGameType> MiniGameSelected;
        
    private AudioEngine _audioEngine;

    private void Awake()
    {
        colorSortButton.ButtonUp += () => MiniGameClicked(MiniGameType.ColorSort);
        nonogramButton.ButtonUp +=  () => MiniGameClicked(MiniGameType.Nonogram);
        memoryButton.ButtonUp +=  () => MiniGameClicked(MiniGameType.Memory);

        _audioEngine = Service.GetService<AudioEngine>();
        GameManager.GameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.GameStateChanged -= OnGameStateChanged;
    }

    private void MiniGameClicked(MiniGameType type)
    {
        MiniGameSelected?.Invoke(type);
    }

    private void OnGameStateChanged(GameState gameState)
    {
        if (gameState == GameState.Menu) _audioEngine.PlayBgm("MainMenu");
    }
}