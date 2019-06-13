using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    [SerializeField]
    int _clearLevel = 1;
    [SerializeField]
    int _stageLevel = 1;
    int maxStage = 9;
    public int StageLevel
    {
        get
        {
            return _stageLevel;
        }
        set
        {
            _stageLevel = value;
        }
    }
    public int ClearLevel
    {
        get
        {
            return _clearLevel;
        }
        set
        {
            _clearLevel = value;
        }
    }
    public static LevelManager instance;
    public GameObject[] UI_StagePlayButton;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }
    public void StageOpen()
    {
        for(int i = 0; i< _clearLevel; i++)
        {
            UI_StagePlayButton[i].SetActive(true);
        }
    }
}
