using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TouchSystem : MonoBehaviour {

    delegate void listener(ArrayList touches);

    event listener touchBegin;
    event listener touchMove;
    event listener touchStationary;
    event listener touchEnd;

    bool _isReset = false;
    bool _isDraw = false;
    bool _isMove = false;

    List<Stack<Vector3Int>> _trackList;
    int _curTrack = 0;

    [SerializeField] private Camera _cam;
    public Tilemap tilemap;

    Vector3Int[] _startTilePos;
    Vector3Int _prevTilePos;

    int _startTileNum = 0;

    // Use this for initialization
    void Start()
    {
        Init();

        TouchFlags();
    }
	
	// Update is called once per frame
	void Update ()
    {
        TileTouchEvent();
    }
    void TileTouchEvent()
    {
        int count = Input.touchCount;//터치 수
        if (count == 0) return;

        //이벤트 플래그
        bool begin, move, stationary, end;
        begin = move = stationary = end = false;

        ArrayList result = new ArrayList();

        for (int i = 0; i < count; i++)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId)) return;

            Touch touch = Input.GetTouch(i);
            result.Add(touch);
            if (touch.phase == TouchPhase.Began && touchBegin != null) begin = true;
            else if (touch.phase == TouchPhase.Moved && touchMove != null) move = true;
            else if (touch.phase == TouchPhase.Stationary && touchMove != null) move = true;
            else if (touch.phase == TouchPhase.Ended && touchEnd != null) end = true;
        }
        if (begin) touchBegin(result);
        if (move) touchMove(result);
        if (move) touchStationary(result);
        if (end) touchEnd(result);
    }
    void TouchFlags()
    {
        touchBegin += (touches) =>
        {
            Vector3 clickPos = tilemap.WorldToCell(_cam.ScreenToWorldPoint(Input.mousePosition)); // 클릭 좌표를 셀의 좌표로 변환하여 저장
            Vector3Int clickPosToTile = new Vector3Int((int)clickPos.x, (int)clickPos.y, 0); //저장된 좌표를 Vector3Int로 변환
            _isDraw = TileCheck(clickPosToTile);
        };
        touchEnd += (touches) =>
        {
        };
        touchStationary += (touches) =>
        {
            
        };
        touchMove += (touches) =>
        {
            //시작타일이 아니라면 리턴
            if (!_isDraw)
                return;
            Vector3 slidePos = tilemap.WorldToCell(_cam.ScreenToWorldPoint(Input.mousePosition)); // 슬라이드 좌표를 셀의 좌표로 변환하여 저장
            Vector3Int slidePosToTile = new Vector3Int((int)slidePos.x, (int)slidePos.y, 0); //저장된 좌표를 Vector3Int로 변환

            if (slidePosToTile == _startTilePos[_curTrack])
                return;
            if (_trackList[_curTrack].Peek() == slidePosToTile) //  
            {
                MapManager.instance.ChangeTile(_trackList[_curTrack].Pop(), Tile.ETileType.NORMAL); //좌표 타일을 지정된 타일로 변경
                _prevTilePos = _trackList[_curTrack].Peek();
                return;
            }
            _prevTilePos = _trackList[_curTrack].Peek();

            _trackList[_curTrack].Push(slidePosToTile);
            SetDirection(_prevTilePos, slidePosToTile);
            MapManager.instance.ChangeTile(slidePosToTile, MapManager.instance.TileIdx); //좌표 타일을 지정된 타일로 변경
        };
    }
    void Init()
    {
        _trackList = new List<Stack<Vector3Int>>();

        _startTileNum = MapManager.instance.StartTileNum;

        _startTilePos = new Vector3Int[_startTileNum];
        _startTilePos = MapManager.instance.StartTilePos;

        for (int i = 0; i < _startTileNum; i++)
        {
            _trackList.Add(new Stack<Vector3Int>());
            _trackList[i].Push(_startTilePos[i]);
        }
    }
    //타일을 리셋시킴
    private void TileReset(int lineNum)
    {
        while(_trackList[lineNum].Count > 1)
            MapManager.instance.ChangeTile(_trackList[lineNum].Pop(), Tile.ETileType.NORMAL);
    }
    bool TileCheck(Vector3Int tilePos)
    {
        for(int i = 0; i < _startTileNum; i++)
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
}
