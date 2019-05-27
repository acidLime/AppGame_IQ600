using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTouchEvent : MonoBehaviour {
    public Transform _cam; //터치카메라
    public float cameraMoveSpeed;
    Vector2 prevPos = Vector2.zero;
    float prevDistance = 0f;

    private void Start()
    {
        _cam = Camera.main.transform;
    }
    private void Update()
    {
        CameraMove();
    }
    public void CameraMove()
    {
        int touchCount = Input.touchCount;
        if(touchCount == 1)
        {
            if (_cam.position.x < -0.01f)
                _cam.position = new Vector3(0, _cam.position.y, _cam.position.z);
            if(_cam.position.x > 11.4f)
                _cam.position = new Vector3(11.4f, _cam.position.y, _cam.position.z);

            Debug.Log(_cam.position);
            if (prevPos == Vector2.zero)
            {
                prevPos = Input.GetTouch(0).position;
                return;
            }
            Vector2 dir = (Input.GetTouch(0).position - prevPos).normalized;
            Vector3 vec = new Vector3(dir.x, 0, dir.y);
            Debug.Log(_cam.position);
            _cam.position -= vec * cameraMoveSpeed * Time.deltaTime;
            prevPos = Input.GetTouch(0).position;
            
            
        }
    }
    public void ExitDrag()
    {
        prevPos = Vector2.zero;
        prevDistance = 0f;
    }
}
