using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TileController : MonoBehaviour
{
    delegate void listener(ArrayList touches);

    event listener touchBegin;
    event listener touchMove;
    event listener touchEnd;

    List<Stack<Vector3Int>> _trackList;
    Vector3Int[] _startTilePos;
    int _startTileNum;
    
    bool _isStartTile = false;
    bool _isBlockTile = false;
    int _curLine = 0;
    
    Vector3Int[] _blockTilePos;
    int _blockTileNum;

    Vector3Int[] _lastTilePos;

    //Vector3Int[] _endPos;

    [SerializeField] private Camera _cam;
    public Tilemap tilemap;

    private void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        {
            TileTouch();
        }
    }
    void Init()
    {
        _startTileNum = MapManager.instance.StartTileNum;
        _startTilePos = new Vector3Int[_startTileNum];
        _startTilePos = MapManager.instance.StartTilePos;

        _blockTileNum = MapManager.instance.BlockTileNum;
        _blockTilePos = new Vector3Int[_blockTileNum];
        _blockTilePos = MapManager.instance.BlockTilePos;

        _lastTilePos = new Vector3Int[_startTileNum];

        //타일을 담을 리스트
        _trackList = new List<Stack<Vector3Int>>();

        for (int i = 0; i < _startTileNum; i++)
        {
            _trackList.Add(new Stack<Vector3Int>());
            _lastTilePos[i] = new Vector3Int(0, 0, 0);
            _trackList[i].Push(_startTilePos[i]);
        }
    }
    void TileTouch()
    {
        int count = Input.touchCount;//터치 수
        if (count == 0) return;

        //이벤트 플래그
        bool begin, move, end;
        begin = move = end = false;

        ArrayList result = new ArrayList();

        for(int i = 0; i < count; i++)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId)) return;

            Touch touch = Input.GetTouch(i);
            result.Add(touch);
            if (touch.phase == TouchPhase.Began && touchBegin != null) begin = true;
            else if (touch.phase == TouchPhase.Moved && touchMove != null) move = true;
            else if (touch.phase == TouchPhase.Ended && touchEnd != null) end = true;
        }
        if (begin) touchBegin(result);
        if (move) touchMove(result);
        if (end) touchEnd(result);
    }
    void TouchEvent()
    {
        touchBegin += (touches) =>
        {
            Vector3 clickPos = tilemap.WorldToCell(_cam.ScreenToWorldPoint(Input.mousePosition)); // 클릭 좌표를 셀의 좌표로 변환하여 저장
            Vector3Int tilePos = new Vector3Int((int)clickPos.x, (int)clickPos.y, 0); //저장된 좌표를 Vector3Int로 변환
            int i;
            for (i = 0; i < _startTileNum; i++)
            {
                if (tilePos.x == _startTilePos[i].x && tilePos.y == _startTilePos[i].y)
                {
                    _isStartTile = true;
                    return;
                }
                else if (tilePos.x == _lastTilePos[i].x && tilePos.y == _lastTilePos[i].y)
                {
                    _curLine = i;
                    return;
                }
                else
                {
                    _isStartTile = false;
                }
            }
        };
        touchEnd += (touches) =>
        {
            _isStartTile = false;
            _isBlockTile = false;
        };
        touchMove += (touches) =>
        {
            int tileCheck = 0;
            for (tileCheck = 0; tileCheck < _startTileNum; tileCheck++)
            {
                if ((!_isStartTile) && _isBlockTile)
                {
                    return;
                }
            }
            if (tileCheck < _startTileNum)
                return;
            Vector3 slidePos = tilemap.WorldToCell(_cam.ScreenToWorldPoint(Input.mousePosition)); // 슬라이드 좌표를 셀의 좌표로 변환하여 저장
            Vector3Int tilePos = new Vector3Int((int)slidePos.x, (int)slidePos.y, 0); //저장된 좌표를 Vector3Int로 변환
            for (int i = 0; i < _blockTileNum; i++)
            {
                if (slidePos.x == _blockTilePos[i].x && slidePos.y == _blockTilePos[i].y)
                {
                    _isBlockTile = true;
                    return;
                }
            }
            if (_trackList[_curLine].Pop() == tilePos)
            {
                _trackList[_curLine].Push(tilePos);
                return;
            }
            _trackList[_curLine].Push(tilePos);
            MapManager.instance.ChangeTile(tilePos, MapManager.instance.TileIdx); //좌표 타일을 지정된 타일로 변경
            
        };
    }
    
}
