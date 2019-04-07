using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;


public class MapManager : MonoBehaviour {

    public Tilemap tilemap;
    [SerializeField] TileBase[] tileBase;
    public GameObject grid;

    public static MapManager instance;

    //엑셀 데이터를 파싱해서 저장할 리스트
    List<Dictionary<string, object>> _tileData;
    //맵 크기
    public int mapSize = 0;

    //최소 맵 크기
    int minMapSize = 6;

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

    Tile.ETileType _tileIdx = Tile.ETileType.NORMAL;
    public Tile.ETileType TileIdx
    {
        get
        {
            return _tileIdx;
        }
        set
        {
            _tileIdx = value;
        }
    }

    bool[,] _canDraw;
    public bool[,] IsDraw
    {
        get
        {
            return _canDraw;
        }
    }
    float _gridSize;
    public float GridSize
    {
        get
        {
            return _gridSize;
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

        InitMap();

    }
    // Use this for initialization

    // Update is called once per frame
    public void InitMap()
    {
        int dataIdx = 0;
        _startTileNum = (int)_tileData[_stageLevel -1]["startTileNum"];
        _blockTileNum = (int)_tileData[_stageLevel - 1]["blockTileNum"];
        _slowTileNum = (int)_tileData[_stageLevel - 1]["slowTileNum"];
        _trapTileNum = (int)_tileData[_stageLevel - 1]["trapTileNum"];

        if (mapSize < minMapSize)
            mapSize = minMapSize;
        _canDraw = new bool[mapSize, mapSize];
        for (int i = 0; i < mapSize; i++)
        {
            for(int j = 0; j<mapSize; j++)
            {
                _canDraw[i,j] = true;
            }
        }
        _gridSize = 8.8f / mapSize;
        //num 6 = 1.25, num 7 = 1.07125, num 8 = 0.9375, num 9 = 0.833, num 10 = 0.75 
        //맵 크기 = 64 * 타일 수 * 그리드 사이즈
        grid.transform.localScale = new Vector3(_gridSize, _gridSize, 1); // 타일 갯수에 따른 맵사이즈 조절
        tilemap.BoxFill(new Vector3Int(mapSize - 1, mapSize - 1, 0), tileBase[(int)Tile.ETileType.NORMAL], 0, 0, mapSize, mapSize);//지정범위만큼 타일 채움
        //데이터 추출 후, 타일 세팅
        while((int)_tileData[dataIdx]["stage"] == _stageLevel)
        {
            tilemap.SetTile(new Vector3Int((int)_tileData[dataIdx]["tileX"], (int)_tileData[dataIdx]["tileY"], 0), tileBase[(int)_tileData[dataIdx]["tileType"]]);
            dataIdx++;
        }
         
        FindAndSetSpecialTilePos();
    }
    public void ChangeTile(Vector3Int position, Tile.ETileType tileType)
    {
        //맵밖이면 리턴
        if (position.x > mapSize - 1 || position.y > mapSize - 1 ||
            position.x < 0 || position.y < 0)
            return;
        //startTile이면 리턴
        for(int i = 0; i < _startTileNum; i++)
        {
            if (_startTilePos[i] == position)
                return;
        }
        //장애물이면 리턴
        for(int i = 0; i < _blockTileNum; i++)
        {
            if (position == _blockTilePos[i])
                return;
        }
        //도착타일이면 리턴
        if (position == _endTilePos)
            return;
        tilemap.SetTile(position, tileBase[(int)tileType]);
    }
    public void SetTileIdx(int tileIdx)
    {
        _tileIdx = (Tile.ETileType)tileIdx;
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

        while((int)_tileData[dataIdx]["stage"] == _stageLevel)
        {

            switch ((int)_tileData[dataIdx]["tileType"])
            {
                case (int)Tile.ETileType.START:
                    _startTilePos[startTileNum++] = new Vector3Int((int)_tileData[dataIdx]["tileX"], (int)_tileData[dataIdx]["tileY"], 0);
                    _canDraw[(int)_tileData[dataIdx]["tileX"], (int)_tileData[dataIdx]["tileY"]] = false;
                    break;
                case (int)Tile.ETileType.END:
                    _endTilePos = new Vector3Int((int)_tileData[dataIdx]["tileX"], (int)_tileData[dataIdx]["tileY"], 0);
                    break;
                case (int)Tile.ETileType.BLOCK:
                    _blockTilePos[blockIdx++] = new Vector3Int((int)_tileData[dataIdx]["tileX"], (int)_tileData[dataIdx]["tileY"], 0);
                    _canDraw[(int)_tileData[dataIdx]["tileX"], (int)_tileData[dataIdx]["tileY"]] = false;
                    break;
                case (int)Tile.ETileType.SLOW:
                    _slowTilePos[slowIdx++] = new Vector3Int((int)_tileData[dataIdx]["tileX"], (int)_tileData[dataIdx]["tileY"], 0);
                    break;
                case (int)Tile.ETileType.TRAP:
                    _trapTilePos[trapIdx++] = new Vector3Int((int)_tileData[dataIdx]["tileX"], (int)_tileData[dataIdx]["tileY"], 0);
                    break;
                default:
                    break;
            }
            dataIdx++;
        }
    }
}
