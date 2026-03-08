using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;
using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace PahudProject.Game.Nonogram
{
    public class TileButtonController: ButtonController
    {
        public string TileColor;
        public int Column;
        public int Row;
        public TileState TileState { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            TileState = TileState.None;
            if (View is TileButtonView tileView) tileView.UpdateSprite(TileState);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!inputEnabled) return;
            ChangeState(eventData.button);
            base.OnPointerUp(eventData);
        }

        public void SetData(string colorCode, int column, int row)
        {
            TileColor = colorCode;
            Column = column;
            Row = row;
            if (View is TileButtonView tileView) tileView.SetColor(colorCode);
        }

        private void ChangeState(InputButton input)
        {
            switch (TileState)
            {
                case TileState.None:
                    if (input == InputButton.Left) TileState = TileState.Fill;
                    else if (input == InputButton.Right) TileState = TileState.Block;
                    break;
                case TileState.Block:
                    if (input == InputButton.Left) return;
                    if (input == InputButton.Right) TileState = TileState.None;
                    break;
                case TileState.Fill:
                    if (input == InputButton.Left) TileState = TileState.None;
                    else if (input == InputButton.Right) return;
                    break;
            }

            if (View is TileButtonView tileView) tileView.UpdateSprite(TileState);
        }

        public async UniTask RevealColor()
        {
            if (string.IsNullOrEmpty(TileColor)) return;
            if (View is TileButtonView tileView)
            {
                await tileView.RevealColor();
            }
        }

        public void ResetData()
        {
            TileState = TileState.None;
            if (View is TileButtonView tileView)
            {
                tileView.ResetView();
            }
        }
    }
}