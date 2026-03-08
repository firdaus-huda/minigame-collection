using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class BaseMiniGameManager : MonoBehaviour
{
    public event Action IsReady;
    public event Action IsDestroyed;

    public virtual void Initialize()
    {
        AudioEngine = Service.GetService<AudioEngine>();
    }

    protected AudioEngine AudioEngine;

    protected void OnInitialized()
    {
        IsReady?.Invoke();
    }

    public abstract UniTask StartGame();

    public virtual void Destroy()
    {
        OnDestroyed();
    }

    private void OnDestroyed()
    {
        IsDestroyed?.Invoke();
    }
}