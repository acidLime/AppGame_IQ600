using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTouchEvent : MonoBehaviour {
    [SerializeField] private Camera _cam; //터치카메라

    public Vector3 GetTouchPos()
    {
        Vector3 touchPos = _cam.ScreenToWorldPoint(Input.mousePosition);

        return touchPos;
    }
    public Ray GetRay()
    {
        Ray r = _cam.ScreenPointToRay(Input.mousePosition);
        return r;
    }
    public Camera GetCamera()
    {
        return _cam;
    }
}
