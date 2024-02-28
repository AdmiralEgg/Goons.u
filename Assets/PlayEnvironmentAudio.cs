using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEnvironmentAudio : MonoBehaviour
{
    [Header("Audio Data")]
    [SerializeField]
    private EventReference _poleTouch, _stageTouch, _trapDoorTouch, _speakerTouch, _curtainTouch;
    private FMOD.Studio.EventInstance _poleInstance, _stageInstance, _trapDoorInstance, _speakerInstance, _curtainInstance;

    void Awake()
    {
        SetupFMOD();

        InputManager.EnvironmentTouch += PlayEnvironmentHitSound;
    }

    private void SetupFMOD()
    {
        _poleInstance = FMODUnity.RuntimeManager.CreateInstance(_poleTouch);
        _stageInstance = FMODUnity.RuntimeManager.CreateInstance(_stageTouch);
        _trapDoorInstance = FMODUnity.RuntimeManager.CreateInstance(_trapDoorTouch);
        _speakerInstance = FMODUnity.RuntimeManager.CreateInstance(_speakerTouch);
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
            case "TrapDoor":
                _trapDoorInstance.start();
                break;
            case "Speaker":
                _speakerInstance.start();
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
        _trapDoorInstance.release();
        _speakerInstance.release();
        _curtainInstance.release();
    }
}
