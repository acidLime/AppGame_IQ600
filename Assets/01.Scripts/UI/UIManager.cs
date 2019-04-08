using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public GameObject[] characterInfo;
	// Use this for initialization
	void Start ()
    {
        ShowCharacterInfo(0);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ShowCharacterInfo(int characterCount)
    {
        for(int i = 0; i < characterCount; i++)
        {
            characterInfo[i].SetActive(true);
        }
    }
}
