﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum ECharacterDir
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    public enum EDirection
    {
        NONE,
    }
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
    }
    //public EDirection Direction
    //{
    //    get
    //    {
    //        return _dir;
    //    }
    //    set
    //    {
    //        _dir = value;
    //    }
    //}
    //public ETileType TileType
    //{
    //    get
    //    {
    //        return _tileType;
    //    }
    //    set
    //    {
    //        _tileType = value;
    //    }
    //}


    //EDirection _dir = EDirection.NONE;//이동 방향
    //ETileType _tileType = ETileType.START; //타일 종류
}