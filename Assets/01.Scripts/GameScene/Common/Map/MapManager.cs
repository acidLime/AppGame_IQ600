﻿using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {

    public Tilemap tilemap;
    public Tilemap backGroundTilemap;
    public Tilemap ObjectTilemap;
    public GameObject BackGroundImage;
    public Sprite[] BackGroundImageSet;

    public TileBase[] BaseTileSet1;
    public TileBase[] BaseTileSet2;
    public TileBase[] BaseTileSet3;

    public TileBase[] roadTileSet1;
    public TileBase[] roadTileSet2;
    public TileBase[] roadTileSet3;

    TileBase[] otherBase;
    TileBase[] roadTiles;
    public GameObject grid;
    DataManager DM;
    
    public static MapManager instance;
    
    ERoad _tileIdx = ERoad.HORIZONTAL;
    public ERoad TileIdx
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
        DM = DataManager.instance;
    }
    public void InitMap()
    {
        int mapSize = DM.MapSize;
        if(DM.StageLevel > 0 && DM.StageLevel <= 3)
        {
            otherBase = BaseTileSet1;
            roadTiles = roadTileSet1;
            BackGroundImage.GetComponent<SpriteRenderer>().sprite = BackGroundImageSet[0];
        }
        else if (DM.StageLevel > 3 && DM.StageLevel <= 6)
        {
            otherBase = BaseTileSet2;
            roadTiles = roadTileSet2;
            BackGroundImage.GetComponent<SpriteRenderer>().sprite = BackGroundImageSet[1];
        }
        else if (DM.StageLevel > 6 && DM.StageLevel <= 9)
        {
            otherBase = BaseTileSet3;
            roadTiles = roadTileSet3;
            BackGroundImage.GetComponent<SpriteRenderer>().sprite = BackGroundImageSet[2];
        }
        for (int i = 0; i < mapSize; i++)
        {
            for(int j = 0; j < mapSize; j++)
            {

                if (DM.Tiles[i, j].type == ETileType.BLOCK)
                    DM.Tiles[i, j].canDraw = false;

                if (DM.Tiles[i, j].type == ETileType.NORMAL)
                    continue;
                if(DM.Tiles[i, j].type == ETileType.START)
                {
                    DM.Tiles[i, j].canDraw = false;
                    DM.Tiles[i, j].dir = EDir.DOWN;
                    tilemap.SetTile(DM.Tiles[i, j].tilePos, otherBase[(int)DM.Tiles[i, j].type]);
                    continue;
                }
                ObjectTilemap.SetTile(DM.Tiles[i, j].tilePos, otherBase[(int)DM.Tiles[i, j].type]);
            }
        }
        
        float gridSize = DM.GridSize;
        grid.transform.localScale = new Vector3(gridSize, gridSize, 1); // 타일 갯수에 따른 맵사이즈 조절
        grid.transform.localScale = new Vector3(gridSize, gridSize, 1);
        backGroundTilemap.BoxFill(new Vector3Int(mapSize - 1, mapSize - 1, 0),
            otherBase[(int)ETileType.NORMAL], 0, 0, mapSize, mapSize);//지정범위만큼 타일 채움
        SetTileIdx((int)ERoad.VERTICAL);
        for (int i = 0; i < mapSize; i++)
        {
            for(int j = 0; j < mapSize; j++)
            {
                if(i % 2 == 1)
                {
                    if(j % 2 == 1)
                        backGroundTilemap.SetTile(new Vector3Int(i, j, 0), otherBase[7]);
                }
                if (i % 2 == 0)
                {
                    if (j % 2 == 0)
                        backGroundTilemap.SetTile(new Vector3Int(i, j, 0), otherBase[7]);
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
        tilemap.SetTile(position, otherBase[(int)tileType]);
    }
    public void ChangeTile(Vector3Int position, ERoad roadType)
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
        tilemap.SetTile(position, roadTiles[(int)roadType]);
    }
    public void SetTileIdx(int tileIdx)
    {
        _tileIdx = (ERoad)tileIdx;
    }
    public void SetCurveTile(Vector3Int prevTilePos, EDir prevDir, EDir curDir)
    {
        ChangeTile(prevTilePos, ERoad.CURVE);
        float dir = 0.0f;
        if(prevDir == curDir)
        {
            tilemap.SetTileFlags(prevTilePos, TileFlags.None);

            tilemap.SetTile(prevTilePos, roadTiles[(int)_tileIdx]);
            tilemap.SetColor(prevTilePos, new Color(1, 0, 0, 0.3f));
            return;
        }
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
                    dir = 90.0f;
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
        tilemap.SetTileFlags(prevTilePos, TileFlags.None);
        tilemap.SetTransformMatrix(prevTilePos, matrix);
        tilemap.SetColor(prevTilePos, new Color(1, 0, 0, 0.3f));
    }
}
