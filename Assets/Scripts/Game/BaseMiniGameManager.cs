using System;
using Cysharp.Threading.Tasks;
using PahudProject.Enum;
using UnityEngine;

namespace PahudProject.Game
{
    public abstract class BaseMiniGameManager : MonoBehaviour
    {
        public abstract MiniGameType MiniGameType { get; }
        public event Action IsReady;
        public event Action IsDestroyed;
        public abstract UniTask Initialize();

        protected void OnInitialized()
        {
            IsReady?.Invoke();
        }

        public abstract void StartGame();
        public abstract UniTask Destroy();

        protected void OnDestroyed()
        {
            IsDestroyed?.Invoke();
        }
    }
}