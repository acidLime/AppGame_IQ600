using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TouchEvent : MonoBehaviour
{

    UIManager UI;
    MapManager MM;
    bool _isStart = false;
    public Tilemap tilemap;
    [SerializeField] private Camera _cam; //터치카메라
    EDir _prevDir;
    EDir _curDir;
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
        UI = UIManager.instance;
        MM = MapManager.instance;
        _trackList = new List<Stack<Vector3Int>>();
        _startTileNum = DataManager.instance.StartTileNum;

        _startTilePos = new Vector3Int[_startTileNum];
        _startTilePos = DataManager.instance.StartTilePos;

        _canDraw = new bool[DataManager.instance.MapSize, DataManager.instance.MapSize];
        _canDraw = MapManager.instance.CanDraw;

        mapSize = DataManager.instance.MapSize;

        _curDir = EDir.NONE;
        _prevDir = EDir.NONE;
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
            for (i = 0; i < DataManager.instance.BlockTileNum; i++)
            {
                if (DataManager.instance.BlockTilePos[i] == tilePos)
                    return false;
            }
            Vector3Int stackTop = _trackList[_curTrack].Pop();
            CharacterCtrl.instance.CharacterMoveTile[_curTrack].RemoveAt(CharacterCtrl.instance.CharacterMoveTile[_curTrack].Count - 1);
            MM.ChangeTile(stackTop, ETileType.NORMAL); //좌표 타일을 지정된 타일로 변경
            //그릴 수 있는 타일로 변경
            _canDraw[stackTop.x, stackTop.y] = true;
            return false;
        }

        return true;
    }
    public void TileReset(int lineNum)
    {
        while (_trackList[lineNum].Count > 1)
            MM.ChangeTile(_trackList[lineNum].Pop(), ETileType.NORMAL);
        CharacterCtrl.instance.CharacterMoveTile.Clear();
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
        _prevDir = _curDir;
        if (tileDir.x > 0)
        {
            MM.SetTileIdx((int)ETileType.HORIZONTAL);
            _curDir = EDir.RIGHT;
        }
        if(tileDir.x < 0)
        {
            MM.SetTileIdx((int)ETileType.HORIZONTAL);
            _curDir = EDir.LEFT;
        }
        if (tileDir.y > 0)
        {
            MM.SetTileIdx((int)ETileType.VERTICAL);
            _curDir = EDir.DOWN;
        }
        if (tileDir.y < 0)
        {
            MM.SetTileIdx((int)ETileType.VERTICAL);
            _curDir = EDir.UP;
        }
        MM.SetCurveTile(prevTile, _curDir);
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
        MM.ChangeTile(tilePos, MapManager.instance.TileIdx); //좌표 타일을 지정된 타일로 변경
        //그릴 수 없는 타일로 변경
        _canDraw[tilePos.x, tilePos.y] = false;
        //현재 좌표를 현재 트랙 스택에 푸시
        _trackList[_curTrack].Push(tilePos);
        CharacterCtrl.instance.CharacterMoveTile[_curTrack].Add(tilemap.CellToWorld(tilePos));
        UI.UpdataCharacterInfo(_curTrack, CharacterCtrl.instance.CharacterMoveTile[_curTrack].Count);
    }
    public Vector3Int GetTilePos()
    {
        Vector3 touchPos = tilemap.WorldToCell(_cam.ScreenToWorldPoint(Input.mousePosition)); // 터치된 좌표를 셀의 좌표로 변환하여 저장
        Vector3Int touchPosToTile = new Vector3Int((int)touchPos.x, (int)touchPos.y, 0); //저장된 좌표를 Vector3Int로 변환
        return touchPosToTile;
    }
    public void ChangeLastTile()
    {
        MM.ChangeTile(TrackList[_curTrack].Peek(), ETileType.LAST);
    }
    public void ChangeStartTile()
    {
        MM.ChangeTile(TrackList[_curTrack].Peek(), MapManager.instance.TileIdx);
    }
}