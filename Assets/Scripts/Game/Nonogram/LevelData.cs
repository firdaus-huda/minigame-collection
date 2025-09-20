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
        public int Level;
        public int ColumnCount;
        public List<string> Tiles = new();
    }
}