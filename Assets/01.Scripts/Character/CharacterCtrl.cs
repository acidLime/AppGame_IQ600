using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum CharacterDir
{
    DOWN,
    UP,
    LEFT,
    RIGHT
}
public class CharacterCtrl : MonoBehaviour
{
    
    DataManager DM;
    public GridLayout characterTilemap;
    public CharacterDir characterDir;
    public static CharacterCtrl instance;
    bool[] _canMove;
    int _characterNum;
    int[] _characterMoveCount;
    public GameObject characterPrefab;

    Vector3[] targetPos;

    //List<Queue<Vector3>> _characterMoveTile;
    List<List<Vector3>> _characterMoveTile;
    public List<List<Vector3>> CharacterMoveTile
    {
        get
        {
            return _characterMoveTile;
        }
        set
        {
            _characterMoveTile = value;
        }
    }
    GameObject[] _character;

    TileBase[] _tileBase;

    bool _isEnd = false;

    WaitForSeconds wait;
    public float waitTime = 3.0f;

    // Use this for initialization
    void Start () {
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
        DM = DataManager.instance;
        StartCoroutine(CharacterMoveStart());
        //Init();
    }
	
	// Update is called once per frame
	void Update ()
    {
        for(int characterIdx = 0; characterIdx < _characterNum; characterIdx++)
        {
            if (_canMove[characterIdx] == true)
            {
                Move(characterIdx);
            }
        }
	}
    public void Init()
    {
        _characterNum = DM.StartTileNum;
        _character = new GameObject[_characterNum];
        wait = new WaitForSeconds(waitTime);
        float gridSize = DM.GridSize;
        _characterMoveTile = new List<List<Vector3>>();
        targetPos = new Vector3[_characterNum];
       _characterMoveCount = new int[_characterNum];
        _canMove = new bool[_characterNum];

        for (int i = 0; i < _characterNum; i++)
        {
            _characterMoveTile.Add(new List<Vector3>());
            Vector3 worldPos = characterTilemap.CellToWorld(DM.StartTilePos[i]);
            _characterMoveTile[i].Add(worldPos);
            _character[i] = Instantiate(characterPrefab, _characterMoveTile[i][0], Quaternion.identity);

            _character[i].SetActive(true);
            _character[i].transform.localScale = new Vector3(gridSize, gridSize, 1);
            //_character[i].transform.position = _characterMoveTile[i][0];
            _characterMoveCount[i] = 0;
            _canMove[i] = false;
        }

    }
    IEnumerator CanMove(int characterIdx) 
    {
        while(_characterMoveTile[characterIdx].Count > 0)
        {
            
            if(_characterMoveTile[characterIdx].Count <= _characterMoveCount[characterIdx] ||
                (targetPos[characterIdx] == DM.EndTilePos && _characterMoveTile[0].Count != _characterMoveTile[1].Count - 2))
            {
                StopAllCoroutines();
                GameManager.instance.GameOver();
                break;
            }
            targetPos[characterIdx] = _characterMoveTile[characterIdx][_characterMoveCount[characterIdx]++];
            if(targetPos[characterIdx] == DM.EndTilePos && _characterMoveTile[0].Count == _characterMoveTile[1].Count - 2)
            {
                StopAllCoroutines();
                GameManager.instance.EndGame();
                break;
            }
            _canMove[characterIdx] = true;
            yield return wait;
        }
    }
    IEnumerator CharacterMoveStart()
    {

        int characterIdx = DM.StartTileNum -1;
        while (characterIdx >= 0)
        {
            yield return wait;
            StartCoroutine(CanMove(characterIdx--));
        }
    }
    void Move(int characterIdx)
    {
        Vector3 diff = _character[characterIdx].transform.position - targetPos[characterIdx];
        _character[characterIdx].transform.position = Vector3.MoveTowards(_character[characterIdx].transform.position,
            targetPos[characterIdx], 1.0f * Time.deltaTime);
        if (diff.sqrMagnitude < 0.01f * 0.01f)
            _canMove[characterIdx] = false;
    }
}