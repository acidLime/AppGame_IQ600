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
    NORMAL2,
   
}
public enum ERoad
{
    VERTICAL,
    HORIZONTAL,
    CURVE,
    LAST,
    OVERLAP
}

public enum EDir
{
    NONE,
    UP,
    DOWN,
    LEFT,
    RIGHT
}
public struct MyTile
{
    public Vector3Int tilePos;
    public EDir dir;
    public EDir prevDir;

    public ETileType type;
    public bool canDraw;
    public bool dontDestroy;
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
    List<Dictionary<string, object>> _missionData;
    public List<Dictionary<string, object>> MissionData
    {
        get
        {
            return _missionData;
        }
    }
    List<Dictionary<string, object>> _characterData;
    public List<Dictionary<string, object>> CharacterData
    {
        get
        {
            return _characterData;
        }
    }
    int _stageLevel;
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
    public Vector3Int _trapTilePos;
    public int TrapTileNum;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        _tileData = CSVReader.Read("tile");
        _missionData = CSVReader.Read("Mission");
        _characterData = CSVReader.Read("StartTime");
    }
    public void Init()
    {
        _stageLevel = LevelManager.instance.StageLevel;
        _mapSize = (int)_tileData[_stageLevel - 1]["mapSize"];
        _gridSize = 8.8f / _mapSize;
        _startTileNum = (int)_characterData[_stageLevel-1]["characterNum"];
        _tiles = new MyTile[_mapSize, _mapSize];
        for(int i = 0; i < _mapSize; i++)
        {
            for(int j = 0; j < _mapSize; j++)
            {
                _tiles[i, j] = new MyTile();
            }
        }
        _startTilePos = new Vector3Int[_startTileNum];
        int dataIdx = 0;
        int x = 0;
        int y = 0;
        while ((int)_tileData[dataIdx]["stage"] != 0)
        {
            if((int)_tileData[dataIdx]["stage"] == StageLevel)
            {
                x = (int)_tileData[dataIdx]["tileX"];
                y = (int)_tileData[dataIdx]["tileY"];

                _tiles[x, y].tilePos = new Vector3Int(x, y, 0);
                _tiles[x, y].type = (ETileType)(int)_tileData[dataIdx]["tileType"];
                _tiles[x, y].canDraw = true;
                _tiles[x, y].dontDestroy = false;
            }
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
                {
                    _startTilePos[dataIdx++] = Tiles[i, j].tilePos;
                }
                if (Tiles[i, j].type == ETileType.TRAP)
                {
                    _trapTilePos = Tiles[i, j].tilePos;
                    TrapTileNum++;
                }
            }
        }
    }
}
