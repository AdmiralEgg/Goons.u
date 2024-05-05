using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Disabled is Curtains Draw
/// Enabled is Curtains Open
/// </summary>
public class CurtainEnableMechanism : BaseEnableMechanism
{
    [SerializeField]
    private EventReference _curtainSqueakEvent;
    private EventInstance _curtainSqueakInstance;

    bool _isMovingSoundPlaying = false;

    public override void Awake()
    {
        base.Awake();

        _curtainSqueakInstance = FMODUnity.RuntimeManager.CreateInstance(_curtainSqueakEvent);
    }

    public void OnEnable()
    {
        _curtainSqueakInstance = FMODUnity.RuntimeManager.CreateInstance(_curtainSqueakEvent);
    }

    private void Update()
    {
        // if InTransition, play sound
        if ((CurrentEnabledState == EnabledState.InTransition) && (_isMovingSoundPlaying == false))
        {
            _curtainSqueakInstance.start();
            _isMovingSoundPlaying = true;
        }

        if ((CurrentEnabledState != EnabledState.InTransition) && (_isMovingSoundPlaying == true))
        {
            _curtainSqueakInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _isMovingSoundPlaying = false;
        }
    }

    public void OnDisable()
    {
        _curtainSqueakInstance.release();
    }

    public void OnDestroy()
    {
        _curtainSqueakInstance.release();
    }
}
