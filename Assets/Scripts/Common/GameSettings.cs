using System;
using System.Collections.Generic;
using UnityEngine;

namespace PahudProject.Common
{
    public class GameSettings
    {
        private const string VolumePlayerPrefsKey = "Settings.Volume";
        
        public static event Action<float> VolumeChanged;

        private static readonly Dictionary<string, object> ValueCache = new();

        private static float GetFloat(string key, float defaultValue = 0f)
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

        private static void SetFloat(string key, float value, bool shouldSave = false)
        {
            ValueCache[key] = value;
            if (shouldSave) PlayerPrefs.SetFloat(key, value);
        }
        
        public static float GetVolume()
        {
            return GetFloat(VolumePlayerPrefsKey);
        }

        public static void SetVolume(float volume, bool shouldSave = false)
        {
            SetFloat(VolumePlayerPrefsKey, volume, shouldSave);
            VolumeChanged?.Invoke(volume);
        }
    }
}