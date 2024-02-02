using System;
using System.Collections;
using UnityEngine;

public class MelodyButtonRunMechanism : BaseRunMechanism
{
    [Header("Audio Setup")]
    [SerializeField]
    private AudioClip _melodyClip;
    [SerializeField]
    private AudioClip _mechanismRunStartClip;
    [SerializeField]
    private AudioClip _mechanismRunStopClip;
    [SerializeField]
    private AudioSource _melodySource;
    [SerializeField]
    private AudioSource _startStopSource;

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

    [Header("Other")]
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
        if (_currentRunningState == RunningState.Running) return;

        base.StartMechanism();
        _melodySource.PlayOneShot(_melodyClip);
        _light.color = _runningColor;
        _currentRunningState = RunningState.Running;
        _animator.Play("PushIn");
        _startStopSource.PlayOneShot(_mechanismRunStopClip);

        MelodyPlayed?.Invoke();

        // wait until complete then stop
        StartCoroutine(WaitForFinish());
    }

    public override void StopMechanism()
    {
        if (_currentRunningState == RunningState.Shutdown) return;

        base.StopMechanism();

        // If a user selects stop, stop waiting for the track to finish
        StopCoroutine(WaitForFinish());

        _melodySource.Stop();
        _light.color = _shutdownColor;
        _currentRunningState = RunningState.Shutdown;
        _animator.Play("PopOut");
        _startStopSource.PlayOneShot(_mechanismRunStopClip);
    }

    private IEnumerator WaitForFinish()
    {
        while (_melodySource.isPlaying)
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
