using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PahudProject.Game.Nonogram
{
    public class NonogramMiniGameManager : BaseMiniGameManager
    {
        [SerializeField] private TextAsset levelDataText;
        [SerializeField] private LevelSpawner levelSpawner;
        [SerializeField] private Canvas canvas;
        [SerializeField] private ParticleSystem winEffect;

        [SerializeField] private ButtonController restartButton;
        [SerializeField] private ButtonController shuffleButton;
        [SerializeField] private ButtonController guideButton;
        [SerializeField] private PopupController guidePopup;
        
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
            guidePopup.Show(transform);
        }

        public override void Initialize()
        {
            base.Initialize();
            _levelData = JsonConvert.DeserializeObject<LevelData>(levelDataText.text);
            canvas.worldCamera = Camera.main;
            levelSpawner.Initialize();
            OnInitialized();
        }

        public override UniTask StartGame()
        {
            StartLevel(Random.Range(1,_levelData.Levels.Count));
            _gameStarted = true;

            return UniTask.CompletedTask;
        }

        private void StartLevel(int level)
        {
            var data = _levelData.Levels.Find(x => x.level == level);
            _currentLevelDetail = data;
            var tiles = levelSpawner.SpawnLevel(data);
            foreach (var tile in tiles)
            {
                tile.ButtonUp += OnTileClicked;
            }
            _currentTiles = tiles;
            AudioEngine.PlayBgm("Nonogram");
        }

        private void OnTileClicked()
        {
            CheckTilesValid();
        }

        private void CheckTilesValid()
        {
            var tileValues = _currentTiles.Select(x => x.TileState == TileState.Fill);
            var validValues = _currentLevelDetail.tiles.Select(x => !string.IsNullOrEmpty(x));

            var valid = tileValues.SequenceEqual(validValues);
            if (valid) OnLevelFinished();
        }

        private async void OnLevelFinished()
        {
            _gameStarted = false;
            AudioEngine.PlaySfx("Win");
            winEffect.Play();
            foreach (var tile in _currentTiles)
            {
                tile.SetEnableInput(false);
                tile.ButtonUp -= OnTileClicked;
            }
            
            await PlayWinAnimation();
            await Delay.Seconds(2f);
            levelSpawner.ClearLevel();
            StartGame();

            async UniTask PlayWinAnimation()
            {
                //Play win fx
                await Delay.Seconds(2f);
                for (int i = 0; i < _currentLevelDetail.columnCount; i++)
                {
                    var tiles = _currentTiles.Where(x => x.Column == i);
                    foreach (var tile in tiles)
                    {
                        await tile.RevealColor();
                    }
                }
            }
        }

        public override void Destroy()
        {
            Destroy(gameObject);
            base.Destroy();
        }
    }
}