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
    
    public GridLayout characterTilemap;
    public Tilemap Trackmap;
    public CharacterDir characterDir;
    public static CharacterCtrl instance;
    bool[] _canMove;
    int _characterNum;
    int[] _characterMoveCount;

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
    public GameObject[] _character;

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
        Init();
        StartCoroutine(CharacterMoveStart());
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
    void Init()
    {
        _characterNum = MapManager.instance.StartTileNum;
        wait = new WaitForSeconds(waitTime);
        float gridSize = MapManager.instance.GridSize;
        _characterMoveTile = new List<List<Vector3>>();
        targetPos = new Vector3[_characterNum];
        _characterMoveCount = new int[_characterNum];
        _canMove = new bool[_characterNum];

        for (int i = 0; i < _characterNum; i++)
        {
            _characterMoveTile.Add(new List<Vector3>());
            
            //_characterMoveTile.Add(new Queue<Vector3>());
            Vector3 worldPos = characterTilemap.CellToWorld(MapManager.instance.StartTilePos[i]);
            //_characterMoveTile[i].Enqueue(worldPos);
            _characterMoveTile[i].Add(worldPos);
            _character[i].SetActive(true);
            _character[i].transform.localScale = new Vector3(gridSize, gridSize, 1);
            _character[i].transform.position = _characterMoveTile[i][0];
            _characterMoveCount[i] = 0;
            _canMove[i] = false;
        }

    }
    IEnumerator CanMove(int characterIdx) 
    {
        while(_characterMoveTile[characterIdx].Count > 0)
        {
            targetPos[characterIdx] = _characterMoveTile[characterIdx][_characterMoveCount[characterIdx]++];
            _canMove[characterIdx] = true;
            yield return wait;
        }
    }
    IEnumerator CharacterMoveStart()
    {

        int characterIdx = MapManager.instance.StartTileNum -1;
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
        if (diff.sqrMagnitude < 0.1f * 0.1f)
            _canMove[characterIdx] = false;
    }
}