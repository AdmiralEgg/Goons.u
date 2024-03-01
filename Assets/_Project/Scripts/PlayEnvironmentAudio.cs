using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEnvironmentAudio : MonoBehaviour
{
    [Header("Audio Data")]
    [SerializeField]
    private EventReference _poleTouch, _stageTouch, _curtainTouch;
    private FMOD.Studio.EventInstance _poleInstance, _stageInstance, _curtainInstance;

    void Awake()
    {
        SetupFMOD();

        InputManager.EnvironmentTouch += PlayEnvironmentHitSound;
    }

    private void SetupFMOD()
    {
        _poleInstance = FMODUnity.RuntimeManager.CreateInstance(_poleTouch);
        _stageInstance = FMODUnity.RuntimeManager.CreateInstance(_stageTouch);
        _curtainInstance = FMODUnity.RuntimeManager.CreateInstance(_curtainTouch);
    }

    public void PlayEnvironmentHitSound(string tag)
    {
        switch (tag)
        {
            case "Pole":
                _poleInstance.start();
                break;
            case "Stage":
                _stageInstance.start();
                break;
            case "Curtain":
                _curtainInstance.start();
                break;
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        FMODRelease();
    }

    private void OnDisable()
    {
        FMODRelease();    
    }

    private void FMODRelease() 
    {
        _poleInstance.release();
        _stageInstance.release();
        _curtainInstance.release();
    }
}
