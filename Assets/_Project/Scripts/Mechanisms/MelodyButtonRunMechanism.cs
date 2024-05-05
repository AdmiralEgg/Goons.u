using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using UnityEngine;

public class MelodyButtonRunMechanism : BaseRunMechanism
{
    [Header("Audio Setup")]
    [SerializeField]
    private EventReference _melodyEvent;
    [SerializeField]
    private EventReference _mechanismRunStartEvent;
    [SerializeField]
    private EventReference _mechanismRunStopEvent;

    private FMOD.Studio.EventInstance _melodyInstance;
    private FMOD.Studio.EventInstance _mechanismRunStartInstance;
    private FMOD.Studio.EventInstance _mechanismRunStopInstance;

    [Header("Light Setup")]
    [SerializeField]
    private Light _light;
    [SerializeField]
    private Color _runningColor;
    [SerializeField]
    private Color _shutdownColor;

    [Header("Animation Setup")]
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private float _clickSpamBlockDuration;
    [SerializeField]
    private bool _isSpamBlocking;

    [Header("Other")]
    [SerializeField]
    private bool _allowManualDisable = false;

    public static Action MelodyPlayed;

    private void Awake()
    {
        if (_melodyEvent.IsNull == false)
        {
            SetupFMOD();
        }

        _clickSpamBlockDuration = 0.3f;
        _isSpamBlocking = false;
        _currentRunningState = RunningState.Shutdown;
        _light.color = _shutdownColor;
    }

    private void SetupFMOD()
    {
        _melodyInstance = FMODUnity.RuntimeManager.CreateInstance(_melodyEvent);
        _mechanismRunStartInstance = FMODUnity.RuntimeManager.CreateInstance(_mechanismRunStartEvent);
        _mechanismRunStopInstance = FMODUnity.RuntimeManager.CreateInstance(_mechanismRunStopEvent);
    }

    public override void StartMechanism()
    {
        if (_currentRunningState == RunningState.Running) return;

        base.StartMechanism();
        _light.color = _runningColor;
        _animator.Play("PushIn");

        _mechanismRunStartInstance.start();
        _melodyInstance.start();

        // wait until complete then stop
        StartCoroutine(WaitForFinish());
    }

    public override void StopMechanism()
    {
        if (_currentRunningState == RunningState.Shutdown) return;

        base.StopMechanism();

        // If a user selects stop, stop waiting for the track to finish
        StopCoroutine(WaitForFinish());

        _light.color = _shutdownColor;
        _animator.Play("PopOut");

        _mechanismRunStopInstance.start();
        _melodyInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private IEnumerator WaitForFinish()
    {
        // Give time for melody to start
        yield return new WaitForSeconds(_clickSpamBlockDuration);

        PLAYBACK_STATE playbackState;
        _melodyInstance.getPlaybackState(out playbackState);
        
        while (playbackState == PLAYBACK_STATE.PLAYING)
        {
            yield return new WaitForSeconds(0.5f);
            _melodyInstance.getPlaybackState(out playbackState);
        }

        Debug.Log("Melody finished, stopping");
        
        MelodyPlayed?.Invoke();
        StopMechanism();
    }

    public override void OnClickedTrigger()
    {
        if (_isSpamBlocking) return;
        
        base.OnClickedTrigger();

        if (_currentRunningState == RunningState.Shutdown)
        {
            StartMechanism();
            StartCoroutine(BlockClickSpam());
            return;
        }

        if ((_currentRunningState == RunningState.Running) && _allowManualDisable)
        {
            StopMechanism();
            return;
        }
    }

    private IEnumerator BlockClickSpam()
    {
        _isSpamBlocking = true;
        yield return new WaitForSeconds(_clickSpamBlockDuration);
        _isSpamBlocking = false;
    }
}
