using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public GameObject[] characterInfo;
    public Text[] text;
    public static UIManager instance;

    // Use this for initialization
    void Start ()
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
        ShowCharacterInfo(2);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ShowCharacterInfo(int characterIdx)
    {
        for(int i = 0; i < characterIdx; i++)
        {
            characterInfo[i].SetActive(true);
        }
    }
    public void UpdataCharacterInfo(int characterIdx, int tileCount)
    {
        text[characterIdx].text = "N : " + tileCount;
    }
}
