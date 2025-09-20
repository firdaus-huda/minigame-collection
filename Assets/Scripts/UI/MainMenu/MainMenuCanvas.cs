using System;
using PahudProject.Common;
using PahudProject.Enum;
using PahudProject.UI.Input;
using UnityEngine;

namespace PahudProject.UI.MainMenu
{
    public class MainMenuCanvas : MonoBehaviour
    {
        [SerializeField] private ButtonController colorSortButton;
        [SerializeField] private ButtonController mathPuzzleButton;
        [SerializeField] private ButtonController nonogramButton;
        [SerializeField] private ButtonController memoryButton;

        public event Action<MiniGameType> MiniGameSelected;

        private void Awake()
        {
            colorSortButton.ButtonUp += () => MiniGameClicked(MiniGameType.ColorSort);
            mathPuzzleButton.ButtonUp +=  () => MiniGameClicked(MiniGameType.MathPuzzle);
            nonogramButton.ButtonUp +=  () => MiniGameClicked(MiniGameType.Nonogram);
            memoryButton.ButtonUp +=  () => MiniGameClicked(MiniGameType.Memory);

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
            if (gameState == GameState.Menu) AudioEngine.PlayBGM(BGMType.MainMenu);
        }
    }
}

