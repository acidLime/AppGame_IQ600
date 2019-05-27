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
    public static TouchEvent instance;
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
    public Queue<Vector3Int> curSlideQueue;
   // bool[,] _canDraw;
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
    private void Awake()
    {
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
    }
    public void Init()
    {
        UI = UIManager.instance;
        MM = MapManager.instance;
        DM = DataManager.instance;
        _trackList = new List<Stack<Vector3Int>>();
        curSlideQueue = new Queue<Vector3Int>();

        mapSize = DM.MapSize;

        for (int i = 0; i < DM.StartTileNum; i++)
        {

            _trackList.Add(new Stack<Vector3Int>());
            _trackList[i].Push(DM.StartTilePos[i]);

        }
    }
    public Vector3Int GetPrevTilePos()
    {
        if (_trackList[_curTrack].Peek() == DM.StartTilePos[_curTrack])
            return _trackList[_curTrack].Peek();
        Vector3Int top = _trackList[_curTrack].Pop();
        Vector3Int value;

        if (_trackList[_curTrack].Count == 0)
            value = DM.StartTilePos[_curTrack];
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
        if (tilePos == DM.StartTilePos[_curTrack]) return false;

        if (tilePos.x >= mapSize || tilePos.y >= mapSize ||
            tilePos.x < 0 || tilePos.y < 0)
        {
            return false;
        }

        if (!DM.Tiles[tilePos.x, tilePos.y].canDraw)
        {
            if (GetPrevTilePos() != tilePos)
                return false;

            //장애물이라면 리턴
            if (DM.Tiles[tilePos.x, tilePos.y].type == ETileType.BLOCK)
                    return false;
            Vector3Int stackTop = _trackList[_curTrack].Pop();
            CharacterCtrl.instance.CharacterMoveTile[_curTrack].RemoveAt(CharacterCtrl.instance.CharacterMoveTile[_curTrack].Count - 1);
            MM.ChangeTile(stackTop, ETileType.NORMAL); //좌표 타일을 지정된 타일로 변경
            //그릴 수 있는 타일로 변경
            DM.Tiles[stackTop.x, stackTop.y].canDraw = true;
            return false;
        }

        return true;
    }
    public bool IsStartTile(Vector3Int tilePos)
    {
        for (int i = 0; i < DM.StartTileNum; i++)
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
    void SetDirection(Vector3Int prevPos, Vector3Int tilePos)
    {
        Vector3Int tileDir;
        tileDir = tilePos - prevPos;
        if (tileDir.x > 0)
        {
            MM.SetTileIdx((int)ERoad.HORIZONTAL);
            DM.Tiles[tilePos.x, tilePos.y].dir = EDir.RIGHT;
        }
        if (tileDir.x < 0)
        {
            MM.SetTileIdx((int)ERoad.HORIZONTAL);
            DM.Tiles[tilePos.x, tilePos.y].dir = EDir.LEFT;
        }
        if (tileDir.y > 0)
        {
            MM.SetTileIdx((int)ERoad.VERTICAL);
            DM.Tiles[tilePos.x, tilePos.y].dir = EDir.UP;
        }
        if (tileDir.y < 0)
        {
            MM.SetTileIdx((int)ERoad.VERTICAL);
            DM.Tiles[tilePos.x, tilePos.y].dir = EDir.DOWN;
        }
        if(DM.Tiles[tilePos.x,tilePos.y].dir != DM.Tiles[prevPos.x, prevPos.y].dir)
        {
            MM.SetCurveTile(prevPos, DM.Tiles[prevPos.x, prevPos.y].dir, DM.Tiles[tilePos.x, tilePos.y].dir);
        }
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
        if (DM.Tiles[_trackList[_curTrack].Peek().x,_trackList[_curTrack].Peek().y].type == ETileType.END)
            return;
        //타일의 방향을 설정
        SetDirection(_trackList[_curTrack].Peek(), tilePos);
        //타일을 그림
        MM.ChangeTile(tilePos, MM.TileIdx); //좌표 타일을 지정된 타일로 변경
        MM.tilemap.SetTileFlags(tilePos, TileFlags.None);
        MM.tilemap.SetColor(tilePos, new Color(1, 0, 0, 0.3f));
        if (DM.Tiles[tilePos.x, tilePos.y].type == ETileType.END)
            DM.Tiles[tilePos.x, tilePos.y].canDraw = true;
        else if (DM.Tiles[tilePos.x, tilePos.y].type == ETileType.OVERLAP)
        {
            DM.Tiles[tilePos.x, tilePos.y].canDraw = true;
            MM.ChangeTile(tilePos, ERoad.OVERLAP); //좌표 타일을 지정된 타일로 변경
        }
        //그릴 수 없는 타일로 변경
        else
            DM.Tiles[tilePos.x, tilePos.y].canDraw = false;
        //현재 좌표를 현재 트랙 스택에 푸시
        _trackList[_curTrack].Push(tilePos);
        CharacterCtrl.instance.CharacterMoveTile[_curTrack].Add(tilemap.CellToWorld(tilePos));
        if(DM.Tiles[tilePos.x, tilePos.y].type == ETileType.SLOW)
            CharacterCtrl.instance.CharacterMoveTile[_curTrack].Add(tilemap.CellToWorld(tilePos));
        UI.UpdataCharacterInfo(_curTrack, CharacterCtrl.instance.CharacterMoveTile[_curTrack].Count);
        curSlideQueue.Enqueue(tilePos);
    }
    public Vector3Int GetTilePos()
    {
        Vector3 touchPos = tilemap.WorldToCell(_cam.ScreenToWorldPoint(Input.mousePosition)); // 터치된 좌표를 셀의 좌표로 변환하여 저장
        Vector3Int touchPosToTile = new Vector3Int((int)touchPos.x, (int)touchPos.y, 0); //저장된 좌표를 Vector3Int로 변환
        return touchPosToTile;
    }
    public void ChangeLastTile()
    {
        Vector3Int tilePos = new Vector3Int(TrackList[_curTrack].Peek().x, TrackList[_curTrack].Peek().y, 0);
        MM.ChangeTile(tilePos, ERoad.LAST);
        float rot = 0.0f;
        switch(DM.Tiles[tilePos.x, tilePos.y].dir)
        {
            case EDir.UP:
                MM.SetTileIdx((int)ERoad.HORIZONTAL);
                rot = 180.0f;
                break;
            case EDir.DOWN:
                MM.SetTileIdx((int)ERoad.HORIZONTAL);
                rot = 0.0f;
                break;
            case EDir.LEFT:
                MM.SetTileIdx((int)ERoad.VERTICAL);
                rot = 270.0f;
                break;
            case EDir.RIGHT:
                MM.SetTileIdx((int)ERoad.VERTICAL);
                rot = 90.0f;
                break;
        }
        Matrix4x4 matrix = Matrix4x4.Rotate(Quaternion.Euler(0.0f, 0.0f, rot));
        tilemap.SetTransformMatrix(tilePos, matrix);
    }
    public void ChangeStartTile(Vector3Int touchPos)
    {
        if(touchPos == TrackList[_curTrack].Peek())
            MM.ChangeTile(TrackList[_curTrack].Peek(), ERoad.VERTICAL);
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
            if (_trackList[_curTrack].Peek() == DM.StartTilePos[_curTrack])
                break;
            topTilePos = _trackList[_curTrack].Pop();

            MM.tilemap.SetTile(topTilePos, null);
            DM.Tiles[topTilePos.x, topTilePos.y].canDraw = true;
            CharacterCtrl.instance.CharacterMoveTile[_curTrack].RemoveAt(tileIdx--);
            if(DM.Tiles[topTilePos.x, topTilePos.y].type == ETileType.SLOW)
            {
                CharacterCtrl.instance.CharacterMoveTile[_curTrack].RemoveAt(tileIdx--);
            }
        }
        while (topTilePos != removePos);
    }
    public void CheckDoubleTouch(Vector3Int tilePos)
    {
        float touchTime = 1.0f;
        while(touchTime > 0)
        {
            touchTime -= Time.deltaTime;
            if (_prevTouchPos == tilePos && _trackList[_curTrack].Contains(tilePos))
            {
                _canDoubleTouch = true;
                _prevTouchPos = new Vector3Int(-1, -1, 0);

                break;
            }
            _canDoubleTouch = false;
        }
    }
    public void FindCurTrack(Vector3Int tilePos)
    {
        for(int i = 0; i < DM.StartTileNum; i++)
        {
            if(_trackList[i].Contains(tilePos))
            {
                _curTrack = i;
                return;
            }
        }
    }
    public void ChangeTileColor()
    {
        Vector3Int tilePos = new Vector3Int(0, 0, 0);
        while(curSlideQueue.Count > 0)
        {
            tilePos = curSlideQueue.Dequeue();
            MM.tilemap.SetTileFlags(tilePos, TileFlags.None);
            MM.tilemap.SetColor(tilePos, new Color(1, 1, 1, 1));
        }
    }
}