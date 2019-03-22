using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;


public class MapManager : MonoBehaviour {

    Tile _tile;

    public Tilemap tilemap;
    public TileBase[] tileBase;
    public GameObject grid;

    public static MapManager instance;

    //엑셀 데이터를 파싱해서 저장할 리스트
    List<Dictionary<string, object>> _tileData;
    //타일 개수
    public int tileNum = 0;
    //최소 타일 개수
    int minTileNum = 6;

    int _startTileNum = 0;
    public int StartTileNum
    {
        get
        {
            return _startTileNum;
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
    Vector3Int _endPos;
    public Vector3Int EndPos
    {
        get
        {
            return _endPos;
        }
        set
        {
            _endPos = value;
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

        _tile = GetComponent<Tile>();
        InitMap();

    }
    // Use this for initialization
    void Start()
    {
        _startTilePos = new Vector3Int[3];
        FindAndSetStartTilePos();
    }

    // Update is called once per frame
    void InitMap()
    {
        int dataIdx = 0;

        if (tileNum < minTileNum)
            tileNum = minTileNum;
        //7.5는 화면대비 타일 비율
        //num 6 = 1.25, num 7 = 1.07125, num 8 = 0.9375, num 9 = 0.833, num 10 = 0.75 
        //맵 크기 = 64 * 타일 수 * 그리드 사이즈
        float gridSize = 7.5f / tileNum;
        
        grid.transform.localScale = new Vector3(gridSize, gridSize, 1); // 타일 갯수에 따른 맵사이즈 조절
        tilemap.BoxFill(new Vector3Int(tileNum - 1, tileNum - 1, 0), tileBase[(int)_tile.TileType], 0, 0, tileNum, tileNum);//지정범위만큼 타일 채움
        //데이터 추출 후, 타일 세팅
        while((int)_tileData[dataIdx]["stage"] == _stageLevel)
        {
            tilemap.SetTile(new Vector3Int((int)_tileData[dataIdx]["tileX"], (int)_tileData[dataIdx]["tileY"], 0), tileBase[(int)_tileData[dataIdx]["tileType"]]);
            dataIdx++;
        }
    }
    public void ChangeTile(Vector3Int position, Tile.ETileType tileType)
    {
        //startTile이면 리턴

        //장애물이면 리턴

        //맵밖이면 리턴
        if (position.x > tileNum-1 || position.y > tileNum-1 ||
            position.x < 0 || position.y < 0)
            return;
        tilemap.SetTile(position, tileBase[(int)tileType]);
    }
    public void SetTileIdx(int tileIdx)
    {
        _tileIdx = (Tile.ETileType)tileIdx;
    }
    public Vector3Int[] GetStartPos()
    {
        return _startTilePos;
    }
    void FindAndSetStartTilePos()
    {
        int dataIdx = 0;
        while((int)_tileData[dataIdx]["stage"] == _stageLevel)
        {
            if ((int)_tileData[dataIdx]["tileType"] == (int)Tile.ETileType.START)
            {
                _startTilePos[_startTileNum++] = new Vector3Int((int)_tileData[dataIdx]["tileX"], (int)_tileData[dataIdx]["tileY"], 0);
            }
            dataIdx++;
        }
        
    }
}
