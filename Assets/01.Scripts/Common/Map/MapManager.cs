using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {

    public Tilemap tilemap;
    public Tilemap backGroundTilemap;
    [SerializeField] TileBase[] tileBase;
    public GameObject grid;
    DataManager DM;

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
        DM = DataManager.instance;
        //_tileData = CSVReader.Read("tile");

        InitMap();

    }
    public void InitMap()
    {
        int mapSize = DM.MapSize;
        //맵 사이즈가 최소맵 사이즈보다 작으면 최소사이즈로 변경
        _canDraw = new bool[mapSize, mapSize];
        for (int i = 0; i < mapSize; i++)
        {
            for(int j = 0; j < mapSize; j++)
            {
                _canDraw[i,j] = true;
            }
        }
        Vector3Int[] StartTilePos = DM.StartTilePos;
        for (int i = 0; i < DM.StartTileNum; i++)
        {
            _canDraw[StartTilePos[i].x, StartTilePos[i].y] = false;
        }
        Vector3Int[] BlockTilePos = DM.BlockTilePos;
        for(int i = 0; i < DM.BlockTileNum; i++)
        {
            _canDraw[BlockTilePos[i].x, BlockTilePos[i].y] = false;
        }
        int dataIdx = 0;
        while ((int)DM.TileData[dataIdx]["stage"] == DM.StageLevel)
        {
            tilemap.SetTile(new Vector3Int((int)DM.TileData[dataIdx]["tileX"],
                (int)DM.TileData[dataIdx]["tileY"], 0),
                tileBase[(int)DM.TileData[dataIdx]["tileType"]]);
            dataIdx++;
        }
        float gridSize = DM.GridSize;
        grid.transform.localScale = new Vector3(gridSize, gridSize, 1); // 타일 갯수에 따른 맵사이즈 조절
        grid.transform.localScale = new Vector3(gridSize, gridSize, 1);
        backGroundTilemap.BoxFill(new Vector3Int(mapSize - 1, mapSize - 1, 0),
            tileBase[(int)ETileType.NORMAL], 0, 0, mapSize, mapSize);//지정범위만큼 타일 채움
        
    }
    //타일을 지정 타일로 변경해줌
    public void ChangeTile(Vector3Int position, ETileType tileType)
    {
        int mapSize = DM.MapSize;

        //맵밖이면 리턴
        if (position.x > mapSize - 1 || position.y > mapSize - 1 ||
            position.x < 0 || position.y < 0)
            return;
        //startTile이면 리턴
        for(int i = 0; i < DM.StartTileNum; i++)
        {
            if (DM.StartTilePos[i] == position)
                return;
        }
        //장애물이면 리턴
        for(int i = 0; i < DM.BlockTileNum; i++)
        {
            if (position == DM.BlockTilePos[i])
                return;
        }
        //도착타일이면 리턴
        if (position == DM.EndTilePos)
            return;
        tilemap.SetTile(position, tileBase[(int)tileType]);
    }
    public void SetTileIdx(int tileIdx)
    {
        _tileIdx = (ETileType)tileIdx;
    }
    public void SetCurveTile(Vector3Int tilePos, EDir tileDir)
    {
        ChangeTile(tilePos, ETileType.CURVE);
        float dir = 0.0f;
        switch (tileDir)
        {
            case EDir.UP:
                dir = 270.0f;
                break;
            case EDir.DOWN:
                dir = 90.0f;
                break;
            case EDir.LEFT:
                dir = 180.0f;
                break;
            case EDir.RIGHT:
                dir = 0.0f;
                break;
        }
        Matrix4x4 matrix = Matrix4x4.Rotate(Quaternion.Euler(0.0f, 0.0f, dir));
        tilemap.SetTransformMatrix(tilePos, matrix);
    }
}
