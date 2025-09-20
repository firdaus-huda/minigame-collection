using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PahudProject.Common;
using PahudProject.Enum;
using PahudProject.UI.Input;
using UnityEngine;
using UnityEngine.UI;

namespace PahudProject.Game.Nonogram
{
    public class TileButtonView: ButtonView
    {
        [SerializeField] private GameObject blockTile;
        [SerializeField] private GameObject fillTile;
        [SerializeField] private Image colorTile;

        private TileState _currentState;
        private const float RevealDuration = 0.5f;

        public override void OnPointerDown() { }

        public override void OnPointerUp()
        {
            switch (_currentState)
            {
                case TileState.None:
                    AudioEngine.PlaySfx(SFXType.NonogramTileButtonClear);
                    break;
                case TileState.Block:
                    AudioEngine.PlaySfx(SFXType.NonogramTileButtonBlock);
                    break;
                case TileState.Fill:
                    AudioEngine.PlaySfx(SFXType.NonogramTileButtonFill);
                    break;
            }
        }

        public void SetColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return;
            if (ColorUtility.TryParseHtmlString(color, out Color colorOutput))
            {
                colorTile.color = new Color(colorOutput.r, colorOutput.g, colorOutput.b, 0f);
            }
        }

        public void UpdateSprite(TileState state)
        {
            switch (state)
            {
                case TileState.None:
                    blockTile.SetActive(false);
                    fillTile.SetActive(false);
                    break;
                case TileState.Block:
                    blockTile.SetActive(true);
                    fillTile.SetActive(false);
                    break;
                case TileState.Fill:
                    blockTile.SetActive(false);
                    fillTile.SetActive(true);
                    break;
            }

            _currentState = state;
        }

        public async UniTask RevealColor()
        {
            colorTile.gameObject.SetActive(true);
            colorTile.DOFade(1f, RevealDuration);
        }

        public void ResetView()
        {
            colorTile.gameObject.SetActive(false);
            blockTile.gameObject.SetActive(false);
            fillTile.gameObject.SetActive(false);
        }
    }
}