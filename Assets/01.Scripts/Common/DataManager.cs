using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ETileType
{
    START,
    END,
    NORMAL,
    BLOCK,
    SLOW,
    TRAP,
    OVERLAP,
    VERTICAL,
    HORIZONTAL,
    CURVE,
    LAST
}
public enum EDir
{
    NONE,
    UP,
    DOWN,
    LEFT,
    RIGHT
}
public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    MyTile[,] _tiles;
    //맵 크기
    int _mapSize;
    public int MapSize
    {
        get
        {
            return _mapSize;
        }
    }
    //최소 맵 크기
    float _gridSize;
    public float GridSize
    {
        get
        {
            return _gridSize;
        }
    }
    List<Dictionary<string, object>> _tileData;
    public List<Dictionary<string, object>> TileData
    {
        get
        {
            return _tileData;
        }
    }
    int _stageLevel = 1;
    public int StageLevel
    {
        get
        {
            return _stageLevel;
        }
        set
        {
            _stageLevel = value;
        }
    }
    public MyTile[,] Tiles
    {
        get
        {
            return _tiles;
        }
    }
    int _startTileNum;
    public int StartTileNum
    {
        get
        {
            return _startTileNum;
        }
    }
    Vector3Int[] _startTilePos;
    public Vector3Int[] StartTilePos
    {
        get
        {
            return _startTilePos;
        }
    }
    private void Awake()
    {
        //instance가 null이면
        if (instance == null)
        {
            //instance는 자기자신으로
            instance = this;
        }
        else if (instance != null)
        {
            //instance를 삭제
            Destroy(gameObject);
        }
        _tileData = CSVReader.Read("tile");
        Init();
    }
    public void Init()
    {
        _mapSize = (int)_tileData[_stageLevel - 1]["mapSize"];
        _gridSize = 8.8f / _mapSize;
        _startTileNum = 2;
        _tiles = new MyTile[_mapSize, _mapSize];
        for(int i = 0; i < _mapSize; i++)
        {
            for(int j = 0; j < _mapSize; j++)
            {
                _tiles[i, j] = GetComponent<MyTile>();
            }
        }
        _startTilePos = new Vector3Int[_startTileNum];
        int dataIdx = 0;
        while ((int)_tileData[dataIdx]["stage"] == _stageLevel)
        {
            int x = (int)_tileData[dataIdx]["tileX"];
            int y = (int)_tileData[dataIdx]["tileY"];
            Debug.Log((ETileType)(int)_tileData[dataIdx]["tileType"]);
            _tiles[x, y].tilePos = new Vector3Int(x, y, 0);
            _tiles[x, y].type = (ETileType)(int)_tileData[dataIdx]["tileType"];
            dataIdx++;
        }
        FindTile();

    }
    public void FindTile()
    {
        int i;
        int j;
        int dataIdx = 0;
        for (i = 0; i < MapSize; i++)
        {
            for (j = 0; j < MapSize; j++)
            {
                if (Tiles[i, j].type == ETileType.START)
                    _startTilePos[dataIdx++] = Tiles[i,j].tilePos;
            }
        }
    }
}
