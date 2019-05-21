using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StageTouchSystem : MonoBehaviour {


    delegate void listener(ArrayList touches);

    event listener touchBegin;
    event listener touchMove;
    event listener touchEnd;

    StageTouchEvent touchEvent;
    Vector3 touchPos;

    public GameObject map;

    public float Speed;
    public Vector2 nowPos, prePos;
    public Vector3 movePos;

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
            Ray r = touchEvent.GetRay();
            touchPos = touchEvent.GetTouchPos();
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("STAGESELECT"))
                {
                    SceneManager.LoadScene("GameScene");
                }
            }
                
        };
        touchEnd += (touches) =>
        {
        };
        touchMove += (touches) =>
        {
            if (map.transform.position.x > 0f)
            {
                map.transform.position = new Vector3(0f, map.transform.position.y, map.transform.position.z); return;
            }
            if (map.transform.position.x < -11.6f)
            {
                map.transform.position = new Vector3(-11.6f, map.transform.position.y, map.transform.position.z); return;
            }
            Vector3 curPos = touchEvent.GetTouchPos();
            nowPos.x = curPos.x - touchPos.x;
            movePos = (Vector3)(prePos - nowPos) * Speed;
            map.transform.Translate(movePos);
            prePos = nowPos;
        };
    }
    

}
