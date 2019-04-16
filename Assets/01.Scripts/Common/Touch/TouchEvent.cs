using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TouchEvent : MonoBehaviour
{

    UIManager UI;
    MapManager MM;
    DataManager DM;
    bool _isStart = false;
    bool _canDoubleTouch = false;
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
    Vector3Int _prevTouchPos;
    public Vector3Int PrevTouchPos
    {
        get
        {
            return _prevTouchPos;
        }
        set
        {
            _prevTouchPos = value;
        }
    }
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
    public bool CanDoubleTouch
    {
        get
        {
            return _canDoubleTouch;
        }
        set
        {
            _canDoubleTouch = value;
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
        DM = DataManager.instance;
        _trackList = new List<Stack<Vector3Int>>();
        _startTileNum = DM.StartTileNum;

        _startTilePos = new Vector3Int[_startTileNum];
        _startTilePos = DM.StartTilePos;

        _canDraw = new bool[DM.MapSize, DM.MapSize];
        _canDraw = MM.CanDraw;

        mapSize = DM.MapSize;

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
        if (_trackList[_curTrack].Peek() == _startTilePos[_curTrack])
            return _trackList[_curTrack].Peek();
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
            for (i = 0; i < DM.BlockTileNum; i++)
            {
                if (DM.BlockTilePos[i] == tilePos)
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
        
        if (tileDir.x > 0)
        {
            MM.SetTileIdx((int)ETileType.HORIZONTAL);
            //위아래 플립
            _curDir = EDir.RIGHT;
        }
        if (tileDir.x < 0)
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
        if(_prevDir != _curDir)
        {
            MM.SetCurveTile(prevTile, _curDir);
        }
        _prevDir = _curDir;
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
        MM.ChangeTile(tilePos, MM.TileIdx); //좌표 타일을 지정된 타일로 변경
        //그릴 수 없는 타일로 변경
        Color color = new Color(1.0f, 0.1f, 0.1f, 0.3f);
        tilemap.SetColor(tilePos, color);
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
        MM.ChangeTile(TrackList[_curTrack].Peek(), MM.TileIdx);
    }
    //인자로 받은 위치까지 타일 삭제
    public void RemoveTile(Vector3Int removePos)
    {
        /*
        if(removePos == _startTilePos[_curTrack])
        {
            while(_trackList[_curTrack].Count > 1 )
                MM.ChangeTile(_trackList[_curTrack].Pop(), ETileType.NORMAL);
            CharacterCtrl.instance.CharacterMoveTile[_curTrack].Clear();
            CharacterCtrl.instance.CharacterMoveTile[_curTrack].Add(tilemap.CellToWorld(DM.StartTilePos[_curTrack]));
            return;
        }
        */
        Vector3Int topTilePos;
        int tileIdx = CharacterCtrl.instance.CharacterMoveTile[_curTrack].Count - 1;
        do
        {

            if (_trackList[_curTrack].Peek() == _startTilePos[_curTrack])
                break;
            topTilePos = _trackList[_curTrack].Pop();
            MM.ChangeTile(topTilePos, ETileType.NORMAL);
            _canDraw[topTilePos.x, topTilePos.y] = true;
            CharacterCtrl.instance.CharacterMoveTile[_curTrack].RemoveAt(tileIdx--);
        }
        while (topTilePos != removePos);
    }
    public void CheckDoubleTouch(Vector3Int tilePos)
    {
        float touchTime = 1.0f;
        while(touchTime > 0)
        {
            touchTime -= Time.deltaTime;
            if (_prevTouchPos == tilePos)
            {
                _canDoubleTouch = true;
                _prevTouchPos = new Vector3Int(-1, -1, 0);

                break;
            }
            _canDoubleTouch = false;
        }
    }
}