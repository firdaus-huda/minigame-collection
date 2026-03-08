using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    private void Awake()
    {
        _ = Initialize();
    }

    private async UniTask Initialize()
    {
        Application.targetFrameRate = 60;

        await Service.InitializeAsync();

        await SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
        
        SceneManager.UnloadSceneAsync(gameObject.scene);
    }
}
