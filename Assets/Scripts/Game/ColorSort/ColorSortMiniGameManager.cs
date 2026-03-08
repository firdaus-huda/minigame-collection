using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ColorSortMiniGameManager : BaseMiniGameManager
{
    [SerializeField] private LevelSelectCanvas levelSelectCanvas;
    [SerializeField] private GridLayoutGroup layoutGroup;
    [SerializeField] private Ball ballPrefab;
    [SerializeField] private Bottle bottlePrefab;
    [SerializeField] private Transform poolParent;
    [SerializeField] private Transform gameParent;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject blocker;
    [SerializeField] private GameObject sidebar;
    [SerializeField] private ButtonController shuffleButton;
    [SerializeField] private ButtonController levelSelectButton;
    [SerializeField] private ParticleSystem winEffect;

    private const int EasyBottleCount = 5;
    private const int MediumBottleCount = 8;
    private const int HardBottleCount = 12;

    private const int EasyLayoutConstraint = 5;
    private const int MediumLayoutConstraint = 4;
    private const int HardLayoutConstraint = 6;
    
    private const int InitialBottlePoolSize = 12;
    private const int InitialBallPoolSize = 20;
    private readonly List<Bottle> _bottlePool = new();
    private readonly List<Bottle> _usedBottles = new();
    private readonly List<Ball> _ballPool = new();
    private readonly List<Ball> _usedBalls = new();
    
    private Bottle _currentSelectedBottle;
    private Difficulty _currentDifficulty;
    
    public override void Initialize()
    {
        base.Initialize();
        shuffleButton.ButtonUp += OnShuffleButtonClicked;
        levelSelectButton.ButtonUp += OnLevelSelectButtonClicked;
        canvas.worldCamera = Camera.main;
        PoolObjects();
        OnInitialized();

        void PoolObjects()
        {
            for (int i = 0; i < InitialBallPoolSize; i++)
            {
                var ball = Instantiate(ballPrefab, poolParent);
                ball.gameObject.SetActive(false);
                _ballPool.Add(ball);
            }

            for (int i = 0; i < InitialBottlePoolSize; i++)
            {
                var bottle = Instantiate(bottlePrefab, poolParent);
                bottle.Clicked += OnBottleClicked;
                bottle.gameObject.SetActive(false);
                _bottlePool.Add(bottle);
            }
        }
    }

    private void OnShuffleButtonClicked()
    {
        ResetGame();
        StartGame(_currentDifficulty);
    }

    private void OnLevelSelectButtonClicked()
    {
        _ = StartGame();
    }

    private void OnBottleClicked(Bottle bottle)
    {
        if (_currentSelectedBottle == null)
        {
            _currentSelectedBottle = bottle;
            _currentSelectedBottle.SetSelected(true);
        }
        else if (_currentSelectedBottle == bottle)
        {
            _currentSelectedBottle.SetSelected(false);
            _currentSelectedBottle = null;
        }
        else
        {
            _ = MoveBottleContents(_currentSelectedBottle, bottle);
        }
    }

    private async UniTask MoveBottleContents(Bottle from, Bottle to)
    {
        bool isValid = true;
        if (to.IsFull) isValid = false;
        else if (to.ContentCount > 0 && from.TopBall.Color != to.TopBall.Color) isValid = false;
        if (isValid) await StartMoveSequence();
        
        _currentSelectedBottle.SetSelected(false);
        _currentSelectedBottle = null;
        
        _ = CheckWinningCondition();

        async UniTask CheckWinningCondition()
        {
            foreach (var bottle in _usedBottles)
            {
                if (!bottle.IsCompleted)
                {
                    return;
                }
            }
            Debug.Log("Win");
            blocker.SetActive(true);
            winEffect.Play();
            sidebar.SetActive(false);
            AudioEngine.PlaySfx("Win");
            await Delay.Seconds(3f);
            _ = StartGame();
        }

        async UniTask StartMoveSequence()
        {
            to.SetSelected(true);
            blocker.SetActive(true);
            do
            {
                if (from.ContentCount == 0 || (to.ContentCount > 0 && from.TopBall.Color != to.TopBall.Color))
                {
                    blocker.SetActive(false);
                    to.SetSelected(false);
                    return;
                }

                var ball = await from.Get();
                _ = to.Insert(ball);
            } while (!to.IsFull);
            blocker.SetActive(false);
            to.SetSelected(false);
        }
    }

    public override async UniTask StartGame()
    {
        blocker.SetActive(false);
        sidebar.SetActive(false);
        AudioEngine.PlayBgm("ColorSort");
        ResetGame();
        _currentDifficulty = await levelSelectCanvas.Show();
        levelSelectCanvas.Hide();
        sidebar.SetActive(true);
        StartGame(_currentDifficulty);
    }

    private void StartGame(Difficulty difficulty)
    {
        PrepareBottles();

        switch (difficulty)
        {
            case Difficulty.Easy:
                layoutGroup.constraintCount = EasyLayoutConstraint;
                break;
            case Difficulty.Medium:
                layoutGroup.constraintCount = MediumLayoutConstraint;
                break;
            case Difficulty.Hard:
                layoutGroup.constraintCount = HardLayoutConstraint;
                break;
        }
        void PrepareBottles()
        {
            int bottleCount = 0;
            switch (difficulty)
            {
                case Difficulty.Easy:
                    bottleCount = EasyBottleCount;
                    break;
                case Difficulty.Medium:
                    bottleCount = MediumBottleCount;
                    break;
                case Difficulty.Hard:
                    bottleCount = HardBottleCount;
                    break;
            }

            List<BallColor> usedColors = Enum.GetValues(typeof(BallColor)).Cast<BallColor>().ToList();
            usedColors.Shuffle();
            List<Ball> balls = new();
            
            for (int i = 0; i < bottleCount; i++)
            {
                var bottle = GetBottleFromPool();
                bottle.gameObject.SetActive(true);
                bottle.transform.SetParent(gameParent);
                _usedBottles.Add(bottle);
                var color = usedColors[i];
                
                for (int j = 0; j < bottle.MaxContentCount; j++)
                {
                    if (i is 0 or 1 || difficulty == Difficulty.Medium && i is 2 || difficulty == Difficulty.Hard && i is 2 or 3)
                    {
                        balls.Add(null);
                        continue;
                    }

                    var ball = GetBallFromPool();
                    ball.Initialize();
                    ball.gameObject.SetActive(true);
                    ball.transform.SetParent(gameParent);
                    _usedBalls.Add(ball);
                    ball.SetColor(color);
                    balls.Add(ball);
                }
            }
            
            balls.Shuffle();
            int currentBallIndex = 0;
            foreach (var bottle in _usedBottles)
            {
                for (int i = 0; i < bottle.MaxContentCount; i++)
                {
                    bottle.Insert(balls[currentBallIndex], true);
                    currentBallIndex += 1;
                }
            }
        }

        Bottle GetBottleFromPool()
        {
            foreach (var bottle in _bottlePool)
                if (!bottle.gameObject.activeSelf)
                    return bottle;
            var newBottle = Instantiate(bottlePrefab, poolParent);
            newBottle.Clicked += OnBottleClicked;
            _bottlePool.Add(newBottle);
            return newBottle;
        }

        Ball GetBallFromPool()
        {
            foreach (var ball in _ballPool)
                if (!ball.gameObject.activeSelf)
                    return ball;
            var newBall = Instantiate(ballPrefab, poolParent);
            _ballPool.Add(newBall);
            return newBall;
        }
    }

    private void ResetGame()
    {
        foreach (var bottle in _usedBottles)
        {
            bottle.Clear();
            bottle.transform.SetParent(poolParent);
            bottle.gameObject.SetActive(false);
        }
        _usedBottles.Clear();
        foreach (var ball in _usedBalls)
        {
            ball.transform.SetParent(poolParent);
            ball.gameObject.SetActive(false);
        }
        _usedBalls.Clear();
    }

    public override void Destroy()
    {
        foreach (var bottle in _usedBottles)
        {
            bottle.Clicked -= OnBottleClicked;
        }
        Destroy(gameObject);
        base.Destroy();
    }
}