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
    public Character[] characters;

    DataManager DM;
    public GridLayout characterTilemap;
    public static CharacterCtrl instance;
    int _characterNum;
    public GameObject characterPrefab;
    bool isEssential = false;
    Vector3Int[] essentialPassingTile;
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
    bool isFast = false;

    WaitForSeconds startWait;
    WaitForSeconds moveWait;
    int _characterIdx;
    public float moveTime = 2.0f;
    public float startTime = 2.0f;
    public int n = 1;

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
        //Init();

    }

    // Update is called once per frame
    void Update ()
    {
        for(int characterIdx = 0; characterIdx < _characterNum; characterIdx++)
        {
            if (characters[characterIdx].canMove == true)
            {
                Move(characterIdx);
            }

        }
    }
    public void Init()
    {
        _characterNum = DM.StartTileNum;
        characters = new Character[_characterNum];

        moveWait = new WaitForSeconds(moveTime);
        startTime = moveTime * n;
        startWait = new WaitForSeconds(startTime);
        _characterIdx = _characterNum -1;

        float gridSize = DM.GridSize;
        _characterMoveTile = new List<List<Vector3>>();
        
        for (int i = 0; i < _characterNum; i++)
        {
            characters[i] = new Character();
            _characterMoveTile.Add(new List<Vector3>());
            Vector3 worldPos = characterTilemap.CellToWorld(DM.StartTilePos[i]);
            _characterMoveTile[i].Add(worldPos);

            characters[i].character = Instantiate(characterPrefab, worldPos, Quaternion.identity);
            characters[i].anim = characters[i].character.GetComponentInChildren<Animator>();
            characters[i].characterMoveCount = 0;
            characters[i].canMove = false;
            characters[i].arrived = false;
            characters[i].character.SetActive(true);
            characters[i].character.transform.localPosition = worldPos;
        }


    }
    IEnumerator CanMove(int characterIdx) 
    {
        while(_characterMoveTile[characterIdx].Count > 0)
        {
            
            if(_characterMoveTile[characterIdx].Count <= characters[characterIdx].characterMoveCount)
            {
                StopAllCoroutines();
                GameManager.instance.GameOver();

                break;
            }
            //targetPos[characterIdx] = _characterMoveTile[characterIdx][_characterMoveCount[characterIdx]++];
            //Vector3Int tilePos = MapManager.instance.tilemap.WorldToCell(targetPos[characterIdx]);
            //if(targetPos[characterIdx] == DM.EndTilePos && _characterMoveTile[0].Count == _characterMoveTile[1].Count - 2)
            characters[characterIdx].targetPos = _characterMoveTile[characterIdx][characters[characterIdx].characterMoveCount++];
            Vector3Int tilePos = MapManager.instance.tilemap.WorldToCell(characters[characterIdx].targetPos);
            characters[characterIdx].canMove = true;
            
            yield return moveWait;

        }
    }
    public IEnumerator CharacterMoveStart()
    {
        
        //int characterIdx = 1;
        yield return moveTime * 2;
        while (_characterIdx >= 0)
        {
            yield return startWait;
            StartCoroutine(CanMove(_characterIdx--));
        }
       SoundManager.instance.PlayFootSound();

    }
    void Move(int characterIdx)
    {
        Vector3Int tilePos = MapManager.instance.tilemap.WorldToCell(characters[characterIdx].targetPos);
        SetAnimation(characterIdx, DM.Tiles[tilePos.x, tilePos.y].dir);
        Vector3 diff = characters[characterIdx].character.transform.position - characters[characterIdx].targetPos;
        
        characters[characterIdx].character.transform.position = Vector3.MoveTowards(characters[characterIdx].character.transform.position,
            characters[characterIdx].targetPos, 1.0f * Time.deltaTime);

        if (diff.sqrMagnitude < 0.01f * 0.01f)
        {
            characters[characterIdx].canMove = false;
            if (DM.Tiles[tilePos.x, tilePos.y].type == ETileType.END)
                characters[characterIdx].arrived = true;
            if(DM.Tiles[tilePos.x, tilePos.y].type == ETileType.TRAP)
            {
                SoundManager.instance.PlaySfxSound("event:/SFX/block/essensial");
                MapManager.instance.ObjectTilemap.SetTile(tilePos, null);
                isEssential = true;

            }
            if(DM.Tiles[tilePos.x, tilePos.y].type == ETileType.SLOW)
            {
                SoundManager.instance.PlaySfxSound("event:/SFX/block/slow");
                characters[characterIdx].anim.SetTrigger("Slow");
            }
            DM.Tiles[tilePos.x, tilePos.y].dontDestroy = true;
            MapManager.instance.tilemap.SetTileFlags(tilePos, TileFlags.None);
            MapManager.instance.tilemap.SetColor(tilePos, new Color(0.75f, 0.75f, 0.75f, 1.0f));
            MapManager.instance.tilemap.SetTileFlags(tilePos, TileFlags.LockColor);

        }
        ClearCheck();
    }
    public void SetAnimation(int characterIdx, EDir dir)
    {
        characters[characterIdx].anim.SetInteger("CharacterDir", (int)dir);
    }
    public void ClearCheck()
    {
        int checkCount = _characterNum;
        for(int i = 0; i < _characterNum; i++)
        {
            if (characters[i].arrived == true)
                checkCount--;
        }
        if(checkCount == 0)
        {
            if (isEssential)
                GameManager.instance.EndGame();
            else
                GameManager.instance.GameOver();
        }
    }
    public void PrestoClear()
    {
        
        if(!isFast)
        {
            int checkCount = 0;
            for (int i = 0; i < _characterNum; i++)
            {
                int idx = _characterMoveTile[i].Count - 1;
                Vector3Int tilePos = MapManager.instance.tilemap.WorldToCell(_characterMoveTile[i][idx]);
                Debug.Log(DM.Tiles[tilePos.x, tilePos.y].type);
                if (DM.Tiles[tilePos.x, tilePos.y].type != ETileType.END)
                    continue;
                checkCount++;
            }
            Debug.Log(checkCount);
            if (checkCount == _characterNum)
            {
                Time.timeScale = 3.0f;
                SoundManager.instance.PlaySfxSound("event:/SFX/Charater/foot/footclear");
                moveWait = new WaitForSeconds(1.0f);
                checkCount = 0;
                isFast = true;
            }
        }
    }
}