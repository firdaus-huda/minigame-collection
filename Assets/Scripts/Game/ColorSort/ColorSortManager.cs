using Cysharp.Threading.Tasks;
using PahudProject.Common;
using PahudProject.Enum;

namespace PahudProject.Game.ColorSort
{
    public class ColorSortManager: BaseMiniGameManager
    {
        public override MiniGameType MiniGameType => MiniGameType.ColorSort;
        
        public override UniTask Initialize()
        {
            throw new System.NotImplementedException();
        }
        
        public override void StartGame()
        {
            AudioEngine.PlayBGM(BGMType.ColorSort);
        }

        public override async UniTask Destroy()
        {
            throw new System.NotImplementedException();
        }
    }
}