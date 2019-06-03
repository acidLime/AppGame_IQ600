using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public float changeTime;
    public float playTime = 0;
    bool isChanged = false;
    [FMODUnity.EventRef]
    public string BGMEventPath;
    [FMODUnity.EventRef]
    public string SFXEventPath;
    public static SoundManager instance;
    public FMOD.Studio.EventInstance BGMEvent;
    public FMOD.Studio.ParameterInstance BGMParameter;
    public FMOD.Studio.EventInstance SFXEvent;
    public FMOD.Studio.ParameterInstance SFXParameter;
    FMODUnity.StudioEventEmitter emitter;
    // Use this for initialization
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(transform.gameObject);
    }
    void Start ()
    {
        PlayOnBgm("event:/BGM/main");
    }

    // Update is called once per frame
    void Update ()
    {
        ChangeParameter();
    }
    
    public void PlayOnBgm(string bgmPath)
    {
        BGMEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        BGMEventPath = bgmPath;
        BGMEvent = FMODUnity.RuntimeManager.CreateInstance(BGMEventPath);
        BGMEvent.getParameter("stage", out BGMParameter);
        BGMParameter.setValue(0);
        BGMEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));
        BGMEvent.start();
    }
    void ChangeParameter()
    {
        //playTime += 0.1f * Time.deltaTime;
        //if(playTime >= changeTime)
        //{
        //    if(isChanged)
        //    {
        //        BGMParameter.setValue(0);
        //    }
        //    else
        //    {
        //        BGMParameter.setValue(1);
        //    }
        //}
        //playTime = 0.0f;
    }
    public void PlaySfxSound(string sfxPath)
    {
        SFXEventPath = sfxPath;
        BGMEvent = FMODUnity.RuntimeManager.CreateInstance(SFXEventPath);
        BGMEvent.getParameter("Location", out SFXParameter);
        BGMEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));
        BGMEvent.start();
    }
}
