using System;
using System.Collections.Generic;

namespace PahudProject.Game.Nonogram
{
    public class LevelData
    {
        public List<LevelDetail> Levels = new();
    }

    [Serializable]
    public class LevelDetail
    {
        public int level;
        public int columnCount;
        public List<string> tiles = new();
    }
}