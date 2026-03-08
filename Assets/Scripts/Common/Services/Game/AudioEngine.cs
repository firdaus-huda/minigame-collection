using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class AudioEngine : IGameService
    {
        private AudioData _bgmClips;
        private AudioData _sfxClips;

        private readonly Dictionary<string, AudioClip> _cachedBgmClips = new();
        private readonly Dictionary<string, AudioClip> _cachedSfxClips = new();

        private Transform _audioParent;
        private AudioSource _sfxSource;
        private AudioSource _bgmSource;

        private Tweener _bgmFadeInTweener;
        private GameSettings _gameSettings;

        public UniTask InitializeAsync()
        {
            _bgmClips = Resources.Load<AudioData>("BGMClips");
            _sfxClips = Resources.Load<AudioData>("SFXClips");

            foreach (var bgmClip in _bgmClips.entries)
            {
                _cachedBgmClips[bgmClip.name] = bgmClip.clip;
            }

            foreach (var sfxClip in _sfxClips.entries)
            {
                _cachedSfxClips[sfxClip.name] = sfxClip.clip;
            }

            var audioEngine = new GameObject("AudioEngine");
            Object.DontDestroyOnLoad(audioEngine);
            _audioParent = audioEngine.transform;

            var bgmSource = new GameObject("BGMSource");
            bgmSource.transform.SetParent(_audioParent);
            _bgmSource = bgmSource.AddComponent<AudioSource>();
            
            var sfxSource = new GameObject("SFXSource");
            sfxSource.transform.SetParent(_audioParent);
            _sfxSource = sfxSource.AddComponent<AudioSource>();

            _gameSettings = Service.GetService<GameSettings>();
            if (_gameSettings != null)
            {
                _gameSettings.VolumeChanged += OnVolumeChanged;
                _bgmFadeInTweener = _bgmSource.DOFade(_gameSettings.GetVolume(), 1f).SetAutoKill(false).Pause();
            }
            
            return UniTask.CompletedTask;
        }

        public void PlayBgm(string name)
        {
            if (_bgmSource == null || _bgmClips == null)
            {
                return;
            }
            
            Play();

            void Play()
            {
                if (!_cachedBgmClips.TryGetValue(name, out var clip))
                {
                    Debug.LogWarning($"[{nameof(AudioEngine)}]: BGM {name} was not found.");
                    return;
                }

                if (_bgmSource.clip == clip) return;

                _bgmSource.clip = clip;
                _bgmSource.loop = true;
                _bgmSource.volume = 0f;
                _bgmFadeInTweener.ChangeEndValue(_gameSettings.GetVolume());
                _bgmFadeInTweener?.Restart();
                _bgmSource.Play();
            }
        }

        public void PlaySfx(string name, bool randomizePitch = false)
        {
            if (_sfxSource == null || _sfxClips == null) return;
            PlayOneShot();

            void PlayOneShot()
            {
                if (!_cachedSfxClips.TryGetValue(name, out var clip))
                {
                    Debug.LogWarning($"[{nameof(AudioEngine)}]: SFX {name} was not found.");
                    return;
                }

                _sfxSource.loop = false;
                _sfxSource.volume = 1f;
                _sfxSource.pitch = randomizePitch ? Random.Range(0.9f, 1.1f) : 1f;
                _sfxSource.PlayOneShot(clip);
            }
        }

        private void OnVolumeChanged(float volume)
        {
            if (_bgmSource != null) _bgmSource.volume = volume;
            if (_sfxSource != null) _sfxSource.volume = volume;
        }
    }