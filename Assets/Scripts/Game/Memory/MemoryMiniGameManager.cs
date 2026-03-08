using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PahudProject.Game.Memory
{
    public class MemoryMiniGameManager : BaseMiniGameManager
    {
        [SerializeField] private LevelSelectCanvas levelSelectCanvas;
        [SerializeField] private Card cardPrefab;
        [SerializeField] private Transform poolParent;
        [SerializeField] private Transform gameParent;
        [SerializeField] private GameObject blocker;
        [SerializeField] private Canvas canvas;
        [SerializeField] private ParticleSystem winEffect;
        [SerializeField] private ButtonController levelSelectButton;
        [SerializeField] private GameObject sidebar;

        private const int EasyCardCount = 12;
        private const int MediumCardCount = 18;
        private const int HardCardCount = 24;
        private const int PoolCount = 24;
        private readonly List<Card> _pooledCards = new();
        private readonly List<Card> _usedCards = new();

        private Card _revealedCardValueA;
        private Card _revealedCardValueB;
 
        public override void Initialize()
        {
            base.Initialize();
            levelSelectButton.ButtonUp += OnLevelSelectButtonClicked;
            levelSelectCanvas.Hide();
            canvas.worldCamera = Camera.main;
            PoolCard();
            OnInitialized();

            void PoolCard()
            {
                for (int i = 0; i < PoolCount; i++)
                {
                    var card = Instantiate(cardPrefab, poolParent);
                    card.Hide();
                    card.CardClicked += OnCardClicked;
                    _pooledCards.Add(card);
                }
            }
        }

        public override async UniTask StartGame()
        {
            blocker.SetActive(false);
            sidebar.SetActive(false);
            AudioEngine.PlayBgm("Memory");
            ClearLevel();
            var difficulty = await levelSelectCanvas.Show();
            levelSelectCanvas.Hide();
            sidebar.SetActive(true);
            _ = StartGameSequence(difficulty);
        }
        
        private async UniTask StartGameSequence(Difficulty difficulty)
        {
            PrepareCards();
            await CreateCardAnimationSequence();
            
            void PrepareCards()
            {
                int usedCardCount = 0;
                switch (difficulty)
                {
                    case Difficulty.Easy:
                        usedCardCount = EasyCardCount;
                        break;
                    case Difficulty.Medium:
                        usedCardCount = MediumCardCount;
                        break;
                    case Difficulty.Hard:
                        usedCardCount = HardCardCount;
                        break;
                }

                for (int i = 0; i < usedCardCount; i++)
                {
                    var card = _pooledCards[i];
                    card.SetData((i / 2) + 1);
                    card.transform.SetParent(gameParent);
                    card.SetEnableInput(false);
                    card.Matched = false;
                    _usedCards.Add(card);
                }
                
                ShuffleCards();
            }
            
            void ShuffleCards()
            {
                List<int> indexes = new List<int>();
                List<Transform> items = new List<Transform>();
                for (int i = 0; i < gameParent.childCount;++i)
                {
                    indexes.Add(i);
                    items.Add(gameParent.GetChild(i));
                }
          
                foreach (var item in items)
                {
                    item.SetSiblingIndex(indexes[Random.Range(0, indexes.Count)]);
                }
            }

            async UniTask CreateCardAnimationSequence()
            {
                blocker.SetActive(true);
                foreach (var card in _usedCards)
                {
                    await Delay.Seconds(0.05f);
                    card.Show();
                    card.SetEnableInput(true);
                    AudioEngine.PlaySfx("MemoryCardShow");
                }

                blocker.SetActive(false);
            }
        }

        private void OnCardClicked(Card card)
        {
            blocker.SetActive(true);
            if (_revealedCardValueA == null && _revealedCardValueB == null)
            {
                _revealedCardValueA = card;
                _revealedCardValueA.Reveal();
            }
            else if (_revealedCardValueA == card)
            {
                _revealedCardValueA.Conceal();
                _revealedCardValueA = null;
                blocker.SetActive(false);
                return;
            }
            else if (_revealedCardValueA != null)
            {
                _revealedCardValueB = card;
                _revealedCardValueB.Reveal();
                ValidateCards();
                return;
            }
            blocker.SetActive(false);
        }

        private async UniTask ValidateCards()
        {
            blocker.SetActive(true);
            if (_revealedCardValueA.Value == _revealedCardValueB.Value)
            {
                _revealedCardValueA.SetEnableInput(false);
                _revealedCardValueB.SetEnableInput(false);
                _revealedCardValueA.Matched = true;
                _revealedCardValueB.Matched = true;
                AudioEngine.PlaySfx("MemoryCardMatched");
            }
            else
            {
                AudioEngine.PlaySfx("MemoryCardNotMatched");
                _revealedCardValueA.TriggerUnmatchedEvent();
                _revealedCardValueB.TriggerUnmatchedEvent();
                await Delay.Seconds(1f);
                _revealedCardValueA.Conceal();
                _revealedCardValueB.Conceal();
            }
            _revealedCardValueA = null;
            _revealedCardValueB = null;
            
            blocker.SetActive(false);

            foreach (var card in _usedCards)
            {
                if (!card.Matched) return;
            }
            winEffect.Play();
            sidebar.SetActive(false);
            AudioEngine.PlaySfx("Win");
            await Delay.Seconds(3f);
            _ = StartGame();
        }

        private void ClearLevel()
        {
            foreach (var card in _usedCards)
            {
                card.Hide();
                card.Matched = false;
                card.transform.SetParent(poolParent);
            }
            _usedCards.Clear();
        }

        private void OnLevelSelectButtonClicked()
        {
            _ = StartGame();
        }

        public override void Destroy()
        {
            levelSelectButton.ButtonUp -= OnLevelSelectButtonClicked;
            Destroy(gameObject);
            base.Destroy();
        }
    }
}