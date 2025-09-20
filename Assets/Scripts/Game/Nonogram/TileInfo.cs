using TMPro;
using UnityEngine;

namespace PahudProject.Game.Nonogram
{
    public class TileInfo: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI infoText;
        public TileInfoType type;
        
        public void SetData(string info)
        {
            infoText.text = info;
        }
    }

    public enum TileInfoType
    {
        Top,
        Left
    }
}