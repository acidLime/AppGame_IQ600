using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public float startTime;

    public int startNum;
    public int characterMoveCount;

    public bool canMove;

    public Animator anim;
    public Vector3 targetPos;
    public GameObject character;
    public bool arrived;

}
