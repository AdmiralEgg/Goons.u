using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MusicButtonRunMechanism : BaseRunMechanism
{
    [SerializeField, Tooltip("Song to play")]
    private SongData _songData;

    [SerializeField, Tooltip("The audio source to play the main music from")]
    private Speaker _speaker;

    [SerializeField, Tooltip("Time it takes for the toggle to between start and stop states")]
    private float _rotateTimeSeconds = 5f;

    [SerializeField]
    private Light _startButtonLight, _stopButtonLight;

    private void Awake()
    {
        _currentRunningState = RunningState.Shutdown;
        PointsManager.PointsReached += StopMechanism;
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

    public void ToggleAllLights()
    {
        foreach (Light light in new Light[] { _startButtonLight, _stopButtonLight })
        {
            light.enabled = !light.enabled;
        }
    }

    public void SwitchAllLights(bool switchOn)
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
            SwitchAllLights(false);
            _speaker.PlayMusicStartupSound();
        }

        if (targetState == RunningState.Shutdown)
        {
            _currentRunningState = RunningState.TransitionToShutdown;
            SwitchAllLights(false);
            _speaker.StopMusic();
            _speaker.PlayMusicStopSound();
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

        SwitchAllLights(true);

        if (targetState == RunningState.Running)
        {
            _speaker.StartMusic(_songData);
        }

        _currentRunningState = targetState;
    }
}
