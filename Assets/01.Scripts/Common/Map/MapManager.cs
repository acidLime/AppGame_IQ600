using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {

    public Tilemap tilemap;
    public Tilemap backGroundTilemap;
    public Tilemap ObjectTilemap;
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

    //bool[,] _canDraw;
    //public bool[,] CanDraw
    //{
    //    get
    //    {
    //        return _canDraw;
    //    }
    //}

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

        for (int i = 0; i <mapSize; i++)
        {
            for(int j = 0; j < mapSize; j++)
            {
                //Debug.Log(DM.Tiles[i, j].type);

                if (DM.Tiles[i, j].type == ETileType.START && DM.Tiles[i, j].type == ETileType.BLOCK)
                    DM.Tiles[i, j].canDraw = false;
                if (DM.Tiles[i, j].type == ETileType.NORMAL)
                    continue;
                ObjectTilemap.SetTile(DM.Tiles[i, j].tilePos, tileBase[(int)DM.Tiles[i, j].type]);
            }
        }
        
        float gridSize = DM.GridSize;
        grid.transform.localScale = new Vector3(gridSize, gridSize, 1); // 타일 갯수에 따른 맵사이즈 조절
        grid.transform.localScale = new Vector3(gridSize, gridSize, 1);
        backGroundTilemap.BoxFill(new Vector3Int(mapSize - 1, mapSize - 1, 0),
            tileBase[(int)ETileType.NORMAL], 0, 0, mapSize, mapSize);//지정범위만큼 타일 채움
        for(int i = 0; i < mapSize; i++)
        {
            for(int j = 0; j < mapSize; j++)
            {
                if(i % 2 == 1)
                {
                    if(j % 2 == 1)
                        backGroundTilemap.SetTile(new Vector3Int(i, j, 0), tileBase[6]);
                }
                if (i % 2 == 0)
                {
                    if (j % 2 == 0)
                        backGroundTilemap.SetTile(new Vector3Int(i, j, 0), tileBase[6]);
                }
            }
        }
        
    }
    //타일을 지정 타일로 변경해줌
    public void ChangeTile(Vector3Int position, ETileType tileType)
    {
        int mapSize = DM.MapSize;

        //맵밖이면 리턴
        if (position.x > mapSize - 1 || position.y > mapSize - 1 ||
            position.x < 0 || position.y < 0)
            return;

        //장애물이면 리턴
        if (DM.Tiles[position.x, position.y].type == ETileType.BLOCK ||
            DM.Tiles[position.x, position.y].type == ETileType.END)
            return;
        tilemap.SetTile(position, tileBase[(int)tileType]);
    }
    public void SetTileIdx(int tileIdx)
    {
        _tileIdx = (ETileType)tileIdx;
    }
    public void SetCurveTile(Vector3Int prevTilePos, EDir prevDir, EDir curDir)
    {
        ChangeTile(prevTilePos, ETileType.CURVE);
        float dir = 0.0f;
        switch (curDir) 
        { 
            case EDir.UP:
                if (prevDir == EDir.RIGHT)
                {
                    dir = 180.0f;
                    Debug.Log("Up Right");
                }
                else if(prevDir == EDir.LEFT)
                {
                    dir = -90.0f;
                    Debug.Log("Up left");
                }
                break;
            case EDir.DOWN:
                if (prevDir == EDir.RIGHT)
                {
                    dir = -90.0f;
                    Debug.Log("down Right");
                }
                else if(prevDir == EDir.LEFT)
                {
                    dir = 0.0f;
                    Debug.Log("down left");
                }
                break; 
            case EDir.RIGHT:
                if (prevDir == EDir.UP)
                {
                    dir = 0.0f;
                    Debug.Log("right up");
                }
                else if(prevDir == EDir.DOWN)
                {
                    dir = 90.0f;
                    Debug.Log("right Down");
                }
                break; 
            case EDir.LEFT:
                if (prevDir == EDir.UP)
                {
                    dir = 270.0f;
                    Debug.Log("Left up");
                }
                else if(prevDir == EDir.DOWN)
                {
                    dir = 180.0f;
                    Debug.Log("Left Down");
                }
                break; 
        } 
        Matrix4x4 matrix = Matrix4x4.Rotate(Quaternion.Euler(0.0f, 0.0f, dir)); 
        tilemap.SetTransformMatrix(prevTilePos, matrix); 

    }
}
