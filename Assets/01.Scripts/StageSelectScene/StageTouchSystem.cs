using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class StageTouchSystem : MonoBehaviour {


    delegate void listener(ArrayList touches);

    event listener touchBegin;
    event listener touchMove;
    event listener touchEnd;

    StageTouchEvent touchEvent;


    public float Speed;

    // Use this for initialization
    void Start()
    {
        TouchFlags();
        touchEvent = GetComponent<StageTouchEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        TouchEvent();
    }
    void TouchEvent()
    {
        int count = Input.touchCount;//터치 수
        if (count == 0) return;

        //이벤트 플래그
        bool begin, move, end;
        begin = move = end = false;

        ArrayList result = new ArrayList();

        for (int i = 0; i < count; i++)
        {

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
                
        };
        touchEnd += (touches) =>
        {
        };
        touchMove += (touches) =>
        {
        };
    }
    

}
