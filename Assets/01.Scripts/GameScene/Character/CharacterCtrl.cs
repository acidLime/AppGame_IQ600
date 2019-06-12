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
public struct Character
{
    public float startTime;

    public int startNum;
    public int characterMoveCount;
    public bool moveStart;
    public bool canMove;

    public Animator anim;
    public Vector3 targetPos;
    public GameObject character;
    public bool arrived;
}
public class CharacterCtrl : MonoBehaviour
{
    public Character[] characters;
    public float timeSpeed = 1.0f;
    DataManager DM;
    public GridLayout characterTilemap;
    public static CharacterCtrl instance;
    int _characterNum;
    public GameObject[] characterPrefab;
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

    WaitForSeconds moveWait;
    int _characterIdx;
    public float moveTime = 2.0f;
    public int n = 1;

    void Start () {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        DM = DataManager.instance;
    }

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
        _characterIdx = _characterNum -1;

        float gridSize = DM.GridSize;
        _characterMoveTile = new List<List<Vector3>>();
        for (int i = 0; i < _characterNum; i++)
        {
            characters[i] = new Character();
            _characterMoveTile.Add(new List<Vector3>());
            Vector3 worldPos = characterTilemap.CellToWorld(DM.StartTilePos[i]);
            _characterMoveTile[i].Add(worldPos);
            characters[i].character = Instantiate(characterPrefab[i], worldPos, Quaternion.identity);
            characters[i].anim = characters[i].character.GetComponentInChildren<Animator>();
            characters[i].characterMoveCount = 0;
            characters[i].canMove = false;
            characters[i].arrived = false;
            characters[i].moveStart = false;
            characters[i].character.SetActive(true);
            characters[i].character.transform.localPosition = worldPos;
            characters[i].anim.SetBool("IsMoving", false);
        }
        if((int)DM.MissionData[DM.StageLevel - 1]["possion"] == 1)
        {
            isEssential = true;
        }

    }
    public IEnumerator CanMove(int characterIdx) 
    {
        while(_characterMoveTile[characterIdx].Count > 0)
        {
            if(_characterMoveTile[characterIdx].Count <= characters[characterIdx].characterMoveCount)
            {
                StopAllCoroutines();
                GameManager.instance.GameOver();

                break;
            }
            characters[characterIdx].targetPos = _characterMoveTile[characterIdx][characters[characterIdx].characterMoveCount++];
            Vector3Int tilePos = MapManager.instance.tilemap.WorldToCell(characters[characterIdx].targetPos);
            if(tilePos == DM.StartTilePos[characterIdx])
            {
                characters[characterIdx].targetPos = _characterMoveTile[characterIdx][characters[characterIdx].characterMoveCount++];
                tilePos = MapManager.instance.tilemap.WorldToCell(characters[characterIdx].targetPos);
            }
            characters[characterIdx].canMove = true;
            
            yield return moveWait;

        }
    }
    
    void Move(int characterIdx)
    {
        characters[characterIdx].anim.SetBool("IsMoving", true);
        characters[characterIdx].anim.SetBool("IsSlow", false);
        characters[characterIdx].anim.SetBool("IsTrap", false);

        Vector3Int tilePos = MapManager.instance.tilemap.WorldToCell(characters[characterIdx].targetPos);
        SetAnimation(characterIdx, DM.Tiles[tilePos.x, tilePos.y].dir);
        Vector3 diff = characters[characterIdx].character.transform.position - characters[characterIdx].targetPos;
        
        characters[characterIdx].character.transform.position = Vector3.MoveTowards(characters[characterIdx].character.transform.position,
            characters[characterIdx].targetPos, timeSpeed * Time.deltaTime);

        if (diff.sqrMagnitude < 0.01f * 0.01f)
        {
            characters[characterIdx].canMove = false;
            if (DM.Tiles[tilePos.x, tilePos.y].type == ETileType.END)
                characters[characterIdx].arrived = true;
            if(DM.Tiles[tilePos.x, tilePos.y].type == ETileType.TRAP)
            {
                SoundManager.instance.PlaySfxSound("event:/SFX/block/essensial");
                characters[characterIdx].anim.SetBool("IsTrap", true);

                MapManager.instance.ObjectTilemap.SetTile(tilePos, null);
                isEssential = true;

            }
            if(DM.Tiles[tilePos.x, tilePos.y].type == ETileType.SLOW)
            {
                SoundManager.instance.PlaySfxSound("event:/SFX/block/slow");
                characters[characterIdx].anim.SetBool("IsSlow", true);
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
            int count = _characterMoveTile[0].Count + (int)DataManager.instance.CharacterData[DataManager.instance.StageLevel - 1]["warrior" +1];

            for (int i = 0; i < _characterNum; i++)
            {
                int idx = _characterMoveTile[i].Count - 1;
                Vector3Int tilePos = MapManager.instance.tilemap.WorldToCell(_characterMoveTile[i][idx]);
                if (DM.Tiles[tilePos.x, tilePos.y].type == ETileType.END &&
                    count == _characterMoveTile[i].Count +
                    (int)DataManager.instance.CharacterData[DataManager.instance.StageLevel - 1]["warrior" + (i + 1)])
                        checkCount++;
            }
            if (checkCount == _characterNum)
            {
                Time.timeScale = 3.0f;
                SoundManager.instance.PlaySfxSound("event:/SFX/Charater/foot/footclear");
                timeSpeed = 4.0f;
                checkCount = 0;
                isFast = true;
            }
        }
    }
}