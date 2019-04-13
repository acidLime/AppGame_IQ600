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
        //UI = UIManager.instance;
        //int idx = DataManager.instance.StartTileNum;
        //UI.ShowCharacterInfo(idx);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void GameStart(int stageLevel)
    {
        Debug.Log("GameStart");
        
    }
    public void EndGame()
    {
        Debug.Log("end");
    }
    public void ReStart()
    {
        Debug.Log("Reset");
    }
    public void SoundOn()
    {

    }
    public void Option()
    {

    }
}
