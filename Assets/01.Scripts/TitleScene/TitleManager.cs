using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TitleManager : MonoBehaviour {

    public GameObject logo;
    public Image fadeOutImage;
    public GameObject startPosition;
    public GameObject EndPosition;
    float alpha = 0.0f;
    public GameObject touchText;
    bool canNextStage = false;
    bool isMove = true;
    bool canTouch = false;
    private void Start()
    {
        logo.transform.position = startPosition.transform.position;
    }
    void Update () {
        if(isMove)
            LogoMove();
        if (canTouch)
        {
            MoveOnSelectScene();
        }
        if (canNextStage)
            FadeOut();
    }
    void MoveOnSelectScene()
    {
        int touchCount = Input.touchCount;
        if (touchCount == 1)
        {
            canNextStage = true;
        }
    }
    public void Test2()
    {
        SceneManager.LoadScene("StageSelectScene");
    }
    public void FadeOut()
    {
        alpha += 0.01f;
        if(alpha < 1)
            fadeOutImage.color = new Color(0, 0, 0, alpha);
        if(alpha > 1)
        {
            ChangeScene();
        }
    }
    void ChangeScene()
    {
        SceneManager.LoadScene("StageSelectScene");
    }
    void LogoMove()
    {
        Vector3 diff = logo.transform.position - EndPosition.transform.position;

        logo.transform.position = Vector3.MoveTowards(logo.transform.position, EndPosition.transform.position, 1.0f * Time.deltaTime);
        if(diff.sqrMagnitude < 0.01f * 0.01f)
        {
            touchText.SetActive(true);
            canTouch = true;
            isMove = false;
        }
    }
}
