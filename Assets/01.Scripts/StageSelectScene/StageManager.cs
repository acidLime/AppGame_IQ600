using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StageManager : MonoBehaviour
{

    void Start()
    {
        LevelManager.instance.StageOpen();

        SoundManager.instance.PlayOnBgm("event:/BGM/scene");
    }
    public void PlayOnStage(int stageLevel)
    {
        LevelManager.instance.StageLevel = stageLevel;
        SceneManager.LoadScene("GameScene");
    }

}