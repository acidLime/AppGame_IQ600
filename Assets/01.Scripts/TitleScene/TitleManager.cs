using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TitleManager : MonoBehaviour {

	void Update () {
        MoveOnSelectScene();
    }
    void MoveOnSelectScene()
    {
        int touchCount = Input.touchCount;
        if (touchCount == 1)
        {
            SceneManager.LoadScene("StageSelectScene");

            SoundManager.instance.PlayOnBgm("event:/BGM/scene");
        }
    }
    public void Test2()
    {
        SceneManager.LoadScene("StageSelectScene");
        

    }
}
