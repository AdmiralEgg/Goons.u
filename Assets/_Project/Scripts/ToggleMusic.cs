using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

public class ToggleMusic : MonoBehaviour
{
    public enum ToggleState
    {
        RotateToStart,
        WaitingForStart,
        RotateToStop,
        WaitingForStop
    }

    [SerializeField, Tooltip("Song to play")]
    private SongData _songData;

    [SerializeField, Tooltip("The audio sources to play the main music from")]
    private Speaker[] _connectedSpeakers;

    [SerializeField, ReadOnly, Tooltip("The readiness of this toggle to be pressed")]
    private ToggleState _currentState;

    [SerializeField, Tooltip("Time it takes for the toggle to between start and stop states")]
    private float _rotateTimeSeconds = 5f;

    void Awake()
    {
        _currentState = ToggleState.WaitingForStart;
    }

    private void OnClickedTrigger()
    {
        if ((_currentState == ToggleState.RotateToStart) || (_currentState == ToggleState.RotateToStop)) return;
        
        if (_currentState == ToggleState.WaitingForStart) 
        {
            foreach (Speaker speaker in _connectedSpeakers)
            {
                speaker.StartMusic(_songData);
            }

            _currentState = ToggleState.RotateToStop;

            StartCoroutine(StartRotatingTrigger());
            return;
        }

        if (_currentState == ToggleState.WaitingForStop)
        {
            foreach (Speaker speaker in _connectedSpeakers)
            {
                speaker.StopMusic();
            }

            _currentState = ToggleState.RotateToStart;

            StartCoroutine(StartRotatingTrigger());
            return;
        }
    }

    /// <summary>
    /// Slowly rotate the trigger, then light up the Play/Stop option.
    /// </summary>
    private IEnumerator StartRotatingTrigger()
    {
        float t = 0;

        while (t < _rotateTimeSeconds) 
        {
            float fracComplete = (180 * Time.deltaTime) / _rotateTimeSeconds;

            //this.transform.RotateAround(this.transform.position, new Vector3(0, this.transform.rotation.y, 0), fracComplete);
            this.transform.Rotate(0, fracComplete, 0);

            t += Time.deltaTime;
            
            yield return null;
        }

        if (_currentState == ToggleState.RotateToStart) _currentState = ToggleState.WaitingForStart;
        if (_currentState == ToggleState.RotateToStop) _currentState = ToggleState.WaitingForStop;
    }
}
