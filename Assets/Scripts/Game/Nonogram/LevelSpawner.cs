using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PahudProject.Game.Nonogram
{
    public class LevelSpawner: MonoBehaviour
    {
        [SerializeField] private GameObject emptyTilePrefab;
        [SerializeField] private TileButtonController tilePrefab;
        [SerializeField] private TileInfo topInfoTilePrefab;
        [SerializeField] private TileInfo leftInfoTilePrefab;
        [SerializeField] private Transform tileParent;
        [SerializeField] private Transform tilePoolParent;
        [SerializeField] private GridLayoutGroup parentGrid;

        private readonly List<TileButtonController> _tilePool = new();
        private const int InitialPoolCount = 120;
        private LevelData _levelData;
        private readonly List<TileButtonController> _spawnedTiles = new();
        private readonly List<TileInfo> _spawnedTileInfo = new();
        private GameObject _spawnedEmptyTile;

        public void Initialize()
        {
            for (int i = 0; i < InitialPoolCount; i++)
            {
                var tile = Instantiate(tilePrefab, tilePoolParent);
                tile.gameObject.SetActive(false);
                _tilePool.Add(tile);
            }
        }
        
        public TileButtonController[] SpawnLevel(LevelDetail levelDetail)
        {
            var tileInfo = levelDetail.tiles;
            var columnCount = levelDetail.columnCount;
            var rowCount = tileInfo.Count / columnCount;

            int currentColumn = 0;
            int currentRow = 0;

            SpawnTiles();
            SetColumnInfoText();
            SetRowInfoText();

            return _spawnedTiles.ToArray();

            void SpawnTiles()
            {
                parentGrid.constraintCount = columnCount + 1;
                _spawnedEmptyTile = Instantiate(emptyTilePrefab, tileParent);
                
                for (int i = 0; i < columnCount; i++)
                {
                    var tile = Instantiate(topInfoTilePrefab, tileParent);
                    _spawnedTileInfo.Add(tile);
                }
                
                for (int i = 0; i < tileInfo.Count; i++)
                {
                    if (i % columnCount == 0)
                    {
                        var infoTile = Instantiate(leftInfoTilePrefab, tileParent);
                        _spawnedTileInfo.Add(infoTile);
                        currentColumn = 0;
                        if (i > 0) currentRow++;
                    }
                    
                    var tile = _tilePool[i];
                    tile.SetData(tileInfo[i], currentColumn, currentRow);
                    tile.transform.SetParent(tileParent);
                    tile.gameObject.SetActive(true);
                    tile.SetEnableInput(true);
                    _spawnedTiles.Add(tile);
                    currentColumn++;
                }
            }

            void SetColumnInfoText()
            {
                var info = _spawnedTileInfo.Where(x => x.type == TileInfoType.Top).ToList();

                for (int i = 0; i < columnCount; i++)
                {
                    var tileColumn = _spawnedTiles.Where(x => x.Column == i).ToList();

                    int count = 0;
                    string infoText = "";
                    for (int j = 0; j < tileColumn.Count; j++)
                    {
                        if (string.IsNullOrEmpty(tileColumn[j].TileColor))
                        {
                            if (count == 0) continue;
                            infoText = $"{infoText}\n{count}";
                            count = 0;
                        }
                        else
                        {
                            count++;
                            if (j == tileColumn.Count - 1) infoText = $"{infoText}\n{count}";
                        }
                    }

                    if (string.IsNullOrEmpty(infoText)) infoText = "0";
                    info[i].SetData(infoText);
                }
            }

            void SetRowInfoText()
            {
                var info = _spawnedTileInfo.Where(x => x.type == TileInfoType.Left).ToList();

                for (int i = 0; i < rowCount; i++)
                {
                    var tileRow = _spawnedTiles.Where(x => x.Row == i).ToList();

                    int count = 0;
                    string infoText = "";
                    for (int j = 0; j < tileRow.Count; j++)
                    {
                        if (string.IsNullOrEmpty(tileRow[j].TileColor))
                        {
                            if (count == 0) continue;
                            infoText = $"{infoText} {count}";
                            count = 0;
                        }
                        else
                        {
                            count++;
                            if (j == tileRow.Count - 1) infoText = $"{infoText} {count}";
                        }
                    }
                    
                    if (string.IsNullOrEmpty(infoText)) infoText = "0";
                    info[i].SetData(infoText);
                }
            }
        }

        public void ClearLevel()
        {
            foreach (var tile in _spawnedTiles)
            {
                tile.ResetData();
                tile.gameObject.SetActive(false);
                tile.transform.SetParent(tilePoolParent);
            }

            foreach (var info in _spawnedTileInfo)
            {
                Destroy(info.gameObject);
            }
            
            Destroy(_spawnedEmptyTile);
            _spawnedTiles.Clear();
            _spawnedTileInfo.Clear();
            _spawnedEmptyTile = null;
        }
    }
}