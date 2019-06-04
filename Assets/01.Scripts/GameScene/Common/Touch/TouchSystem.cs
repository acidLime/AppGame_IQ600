using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TouchSystem : MonoBehaviour {

    delegate void listener(ArrayList touches);

    event listener touchBegin;
    event listener touchMove;
    event listener touchEnd;

    TouchEvent touchEvent;

    // Use this for initialization
    void Start()
    {
        TouchFlags();
        touchEvent = GetComponent<TouchEvent>();
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
        bool begin, move, end;
        begin = move = end = false;

        ArrayList result = new ArrayList();

        for (int i = 0; i < count; i++)
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
    
    void TouchFlags()
    {
        touchBegin += (touches) =>
        {
            Vector3Int touchPos = touchEvent.GetTilePos();
            touchEvent.IsStart = touchEvent.IsStartTile(touchPos);
            touchEvent.FindCurTrack(touchPos);
            touchEvent.CheckDoubleTouch(touchPos);
            //Debug.Log(touchPos);
            if (touchEvent.CanDoubleTouch == true)
            {
                touchEvent.RemoveTile(touchPos);
                touchEvent.CanDoubleTouch = false;
                return;
            }
            if (touchEvent.IsStart)
                touchEvent.PrevTilePos = touchEvent.GetPrevTilePos();

            touchEvent.PrevTouchPos = touchPos;
            touchEvent.ChangeStartTile(touchPos);
            touchEvent.curSlideQueue.Enqueue(touchPos);

        }; 
        touchEnd += (touches) =>
        {
            touchEvent.ChangeLastTile();
            touchEvent.ChangeTileColor();
            CharacterCtrl.instance.PrestoClear();
        };
        touchMove += (touches) =>
        {
            Vector3Int slidePos = touchEvent.GetTilePos();
            if (!touchEvent.CanTouchProc(slidePos) || !touchEvent.CanTileDraw(slidePos))
                return;

            touchEvent.DrawTile(slidePos);

        };
    }
}
