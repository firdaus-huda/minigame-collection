using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PahudProject.Enum;
using UnityEngine;

namespace PahudProject.Common
{
    public class AudioEngine : MonoBehaviour
    {
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private int sfxPoolSize = 10;

        [SerializeField] private AudioData bgmData;
        [SerializeField] private AudioData sfxData;

        private static AudioEngine _instance;
        private readonly Dictionary<BGMType, AudioClip> _bgmCache = new();
        private readonly Dictionary<SFXType, AudioClip> _sfxCache = new();
        private readonly Queue<AudioSource> _sfxPool = new();

        private float _currentVolume;
        private Sequence _bgmTransition;
        private const float TransitionDuration = 0.25f;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitClipCache();
                InitSfxPool();
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            _currentVolume = GameSettings.GetVolume();
            OnVolumeSettingChanged(_currentVolume);
            GameSettings.VolumeChanged += OnVolumeSettingChanged;
        }

        private void OnDestroy()
        {
            GameSettings.VolumeChanged -= OnVolumeSettingChanged;
        }

        private void OnVolumeSettingChanged(float volume)
        {
            bgmSource.volume = volume;
            _currentVolume = volume;
        }

        private void InitClipCache()
        {
            foreach (var entry in bgmData.entries)
                if (System.Enum.TryParse(entry.name, true, out BGMType type))
                    _bgmCache.TryAdd(type, entry.clip);
            
            foreach (var entry in sfxData.entries)
                if (System.Enum.TryParse(entry.name, true, out SFXType type))
                    _sfxCache.TryAdd(type, entry.clip);
        }

        private void InitSfxPool()
        {
            for (int i = 0; i < sfxPoolSize; i++)
            {
                var source = CreateNewSfxSource();
                source.gameObject.SetActive(false);
                _sfxPool.Enqueue(source);
            }
        }

        private AudioSource CreateNewSfxSource()
        {
            GameObject go = new GameObject("SFXSource") { transform = { parent = transform } };
            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            return source;
        }

        // ---------- BGM ----------
        public static void PlayBGM(BGMType bgm, bool loop = true)
        {
            _instance.InternalPlayBGM(bgm, loop);
        }

        private void InternalPlayBGM(BGMType bgm, bool loop)
        {
            _bgmCache.TryGetValue(bgm, out var clip);
            if (clip == null)
            {
                Debug.LogWarning($"BGM of type [{bgm}] is not available");
                return;
            }

            bgmSource.loop = loop;
            CreateBGMTransition(clip);
        }

        private void CreateBGMTransition(AudioClip target)
        {
            if (bgmSource.clip == target) return;
            _bgmTransition?.Kill();
            _bgmTransition = DOTween.Sequence();
            _bgmTransition.Append(bgmSource.DOFade(0f, TransitionDuration));
            _bgmTransition.AppendCallback(() => bgmSource.clip = target);
            _bgmTransition.AppendCallback(() => bgmSource.Play());
            _bgmTransition.Append(bgmSource.DOFade(_currentVolume, TransitionDuration));
            _bgmTransition.Play();
        }

        public static void StopBGM()
        {
            _instance.bgmSource.DOFade(0f, TransitionDuration);
        }

        // ---------- SFX ----------
        public static void PlaySfx(SFXType sfx)
        {
            _instance.InternalPlaySfx(sfx);
        }

        private void InternalPlaySfx(SFXType sfx)
        {
            _sfxCache.TryGetValue(sfx, out var clip);
            if (clip == null) return;

            var source = GetSfxSource();
            source.clip = clip;
            source.volume = _currentVolume;
            source.gameObject.SetActive(true);
            source.PlayOneShot(clip);

            ReleaseAfterPlay(source).Forget();
            
            async UniTask ReleaseAfterPlay(AudioSource audioSource)
            {
                await UniTask.WaitUntil(() => !audioSource.isPlaying);
                audioSource.gameObject.SetActive(false);
                _sfxPool.Enqueue(audioSource);
            }
        }

        private AudioSource GetSfxSource()
        {
            if (_sfxPool.Count > 0) return _sfxPool.Dequeue();
            return CreateNewSfxSource();
        }

        
    }
}