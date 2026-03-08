using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class Service
{
    private static readonly List<IGameService> Services = new();

    public static bool Initialized { get; private set; }

    public static async UniTask InitializeAsync()
    {
        if (Initialized) return;
        Initialized = true;
        await InstallServices();
    }

    private static async UniTask InstallServices()
    {
        var gameSetting = new GameSettings();
        // var cameraSetup = new CameraSetup();
        var audioEngine = new AudioEngine();

        Services.Add(gameSetting);
        // Services.Add(cameraSetup);
        Services.Add(audioEngine);

        List<UniTask> initTasks = new();
        foreach (var service in Services)
        {
            initTasks.Add(service.InitializeAsync());
        }

        await UniTask.WhenAll(initTasks);
    }

    public static async UniTask<T> GetServiceAsync<T>()
    {
        await InitializeAsync();
        if (Services.Find(x => x is T) is T service)
        {
            return service;
        }

        Debug.LogError($"Service: [{typeof(T)}] does not exist");
        return default;
    }

    public static T GetService<T>()
    {
        _ = InitializeAsync();
        if (Services.Find(x => x is T) is T service)
        {
            return service;
        }

        Debug.LogError($"Service: [{typeof(T)}] does not exist");
        return default;
    }
}