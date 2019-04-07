using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {

    public Tilemap tilemap;
    public Tilemap backGroundGrid;
    [SerializeField] TileBase[] tileBase;
    public GameObject grid;

    public static MapManager instance;
    
    ETileType _tileIdx = ETileType.NORMAL;
    public ETileType TileIdx
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
    public bool[,] CanDraw
    {
        get
        {
            return _canDraw;
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
        //_tileData = CSVReader.Read("tile");

        InitMap();

    }
    // Use this for initialization

    // Update is called once per frame
    public void InitMap()
    {
        int mapSize = DataManager.instance.MapSize;
        //맵 사이즈가 최소맵 사이즈보다 작으면 최소사이즈로 변경
        _canDraw = new bool[mapSize, mapSize];
        for (int i = 0; i < mapSize; i++)
        {
            for(int j = 0; j < mapSize; j++)
            {
                _canDraw[i,j] = true;
            }
        }
        Vector3Int[] StartTilePos = DataManager.instance.StartTilePos;
        for (int i = 0; i < DataManager.instance.StartTileNum; i++)
        {
            _canDraw[StartTilePos[i].x, StartTilePos[i].y] = false;
        }
        Vector3Int[] BlockTilePos = DataManager.instance.BlockTilePos;
        for(int i = 0; i < DataManager.instance.BlockTileNum; i++)
        {
            _canDraw[BlockTilePos[i].x, BlockTilePos[i].y] = false;
        }
        int dataIdx = 0;
        while ((int)DataManager.instance.TileData[dataIdx]["stage"] == DataManager.instance.StageLevel)
        {
            tilemap.SetTile(new Vector3Int((int)DataManager.instance.TileData[dataIdx]["tileX"],
                (int)DataManager.instance.TileData[dataIdx]["tileY"], 0),
                tileBase[(int)DataManager.instance.TileData[dataIdx]["tileType"]]);
            dataIdx++;
        }
        float gridSize = DataManager.instance.GridSize;
        Debug.Log(mapSize);
        grid.transform.localScale = new Vector3(gridSize, gridSize, 1); // 타일 갯수에 따른 맵사이즈 조절
        backGroundGrid.BoxFill(new Vector3Int(mapSize - 1, mapSize - 1, 0),
            tileBase[(int)ETileType.NORMAL], 0, 0, mapSize, mapSize);//지정범위만큼 타일 채움
    }
    //타일을 지정 타일로 변경해줌
    public void ChangeTile(Vector3Int position, ETileType tileType)
    {
        int mapSize = DataManager.instance.MapSize;

        //맵밖이면 리턴
        if (position.x > mapSize - 1 || position.y > mapSize - 1 ||
            position.x < 0 || position.y < 0)
            return;
        //startTile이면 리턴
        for(int i = 0; i < DataManager.instance.StartTileNum; i++)
        {
            if (DataManager.instance.StartTilePos[i] == position)
                return;
        }
        //장애물이면 리턴
        for(int i = 0; i < DataManager.instance.BlockTileNum; i++)
        {
            if (position == DataManager.instance.BlockTilePos[i])
                return;
        }
        //도착타일이면 리턴
        if (position == DataManager.instance.EndTilePos)
            return;
        tilemap.SetTile(position, tileBase[(int)tileType]);
    }
    public void SetTileIdx(int tileIdx)
    {
        _tileIdx = (ETileType)tileIdx;
    }
}
