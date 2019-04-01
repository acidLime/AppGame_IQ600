using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class CharacterCtrl : MonoBehaviour
{
    public Tilemap characterTilemap;
    public Tilemap Trackmap;

    public static CharacterCtrl instance;
    bool[] _canMove;
    int _characterNum;
    List<Queue<Vector3>> _characterMoveTile;

    public List<Queue<Vector3>> CharacterMoveTile
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
	void Update () {
	}
    void Init()
    {
        _characterNum = MapManager.instance.StartTileNum;
        wait = new WaitForSeconds(waitTime);
        _characterMoveTile = new List<Queue<Vector3>>();
        
        for (int i = 0; i < _characterNum; i++)
        {
            _characterMoveTile.Add(new Queue<Vector3>());
            _characterMoveTile[i].Enqueue(characterTilemap.CellToWorld(MapManager.instance.StartTilePos[i]));
            _character[i].SetActive(true);
            
            _character[i].transform.position = _characterMoveTile[i].Peek();
        }
        _canMove = new bool[_characterNum];

    }
    IEnumerator CanMove(int characterNum) 
    {
        while(_characterMoveTile[characterNum].Count > 0)
        {
            yield return wait;
        }
        Debug.Log("end");
        
    }
    IEnumerator CharacterMoveStart()
    {

        int characterNum = MapManager.instance.StartTileNum -1;
        while (characterNum >= 0)
        {
            yield return wait;
            StartCoroutine(CanMove(characterNum--));
        }
    }
    void Move()
    {
        _character[_characterNum - 1].transform.position = Vector3.MoveTowards(_character[_characterNum].transform.position,
            _characterMoveTile[_characterNum - 1].Dequeue(), 10f * Time.deltaTime);
    }
}