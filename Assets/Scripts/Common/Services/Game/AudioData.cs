using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/AudioData")]
public class AudioData : ScriptableObject
{
    public List<AudioEntry> entries;
}

[Serializable]
public class AudioEntry
{
    public string name;
    public AudioClip clip;
}