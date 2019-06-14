using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public enum Tutorial
{
    STAGE1,
    STAGE1_DRAG,
    STAGE2,
    STAGE3
}
public class UIManager : MonoBehaviour {

    int startTileNum;

    public GameObject[] characterInfo;
    public GameObject gameoverPanel;
    public GameObject clearPanel;
    public Text[] characterInfoText;
    public static UIManager instance;
    public GameObject[] TimerObject;
    public Image[] timer;
    public Text[] timeText;
    float[] times;
    public float showMissionTimer = 5.0f;
    public GameObject playButton;
    public GameObject stopButton;
    public GameObject blackPanel;
    public GameObject missionPanel;
    public Image missionImage;
    public Sprite[] missionSprite;
    public GameObject optionPanel;
    public GameObject[] trapPanel;
    public Sprite[] tutorialSpriteSet;
    public Image tutorialImage;
    public GameObject tutorialPanel;

    static bool isClose = false;
    bool isStart = false;
    bool isTutorialMode = true;
    public GameObject nightOverRey;
    float alpha = 1.0f;

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
        //if(isTutorialMode)
        //{
        //    PlayTutorialMode(Tutorial.STAGE1);
        //}
    }

    // Update is called once per frame
    void Update () {
        TimeCounter();
        ShowMissionPanel();
        if (isStart && alpha > 0.0f)
            EndNight();
    }
    public void Init()
    {
        startTileNum = DataManager.instance.StartTileNum;
        ShowCharacterInfo();
        ShowTrapCount(DataManager.instance.TrapTileNum);
        times = new float[startTileNum];
        isClose = false;
        showMissionTimer = 5.0f;
        for (int i = 0; i < startTileNum; i++)
        {
            TimerObject[i].SetActive(true);
            times[i] = (CharacterCtrl.instance.moveTime *
                (int)DataManager.instance.CharacterData[DataManager.instance.StageLevel - 1]["warrior" + (i + 1)]) + 4;
            timer[i].rectTransform.localPosition =
                MapManager.instance.tilemap.CellToWorld(DataManager.instance.StartTilePos[i]) + new Vector3(0.35f * DataManager.instance.GridSize,1.0f * DataManager.instance.GridSize, 0);
        }
    }
    public void ShowCharacterInfo()
    {
        for(int i = 0; i < startTileNum; i++)
        {
            characterInfo[i].SetActive(true);
        }
    }
    public void UpdataCharacterInfo(int characterIdx, int tileCount)
    {
        characterInfoText[characterIdx].text = tileCount.ToString();
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
        for(int i = 0; i < startTileNum; i++)
        {
            if(times[i] > 0)
                times[i] -= Time.deltaTime;
            //times[i] -= CharacterCtrl.instance.timeSpeed * Time.deltaTime;

            else
            {
                timer[i].enabled = false;
                timeText[i].enabled = false;
                if(CharacterCtrl.instance.characters[i].moveStart == false)
                {
                    StartCoroutine(CharacterCtrl.instance.CanMove(i));
                    CharacterCtrl.instance.characters[i].moveStart = true;
                    SoundManager.instance.PlaySfxSound("event:/SFX/stage/start 0sec");
                    if(!isStart)
                    {
                        EndNight();
                        SoundManager.instance.PlayFootSound();
                        isStart = true;
                    }
                }
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
    public void GameOption()
    {
        Time.timeScale = 0.0f;
        blackPanel.SetActive(true);
        optionPanel.SetActive(true);
        SoundManager.instance.PlaySfxSound("event:/SFX/UI/yes");

    }
    public void ReturnToTitle()
    {

        Time.timeScale = 1.0f;
        SoundManager.instance.BGMParameter.setValue(3);
        SoundManager.instance.footEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        SceneManager.LoadScene("StageSelectScene");
        SoundManager.instance.PlaySfxSound("event:/SFX/UI/no");

    }
    public void GameStop()
    {
        Time.timeScale = 0.0f;
        blackPanel.SetActive(true);
        SoundManager.instance.PlaySfxSound("event:/SFX/UI/yes");

    }
    public void GamePlay()
    {
        Time.timeScale = 1.0f;
        blackPanel.SetActive(false);
        optionPanel.SetActive(false);
        SoundManager.instance.PlaySfxSound("event:/SFX/UI/no");

    }
    public void ShowMissionPanel()
    {
        if(!isClose)
        {
            Time.timeScale = 0.0f;

            missionImage.sprite = missionSprite[DataManager.instance.StageLevel - 1];

            if (showMissionTimer >= 0.0f)
            {

                
                showMissionTimer -= 0.016f;

                int touchCount = Input.touchCount;
                if (touchCount == 1)
                    showMissionTimer = -1f;
                
            }
            else
            {
                missionPanel.SetActive(false);
                Time.timeScale = 1.0f;
                isClose = true;
            }
        }
    }
    public void EndNight()
    {
        alpha -= 0.006f;
        nightOverRey.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, alpha);
    }
    public void PlayTutorialMode(Tutorial mode)
    {
        tutorialPanel.SetActive(true);
        switch(mode)
        {
            case Tutorial.STAGE1:
                ShowTutorial(0, 5);
                break;
            case Tutorial.STAGE1_DRAG:
                ShowTutorial(5, 7);
                break;
            case Tutorial.STAGE2:
                ShowTutorial(7, 13);
                break;
            case Tutorial.STAGE3:
                ShowTutorial(13, 18);
                break;
        }
    }
    void ShowTutorial(int start, int end)
    {
        int idx = start;
        while(idx != end)
        {
            Time.timeScale = 1.0f;
            int touchCount = Input.touchCount;
            if (touchCount == 1)
            {
                tutorialImage.sprite = tutorialSpriteSet[idx];
                idx++;
            }
            Time.timeScale = 0.0f;
        }

    }
    public void ReStart()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void NextStage()
    {
        LevelManager.instance.StageLevel++;
        SceneManager.LoadScene("GameScene");
    }
}
