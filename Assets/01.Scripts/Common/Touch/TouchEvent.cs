using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TouchEvent : MonoBehaviour {

    bool _isReset = false;
    bool _isStart = false;
    public Tilemap tilemap;
    [SerializeField] private Camera _cam; //터치카메라

    public bool IsStart
    {
        get
        {
            return _isStart;
        }
        set
        {
            _isStart = value;
        }
    }
    bool _isMove = false;
    bool[,] _canDraw;
    int mapSize = 0;
    List<Stack<Vector3Int>> _trackList;
    public List<Stack<Vector3Int>> TrackList
    {
        get
        {
            return _trackList;
        }
    }
    int _curTrack = 0;
    public int CurTrack
    {
        get
        {
            return _curTrack;
        }
    }

    Vector3Int[] _startTilePos;
    Vector3Int _prevTilePos;
    public Vector3Int PrevTilePos
    {
        get
        {
            return _prevTilePos;
        }
        set
        {
            _prevTilePos = value;
        }
    }

    int _startTileNum = 0;
    private void Start()
    {
        Init();
    }
    public void Init()
    {
        _trackList = new List<Stack<Vector3Int>>();
        _startTileNum = MapManager.instance.StartTileNum;

        _startTilePos = new Vector3Int[_startTileNum];
        _startTilePos = MapManager.instance.StartTilePos;

        _canDraw = new bool[MapManager.instance.mapSize, MapManager.instance.mapSize];
        _canDraw = MapManager.instance.IsDraw;

        mapSize = MapManager.instance.mapSize;

        for (int i = 0; i < _startTileNum; i++)
        {
            _trackList.Add(new Stack<Vector3Int>());
            _trackList[i].Push(_startTilePos[i]);
        }
    }
    public Vector3Int GetPrevTilePos()
    {
        Vector3Int top = _trackList[_curTrack].Pop();
        Vector3Int value;

        if (_trackList[_curTrack].Count == 0)
            value = _startTilePos[_curTrack];
        else
            value = _trackList[_curTrack].Peek();


        _trackList[_curTrack].Push(top);
        return value;
    }
    public bool CanTouchProc(Vector3Int tilePos)
    {
        //시작타일이 아니라면 리턴
        if (!_isStart)
            return false;
        if (tilePos == _startTilePos[_curTrack]) return false;

        if (tilePos.x >= mapSize || tilePos.y >= mapSize ||
            tilePos.x < 0 || tilePos.y < 0)
        {
            return false;
        }

        if (!_canDraw[tilePos.x, tilePos.y])
        {
            if (GetPrevTilePos() != tilePos)
                return false;

            int i = 0;
            //장애물이라면 리턴
            for (i = 0; i < MapManager.instance.BlockTileNum; i++)
            {
                if (MapManager.instance.BlockTilePos[i] == tilePos)
                    return false;
            }
            Vector3Int stackTop = _trackList[_curTrack].Pop();
            MapManager.instance.ChangeTile(stackTop, Tile.ETileType.NORMAL); //좌표 타일을 지정된 타일로 변경
                                                                             //그릴 수 있는 타일로 변경
            _canDraw[stackTop.x, stackTop.y] = true;
            return false;
        }

        return true;
    }
    public void TileReset(int lineNum)
    {
        while (_trackList[lineNum].Count > 1)
            MapManager.instance.ChangeTile(_trackList[lineNum].Pop(), Tile.ETileType.NORMAL);
    }
    public bool IsStartTile(Vector3Int tilePos)
    {
        for (int i = 0; i < _startTileNum; i++)
        {
            //타일 좌표가 마지막 타일과 같다면
            if (tilePos == _trackList[i].Peek())
            {

                _curTrack = i;
                return true;
            }
        }
        return false;
    }
    //방향에 따른 타일설정
    void SetDirection(Vector3Int prevTile, Vector3Int tilePos)
    {
        Vector3Int tileDir;
        tileDir = tilePos - prevTile;
        if (tileDir.x != 0)
            MapManager.instance.SetTileIdx((int)Tile.ETileType.HORIZONTAL);
        if (tileDir.y != 0)
            MapManager.instance.SetTileIdx((int)Tile.ETileType.VERTICAL);
    }
    public bool CanTileDraw(Vector3Int tilePos)
    {
        if (Mathf.Abs(_trackList[_curTrack].Peek().x - tilePos.x) +
                Mathf.Abs(_trackList[_curTrack].Peek().y - tilePos.y) != 1)
        {
            return false;
        }

        if (!CanTouchProc(tilePos))
        {
            return false;
        }
        return true;
    }
    public void DrawTile(Vector3Int tilePos)
    {
        //타일의 방향을 설정
        SetDirection(_trackList[_curTrack].Peek(), tilePos);
        //타일을 그림
        MapManager.instance.ChangeTile(tilePos, MapManager.instance.TileIdx); //좌표 타일을 지정된 타일로 변경
        //그릴 수 없는 타일로 변경
        _canDraw[tilePos.x, tilePos.y] = false;
        //현재 좌표를 현재 트랙 스택에 푸시
        _trackList[_curTrack].Push(tilePos);
        CharacterCtrl.instance.CharacterMoveTile[_curTrack].Enqueue(tilemap.CellToWorld(tilePos));
    }
    public Vector3Int GetTilePos()
    {
        Vector3 touchPos = tilemap.WorldToCell(_cam.ScreenToWorldPoint(Input.mousePosition)); // 터치된 좌표를 셀의 좌표로 변환하여 저장
        Vector3Int touchPosToTile = new Vector3Int((int)touchPos.x, (int)touchPos.y, 0); //저장된 좌표를 Vector3Int로 변환
        return touchPosToTile;
    }
}
