using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdKeyboard : MonoBehaviour
{
    // And, Or
    [SerializeField]
    private EventReference _noteWordAndEvent, _noteWordOrEvent;
    [SerializeField]
    private EventReference _noteAEvent, _noteCEvent, _noteGEvent;

    private EventInstance _noteWordAndInst, _noteWordOrInst, _noteAInst, _noteCInst, _noteGInst;

    void Awake()
    {
        SetupFMOD();
    }

    private void SetupFMOD()
    {
        _noteWordAndInst = FMODUnity.RuntimeManager.CreateInstance(_noteWordAndEvent);
        _noteWordOrInst = FMODUnity.RuntimeManager.CreateInstance(_noteWordOrEvent);
        _noteAInst = FMODUnity.RuntimeManager.CreateInstance(_noteAEvent);
        _noteCInst = FMODUnity.RuntimeManager.CreateInstance(_noteCEvent);
        _noteGInst = FMODUnity.RuntimeManager.CreateInstance(_noteGEvent);
    }

    private void OnDisable()
    {
        ReleaseFMOD();
    }

    private void OnDestroy()
    {
        ReleaseFMOD();
    }

    private void ReleaseFMOD()
    {
        _noteWordAndInst.release();
        _noteWordOrInst.release();
        _noteAInst.release();
        _noteCInst.release();
        _noteGInst.release();
    }
}
