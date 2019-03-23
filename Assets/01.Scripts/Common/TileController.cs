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
    List<List<Vector3Int>> _trackList;
    Vector3Int[] _startPos;
    int _startTileNum;
    
    bool _isStartTile = false;
    bool _isBlockTile = false;
    bool[] _isLastTile;

    Vector3Int[] _blockTilePos;
    int _blockTileNum;

    //Vector3Int[] _endPos;

    [SerializeField] private Camera _cam;
    public Tilemap tilemap;

    private void Start()
    {
        _startTileNum = MapManager.instance.StartTileNum;
        _startPos = new Vector3Int[_startTileNum];
        _startPos = MapManager.instance.StartTilePos;

        _blockTileNum = MapManager.instance.BlockTileNum;
        _blockTilePos = new Vector3Int[_blockTileNum];
        _blockTilePos = MapManager.instance.BlockTilePos;

        _isLastTile = new bool[_startTileNum];
        _trackList = new List<List<Vector3Int>>();
        Debug.Log(_startTileNum);
        for(int i= 0; i < _startTileNum; i++)
        {
            _trackList.Add(new List<Vector3Int>());
        }
        _trackList[0].Insert(0, new Vector3Int(0,0,0));
        Debug.Log(_trackList[0][0]);
        //_endPos = new Vector3Int[_startTileNum];

        touchBegin += (touches) =>
        {
            Vector3 clickPos = tilemap.WorldToCell(_cam.ScreenToWorldPoint(Input.mousePosition)); // 클릭 좌표를 셀의 좌표로 변환하여 저장
            Vector3Int tilePos = new Vector3Int((int)clickPos.x, (int)clickPos.y, 0); //저장된 좌표를 Vector3Int로 변환
            for (int i = 0; i < _startTileNum; i++)
            {
                if (tilePos.x == _startPos[i].x && tilePos.y == _startPos[i].y)
                {
                    _isStartTile = true;
                    _isLastTile[i] = true;
                    return;
                }
            }
            _isStartTile = false;
        };
        touchEnd += (touches) =>
        {
            _isStartTile = false;
            _isBlockTile = false;
        };
        touchMove += (touches) =>
        {
            int tileCheck = 0;
            for(tileCheck = 0; tileCheck < _startTileNum; tileCheck++)
            {
                if ((!_isStartTile || !_isLastTile[tileCheck]) && _isBlockTile)
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
            MapManager.instance.ChangeTile(tilePos, MapManager.instance.TileIdx); //좌표 타일을 지정된 타일로 변경
        };
    }
    // Update is called once per frame
    void Update()
    {
        {
            TileTouchEvent();
        }
    }
    void TileTouchEvent()
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
    
}
