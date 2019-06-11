using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public int clearLevel = 2;
    [SerializeField]
    int _stageLevel = 1;
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
    public static LevelManager instance;
    public GameObject[] UI_StagePlayButton;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        if (instance != null)
            Destroy(instance);
        DontDestroyOnLoad(transform.gameObject);
    }
    public void StageOpen()
    {
        for(int i = 0; i<= clearLevel; i++)
        {
            UI_StagePlayButton[i].SetActive(true);
        }
    }
    public void OpenTheNextStage()
    {
        clearLevel++;
    }
}
