using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    UIManager UI;
    public static GameManager instance;
    private void Awake()
    {
        //instance가 null이면
        if (instance == null)
        {
            //instance는 자기자신으로
            instance = this;
        }
        else if (instance != null)
        {
            //instance를 삭제
            Destroy(gameObject);
        }
    }
    // Use this for initialization
    void Start ()
    {
        GameStart();

        UI = UIManager.instance;
        int idx = DataManager.instance.StartTileNum;
        //UI.ShowCharacterInfo(idx);
    }

    public void GameStart()
    {
        DataManager.instance.Init();

        MapManager.instance.InitMap();
        TouchEvent.instance.Init();
        CharacterCtrl.instance.Init();
        UIManager.instance.Init();
        UIManager.instance.ClosePanel();
        SoundManager.instance.PlayOnBgm("event:/BGM/stage");


    }
    public void GameOver()
    {
        SoundManager.instance.SFXEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SoundManager.instance.footEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        UIManager.instance.ShowGameOver(Random.Range(0,3));
        SoundManager.instance.BGMParameter.setValue(1);
        Time.timeScale = 0.0f;
    }
    public void EndGame()
    {
        UIManager.instance.ShowClearPanel();
        SoundManager.instance.BGMParameter.setValue(2);
        SoundManager.instance.SFXEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SoundManager.instance.footEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        LevelManager.instance.OpenTheNextStage();
        Time.timeScale = 0.0f;
    }
}
