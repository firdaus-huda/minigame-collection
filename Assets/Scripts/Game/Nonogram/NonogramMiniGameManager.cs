using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PahudProject.Common;
using PahudProject.Enum;
using PahudProject.Game;
using PahudProject.Game.Memory;
using PahudProject.UI.Input;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PahudProject.Game.Nonogram
{
    public class NonogramMiniGameManager : BaseMiniGameManager
    {
        public override MiniGameType MiniGameType => MiniGameType.Nonogram;

        [SerializeField] private TextAsset levelDataText;
        [SerializeField] private LevelSpawner levelSpawner;
        [SerializeField] private Canvas canvas;
        [SerializeField] private ParticleSystem winEffect;

        [SerializeField] private ButtonController restartButton;
        [SerializeField] private ButtonController shuffleButton;
        [SerializeField] private ButtonController guideButton;
        
        private LevelData _levelData;
        private LevelDetail _currentLevelDetail;

        private TileButtonController[] _currentTiles;

        private bool _gameStarted;

        private void Awake()
        {
            restartButton.ButtonUp += OnRestartButtonClicked;
            shuffleButton.ButtonUp += OnShuffleButtonClicked;
            guideButton.ButtonUp += OnGuideButtonClicked;
        }

        private void OnDestroy()
        {
            restartButton.ButtonUp -= OnRestartButtonClicked;
            shuffleButton.ButtonUp -= OnShuffleButtonClicked;
            guideButton.ButtonUp -= OnGuideButtonClicked;
        }

        private void OnRestartButtonClicked()
        {
            if (!_gameStarted) return;
            foreach (var tile in _currentTiles)
            {
                tile.ResetData();
            }
        }

        private void OnShuffleButtonClicked()
        {
            levelSpawner.ClearLevel();
            StartGame();
        }

        private void OnGuideButtonClicked()
        {

        }

        public override UniTask Initialize()
        {
            _levelData = JsonConvert.DeserializeObject<LevelData>(levelDataText.text);
            canvas.worldCamera = Camera.main;
            levelSpawner.Initialize();
            OnInitialized();
            return UniTask.CompletedTask;
        }

        public override void StartGame()
        {
            StartLevel(Random.Range(1,_levelData.Levels.Count));
            _gameStarted = true;
        }

        private void StartLevel(int level)
        {
            var data = _levelData.Levels.Find(x => x.Level == level);
            _currentLevelDetail = data;
            var tiles = levelSpawner.SpawnLevel(data);
            foreach (var tile in tiles)
            {
                tile.ButtonUp += OnTileClicked;
            }
            _currentTiles = tiles;
            AudioEngine.PlayBGM(BGMType.Nonogram);
        }

        private void OnTileClicked()
        {
            CheckTilesValid();
        }

        private void CheckTilesValid()
        {
            var tileValues = _currentTiles.Select(x => x.TileState == TileState.Fill);
            var validValues = _currentLevelDetail.Tiles.Select(x => !string.IsNullOrEmpty(x));

            var valid = tileValues.SequenceEqual(validValues);
            if (valid) OnLevelFinished();
        }

        private async void OnLevelFinished()
        {
            _gameStarted = false;
            AudioEngine.PlaySfx(SFXType.Win);
            winEffect.Play();
            foreach (var tile in _currentTiles)
            {
                tile.SetEnableInput(false);
                tile.ButtonUp -= OnTileClicked;
            }
            
            await PlayWinAnimation();
            await new WaitForSeconds(2f);
            levelSpawner.ClearLevel();
            StartGame();

            async UniTask PlayWinAnimation()
            {
                //Play win fx
                await new WaitForSeconds(2f);
                for (int i = 0; i < _currentLevelDetail.ColumnCount; i++)
                {
                    var tiles = _currentTiles.Where(x => x.Column == i);
                    foreach (var tile in tiles)
                    {
                        await tile.RevealColor();
                    }
                }
            }
        }

        public override UniTask Destroy()
        {
            Destroy(gameObject);
            OnDestroyed();
            return UniTask.CompletedTask;
        }
    }
}