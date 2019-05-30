using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public GameObject[] characterInfo;
    public GameObject gameoverPanel;
    public GameObject clearPanel;
    public Sprite[] gameoverImage;
    public Text[] text;
    public static UIManager instance;
    public Image[] timer;
    public Text[] timeText;
    float[] times;
    public float showMissionTimer = 5.0f;
    public GameObject playButton;
    public GameObject stopButton;
    public GameObject blackPanel;
    public GameObject missionPanel;
    public GameObject optionPanel;
    public GameObject[] trapPanel;
    static bool isClose = false;

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
    }

    // Update is called once per frame
    void Update () {
        TimeCounter();
        ShowMissionPanel();
    }
    public void Init()
    {
        int startTileNum = 2;
        Debug.Log(startTileNum);
        ShowCharacterInfo(startTileNum);
        ShowTrapCount(DataManager.instance.TrapTileNum);
        times = new float[startTileNum];
        isClose = false;
        showMissionTimer = 5.0f;
        times[0] = 6.0f;
        times[1] = 4.0f;
        Debug.Log(DataManager.instance.StartTileNum);
        for (int i = 0; i < startTileNum; i++)
        {
            //times[i] = CharacterCtrl.instance.characters[i].startTime;
            timer[i].rectTransform.localPosition = MapManager.instance.tilemap.CellToWorld(DataManager.instance.StartTilePos[i]) + new Vector3(0.96f,0.96f, 0);
        }
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
        text[characterIdx].text = tileCount.ToString();
    }
    public void ShowGameOver(int imageIdx)
    {
        gameoverPanel.SetActive(true);
    }
    public void ShowClearPanel()
    {
        clearPanel.SetActive(true);
    }
    public void ClosePanel()
    {
        clearPanel.SetActive(false);
        gameoverPanel.SetActive(false);
    }
    public void TimeCounter()
    {
        for(int i = 0; i < DataManager.instance.StartTileNum; i++)
        {
            if(times[i] > 0)
                times[i] -= Time.deltaTime;
            else
            {
                timer[i].enabled = false;
                timeText[i].enabled = false;
            }
            timeText[i].text = Mathf.Ceil(times[i]).ToString();
        }
    }
    public void ShowTrapCount(int trapNum)
    {
        for(int i = 0; i < trapNum; i++)
        {
            trapPanel[i].SetActive(true);
        }
    }
    public void SoundOption()
    {

    }
    public void GameOption()
    {
        Time.timeScale = 0.0f;
        blackPanel.SetActive(true);
        optionPanel.SetActive(true);
    }
    public void ReturnToTitle()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("StageSelectScene");
    }
    public void GameStop()
    {
        Time.timeScale = 0.0f;
        blackPanel.SetActive(true);
    }
    public void GamePlay()
    {
        Time.timeScale = 1.0f;
        blackPanel.SetActive(false);
        optionPanel.SetActive(false);
    }
    public void ShowMissionPanel()
    {
        if(!isClose)
        {
            if (showMissionTimer >= 0.0f)
            {
                Time.timeScale = 1.0f;
                showMissionTimer -= 0.016f;
                int touchCount = Input.touchCount;
                if (touchCount == 1)
                    showMissionTimer = -1f;
                
                Time.timeScale = 0.0f;
            }
            else
            {
                missionPanel.SetActive(false);
                Time.timeScale = 1.0f;
                isClose = true;
            }
        }
    }
    
}
