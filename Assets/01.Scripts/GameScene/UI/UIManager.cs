using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public GameObject[] characterInfo;
    public GameObject gameoverPanel;
    public GameObject clearPanel;
    public Sprite[] gameoverImage;
    public Text[] text;
    public static UIManager instance;

    public GameObject[] timer;
    public Text[] timeText;
    float[] times;
    public GameObject[] trapPanel;

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
        ShowTrapCount(1);
        times = new float[2];
        times[0] = 10.0f;
        times[1] = 6.0f;
        Debug.Log(DataManager.instance.StartTileNum);
        for(int i = 0; i < 2; i++)
        {
            timer[i].transform.position = Camera.main.ScreenToWorldPoint(MapManager.instance.tilemap.CellToWorld(DataManager.instance.StartTilePos[i]));
            Debug.Log(timer[i].transform.position);
        }
    }

    // Update is called once per frame
    void Update () {
        TimeCounter();

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
                timer[i].SetActive(false);
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
}
