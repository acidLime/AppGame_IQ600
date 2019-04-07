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
    VERTICAL,
    HORIZONTAL,
    CURVE,
    LAST
}
public class DataManager : MonoBehaviour
{
    public static DataManager instance;
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

    Vector3Int[] _startTilePos;
    public Vector3Int[] StartTilePos
    {
        get
        {
            return _startTilePos;
        }
    }

    int _startTileNum = 0;
    public int StartTileNum
    {
        get
        {
            return _startTileNum;
        }
    }

    Vector3Int _endTilePos;
    public Vector3Int EndTilePos
    {
        get
        {
            return _endTilePos;
        }
    }

    Vector3Int[] _blockTilePos;
    public Vector3Int[] BlockTilePos
    {
        get
        {
            return _blockTilePos;
        }
    }

    int _blockTileNum = 0;
    public int BlockTileNum
    {
        get
        {
            return _blockTileNum;
        }
    }

    Vector3Int[] _slowTilePos;
    public Vector3Int[] SlowTilePos
    {
        get
        {
            return _slowTilePos;
        }
    }

    int _slowTileNum = 0;
    public int SlowTileNum
    {
        get
        {
            return _slowTileNum;
        }
    }

    Vector3Int[] _trapTilePos;
    public Vector3Int[] TrapTilePos
    {
        get
        {
            return _trapTilePos;
        }
    }

    int _trapTileNum = 0;
    public int TrapTileNum
    {
        get
        {
            return _trapTileNum;
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
    void Init()
    {
        _mapSize = (int)_tileData[_stageLevel - 1]["mapSize"];
        Debug.Log(_mapSize);
        _gridSize = 8.8f / _mapSize;

        _startTileNum = (int)_tileData[_stageLevel - 1]["startTileNum"];
        _blockTileNum = (int)_tileData[_stageLevel - 1]["blockTileNum"];
        _slowTileNum = (int)_tileData[_stageLevel - 1]["slowTileNum"];
        _trapTileNum = (int)_tileData[_stageLevel - 1]["trapTileNum"];

        //특수타일들의 위치를 탐색함
        FindAndSetSpecialTilePos();
    }
    //특수한 타일들의 위치값을 저장
    void FindAndSetSpecialTilePos()
    {
        _startTilePos = new Vector3Int[_startTileNum];
        _blockTilePos = new Vector3Int[_blockTileNum];
        _slowTilePos = new Vector3Int[_slowTileNum];
        _trapTilePos = new Vector3Int[_trapTileNum];

        int dataIdx = 0;
        int startTileNum = 0;
        int blockIdx = 0;
        int slowIdx = 0;
        int trapIdx = 0;

        while ((int)_tileData[dataIdx]["stage"] == _stageLevel)
        {
            switch ((int)_tileData[dataIdx]["tileType"])
            {
                case (int)ETileType.START:
                    _startTilePos[startTileNum++] = new Vector3Int((int)_tileData[dataIdx]["tileX"], (int)_tileData[dataIdx]["tileY"], 0);
                    break;
                case (int)ETileType.END:
                    _endTilePos = new Vector3Int((int)_tileData[dataIdx]["tileX"], (int)_tileData[dataIdx]["tileY"], 0);
                    break;
                case (int)ETileType.BLOCK:
                    _blockTilePos[blockIdx++] = new Vector3Int((int)_tileData[dataIdx]["tileX"], (int)_tileData[dataIdx]["tileY"], 0);
                    break;
                case (int)ETileType.SLOW:
                    _slowTilePos[slowIdx++] = new Vector3Int((int)_tileData[dataIdx]["tileX"], (int)_tileData[dataIdx]["tileY"], 0);
                    break;
                case (int)ETileType.TRAP:
                    _trapTilePos[trapIdx++] = new Vector3Int((int)_tileData[dataIdx]["tileX"], (int)_tileData[dataIdx]["tileY"], 0);
                    break;
                default:
                    break;
            }
            dataIdx++;
        }
    }
}
