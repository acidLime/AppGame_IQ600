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
    public Animator[] anim;
    public GridLayout characterTilemap;
    public CharacterDir characterDir;
    public static CharacterCtrl instance;
    bool[] _canMove;
    int _characterNum;
    int[] _characterMoveCount;
    public GameObject characterPrefab;

    Vector3[] targetPos;
    Vector3Int[] essentialPassingTile;
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
    WaitForSeconds startWait;
    WaitForSeconds moveWait;
    int _characterIdx;
    public float moveTime = 2.0f;
    public float startTime = 0.0f;
    public int n = 2;
    bool[] arrived;

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
        moveWait = new WaitForSeconds(moveTime);
        startTime = moveTime * n;
        startWait = new WaitForSeconds(startTime);
        anim = new Animator[_characterNum];
        _characterIdx = _characterNum -1;

        float gridSize = DM.GridSize;
        _characterMoveTile = new List<List<Vector3>>();
        targetPos = new Vector3[_characterNum];
       _characterMoveCount = new int[_characterNum];
        _canMove = new bool[_characterNum];
        arrived = new bool[_characterNum];

        for (int i = 0; i < _characterNum; i++)
        {
            _characterMoveTile.Add(new List<Vector3>());
            Vector3 worldPos = characterTilemap.CellToWorld(DM.StartTilePos[i]);
            _characterMoveTile[i].Add(worldPos);
            _character[i] = Instantiate(characterPrefab, _characterMoveTile[i][0], Quaternion.identity);

            _character[i].SetActive(true);
            _character[i].transform.localScale = new Vector3(gridSize, gridSize, 1);
            anim[i] = _character[i].GetComponentInChildren<Animator>();
            //_character[i].transform.position = _characterMoveTile[i][0];
            _characterMoveCount[i] = 0;
            _canMove[i] = false;
        }

    }
    IEnumerator CanMove(int characterIdx) 
    {
        while(_characterMoveTile[characterIdx].Count > 0)
        {
            
            if(_characterMoveTile[characterIdx].Count <= _characterMoveCount[characterIdx])
            {
                StopAllCoroutines();
                GameManager.instance.GameOver();
                break;
            }
            targetPos[characterIdx] = _characterMoveTile[characterIdx][_characterMoveCount[characterIdx]++];
            Vector3Int tilePos = MapManager.instance.tilemap.WorldToCell(targetPos[characterIdx]);
            //if(targetPos[characterIdx] == DM.EndTilePos && _characterMoveTile[0].Count == _characterMoveTile[1].Count - 2)
            
            _canMove[characterIdx] = true;
            yield return moveWait;
        }
    }
    IEnumerator CharacterMoveStart()
    {
        
        //int characterIdx = 1;
        yield return moveTime;
        while (_characterIdx >= 0)
        {
            yield return startWait;
            Debug.Log(startWait);
            StartCoroutine(CanMove(_characterIdx--));
        }
    }
    void Move(int characterIdx)
    {
        Vector3Int tilePos = MapManager.instance.tilemap.WorldToCell(targetPos[characterIdx]);
        SetAnimation(characterIdx, DM.Tiles[tilePos.x, tilePos.y].dir);
        Vector3 diff = _character[characterIdx].transform.position - targetPos[characterIdx];
        
        _character[characterIdx].transform.position = Vector3.MoveTowards(_character[characterIdx].transform.position,
            targetPos[characterIdx], 1.0f * Time.deltaTime);

        if (diff.sqrMagnitude < 0.01f * 0.01f)
        {
            _canMove[characterIdx] = false;
            if (DM.Tiles[tilePos.x, tilePos.y].type == ETileType.END)
                arrived[characterIdx] = true;
        }

    }
    public void SetAnimation(int characterIdx, EDir dir)
    {
        anim[characterIdx].SetInteger("CharacterDir", (int)dir);
    }
}