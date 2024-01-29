using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MelodyButtonRunMechanism : BaseRunMechanism
{
    [Header("Audio Setup")]
    [SerializeField]
    private AudioClip _clip;
    [SerializeField]
    private AudioSource _source;

    [Header("Light Setup")]
    [SerializeField]
    private Light _light;
    [SerializeField]
    private Color _runningColor;
    [SerializeField]
    private Color _shutdownColor;

    [SerializeField]
    private bool _allowManualDisable = false;

    public static Action MelodyPlayed;

    private void Awake()
    {
        _currentRunningState = RunningState.Shutdown;
        _light.color = _shutdownColor;
    }

    public override void StartMechanism()
    {
        base.StartMechanism();
        _source.PlayOneShot(_clip);
        _light.color = _runningColor;
        _currentRunningState = RunningState.Running;

        MelodyPlayed?.Invoke();

        // wait until complete then stop
        StartCoroutine(WaitForFinish());
    }

    public override void StopMechanism()
    {
        base.StopMechanism();

        // If a user selects stop, stop waiting for the track to finish
        StopCoroutine(WaitForFinish());

        _source.Stop();
        _light.color = _shutdownColor;
        _currentRunningState = RunningState.Shutdown;
    }

    private IEnumerator WaitForFinish()
    {
        while (_source.isPlaying)
        {
            yield return new WaitForSeconds(0.5f);
        }

        StopMechanism();
    }

    public override void OnClickedTrigger()
    {
        base.OnClickedTrigger();

        if (_currentRunningState == RunningState.Shutdown)
        {
            StartMechanism();
            return;
        }

        if ((_currentRunningState == RunningState.Running) && _allowManualDisable)
        {
            StopMechanism();
            return;
        }
    }
}
