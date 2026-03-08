using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TapEffect: MonoBehaviour
    {
        [SerializeField] private ParticleSystem tapEffectPrefab;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Transform parent;

        private List<RectTransform> _effectPool = new();
        private const int InitialPoolCount = 5;
        private float _effectDuration;
        private void Awake()
        {
            for (int i = 0; i < InitialPoolCount; i++)
            {
                var effect = Instantiate(tapEffectPrefab, parent);
                effect.gameObject.SetActive(false);
                _effectPool.Add(effect.GetComponent<RectTransform>());
            }

            _effectDuration = tapEffectPrefab.main.duration; 
        }

        private void Update()
        {
            if (DetectMouseClick()) 
                _ = ShowTapEffect();
        }

        private bool DetectMouseClick()
        {
            return Mouse.current.leftButton.wasPressedThisFrame;
        }

        private async UniTask ShowTapEffect()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, Mouse.current.position.ReadValue(),
                canvas.worldCamera, out Vector2 spawnPos);

            foreach (var effect in _effectPool)
            {
                if (!effect.gameObject.activeSelf)
                {
                    effect.anchoredPosition = spawnPos;
                    effect.gameObject.SetActive(true);
                    await Delay.Seconds(_effectDuration);
                    effect.gameObject.SetActive(false);
                    return;
                }
            }
            
            var newEffect = Instantiate(tapEffectPrefab, parent);
            var effectRect = newEffect.GetComponent<RectTransform>();
            _effectPool.Add(effectRect);
            effectRect.anchoredPosition = spawnPos;
            await Delay.Seconds(_effectDuration);
            effectRect.gameObject.SetActive(false);
        }
    }