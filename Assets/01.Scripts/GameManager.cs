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

        //UI = UIManager.instance;
        //int idx = DataManager.instance.StartTileNum;
        //UI.ShowCharacterInfo(idx);
    }

    // Update is called once per frame
    void Update () {
		
	}
    public void GameStart()
    {
        DataManager.instance.Init();

        MapManager.instance.InitMap();
        TouchEvent.instance.Init();
        CharacterCtrl.instance.Init();

        UIManager.instance.ClosePanel();

    }
    public void GameOver()
    {
        UIManager.instance.ShowGameOver(Random.Range(0,3));
    }
    public void EndGame()
    {
        DataManager.instance.StageLevel++;
        UIManager.instance.ShowClearPanel();
        Debug.Log("end");
    }
}
