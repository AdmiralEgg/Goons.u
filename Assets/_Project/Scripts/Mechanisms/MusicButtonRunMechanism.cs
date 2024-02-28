using System;
using System.Collections;
using UnityEngine;

public class MusicButtonRunMechanism : BaseRunMechanism
{
    [SerializeField, Tooltip("The audio source to play the main music from")]
    private SpeakerRunMechanism _speakerMechanism;

    [SerializeField, Tooltip("Time it takes for the toggle to between start and stop states")]
    private float _rotateTimeSeconds = 5f;

    [SerializeField]
    private Light _startButtonLight, _stopButtonLight;

    public static Action<Type, bool> MusicMechanismRunStateUpdate;
    public static Action s_MusicStopped;
    public static Action s_MusicStarted;

    public static Action<bool> HouseLights;

    private void Awake()
    {
        _enableMechanism = this.GetComponent<MusicButtonEnableMechanism>();
        _currentRunningState = RunningState.Shutdown;
        PointsManager.PointsReached += StopMechanism;
        SpeakerRunMechanism.s_BeatEvent += () => ToggleAllButtonLights();
    }

    public override void StartMechanism()
    {
        StartCoroutine(StateTransition(RunningState.Running));
    }

    public override void StopMechanism()
    {
        StartCoroutine(StateTransition(RunningState.Shutdown));
    }

    public override void OnClickedTrigger()
    {
        base.OnClickedTrigger();

        if (_enableMechanism?.GetState() != BaseEnableMechanism.EnabledState.Enabled) return;

        switch (_currentRunningState) 
        {
            case RunningState.TransitionToRunning:
                Debug.Log("Switch in transition, ignoring click.");
                return;
            case RunningState.TransitionToShutdown:
                Debug.Log("Switch in transition, ignoring click.");
                return;
            case RunningState.Shutdown:
                Debug.Log("TransitionToRunning");
                StartMechanism();
                break;
            case RunningState.Running:
                Debug.Log("TransitionToShutdown");
                StopMechanism();
                break;
        }
    }

    // Used by the speaker
    public void ToggleAllButtonLights()
    {
        foreach (Light light in new Light[] { _startButtonLight, _stopButtonLight })
        {
            light.enabled = !light.enabled;
        }
    }

    public void SwitchAllButtonLights(bool switchOn)
    {
        foreach (Light light in new Light[] { _startButtonLight, _stopButtonLight })
        {
            light.enabled = switchOn;
        }
    }

    /// <summary>
    /// Slowly rotate the trigger, then light up the Play/Stop option.
    /// </summary>
    private IEnumerator StateTransition(RunningState targetState)
    {
        if (targetState == RunningState.Running)
        {
            _currentRunningState = RunningState.TransitionToRunning;
            SwitchAllButtonLights(false);
            _speakerMechanism.StartMechanism();
            MusicMechanismRunStateUpdate?.Invoke(this.GetType(), true);
        }

        if (targetState == RunningState.Shutdown)
        {
            _currentRunningState = RunningState.TransitionToShutdown;
            SwitchAllButtonLights(false);

            s_MusicStopped?.Invoke();
            _speakerMechanism.SetBeatEventCheck(false);

            // Skip to shutdown region marker.
            _speakerMechanism.SkipToDestinationMarker(SpeakerRunMechanism.MusicMarkerName.MusicEnd);

            MusicMechanismRunStateUpdate?.Invoke(this.GetType(), false);
        }

        float t = 0;

        while (t < _rotateTimeSeconds)
        {
            float fracComplete = (180 * Time.deltaTime) / _rotateTimeSeconds;

            //this.transform.RotateAround(this.transform.position, new Vector3(0, this.transform.rotation.y, 0), fracComplete);
            this.transform.Rotate(0, fracComplete, 0);

            t += Time.deltaTime;

            yield return null;
        }

        SwitchAllButtonLights(true);

        if (targetState == RunningState.Running)
        {
            // Skip to StartMusic region
            _speakerMechanism.SkipToDestinationMarker(SpeakerRunMechanism.MusicMarkerName.MusicStart);
            
            _speakerMechanism.SetBeatEventCheck(true);

            HouseLights?.Invoke(false);
            s_MusicStarted?.Invoke();
        }

        if (targetState == RunningState.Shutdown)
        {
            HouseLights?.Invoke(true);
        }

        _currentRunningState = targetState;
    }
}