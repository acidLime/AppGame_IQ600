using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StageManager : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void StageOpen()
    {

    }
    public void PlayOnStage(int stageLevel)
    {
        SceneManager.LoadScene("GameScene");
    }

}