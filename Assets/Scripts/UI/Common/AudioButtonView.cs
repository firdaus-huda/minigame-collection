using PahudProject.UI.Input;
using UnityEngine;
using UnityEngine.UI;

namespace PahudProject.UI.Common
{
    public class AudioButtonView : ButtonView
    {
        [SerializeField] private Image image;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite mutedSprite;
        public void UpdateSprite(float volume)
        {
            var newSprite = volume > 0 ? normalSprite : mutedSprite;
            if (image.sprite == newSprite) return;

            image.sprite = newSprite;
        }
    }
}