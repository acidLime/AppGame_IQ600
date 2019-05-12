using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour {

    Animator anim;

    // Use this for initialization
    private void Awake()
    {
        anim = GetComponent<Animator>();   
    }
    
    
}
