using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using PahudProject.Common;
using PahudProject.Enum;
using PahudProject.Game;
using PahudProject.UI.Input;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PahudProject.Game.Memory
{
    public class MemoryMiniGameManager : BaseMiniGameManager
    {
        public override MiniGameType MiniGameType => MiniGameType.Memory;
        
        [SerializeField] private LevelSelectCanvas levelSelectCanvas;
        [SerializeField] private Card cardPrefab;
        [SerializeField] private Transform poolParent;
        [SerializeField] private Transform gameParent;
        [SerializeField] private GameObject blocker;
        [SerializeField] private Canvas canvas;
        [SerializeField] private ParticleSystem winEffect;

        private const int EasyCardCount = 12;
        private const int MediumCardCount = 18;
        private const int HardCardCount = 24;
        private const int PoolCount = 24;
        private List<Card> _pooledCards = new();
        private List<Card> _usedCards = new();

        private Card _revealedCardValueA;
        private Card _revealedCardValueB;
 
        public override UniTask Initialize()
        {
            levelSelectCanvas.Hide();
            canvas.worldCamera = Camera.main;
            PoolCard();
            OnInitialized();
            return UniTask.CompletedTask;

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

        public override async void StartGame()
        {
            blocker.SetActive(false);
            AudioEngine.PlayBGM(BGMType.Memory);
            ClearLevel();
            var difficulty = await levelSelectCanvas.Show();
            levelSelectCanvas.Hide();
            
            StartGameSequence(difficulty);
        }
        
        private async void StartGameSequence(Difficulty difficulty)
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
                    await new WaitForSeconds(0.05f);
                    card.Show();
                    card.SetEnableInput(true);
                    AudioEngine.PlaySfx(SFXType.MemoryCardShow);
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

        private async void ValidateCards()
        {
            blocker.SetActive(true);
            if (_revealedCardValueA.Value == _revealedCardValueB.Value)
            {
                _revealedCardValueA.SetEnableInput(false);
                _revealedCardValueB.SetEnableInput(false);
                _revealedCardValueA.Matched = true;
                _revealedCardValueB.Matched = true;
                AudioEngine.PlaySfx(SFXType.MemoryCardMatched);
            }
            else
            {
                AudioEngine.PlaySfx(SFXType.MemoryCardNotMatched);
                _revealedCardValueA.TriggerUnmatchedEvent();
                _revealedCardValueB.TriggerUnmatchedEvent();
                await new WaitForSeconds(1f);
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
            AudioEngine.PlaySfx(SFXType.Win);
            await new WaitForSeconds(2f);
            StartGame();
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

        public override UniTask Destroy()
        {
            _ = levelSelectCanvas.Destroy();
            Destroy(gameObject);
            OnDestroyed();
            return UniTask.CompletedTask;
        }
    }
}