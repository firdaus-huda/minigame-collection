using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameSettings : IGameService
{
    private const string VolumePlayerPrefsKey = "Settings.Volume";
        
    public event Action<float> VolumeChanged;

    private readonly Dictionary<string, object> ValueCache = new();
        
    public UniTask InitializeAsync()
    {
        return UniTask.CompletedTask;
    }

    private float GetFloat(string key, float defaultValue = 0f)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetFloat(key, defaultValue);
            ValueCache[key] = defaultValue;
            return defaultValue;
        }
            
        ValueCache.TryGetValue(key, out object value);
        if (value == null) ValueCache[key] = PlayerPrefs.GetFloat(key);
        return (float) ValueCache[key];
    }

    private void SetFloat(string key, float value, bool shouldSave = false)
    {
        ValueCache[key] = value;
        if (shouldSave) PlayerPrefs.SetFloat(key, value);
    }
        
    public float GetVolume()
    {
        return GetFloat(VolumePlayerPrefsKey, 0.5f);
    }

    public void SetVolume(float volume, bool shouldSave = false)
    {
        SetFloat(VolumePlayerPrefsKey, volume, shouldSave);
        VolumeChanged?.Invoke(volume);
    }
}