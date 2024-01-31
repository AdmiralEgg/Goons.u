using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

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

    [SerializeField, Tooltip("The light that are switched when music is playing")]
    private Light[] _connectedLights;

    [SerializeField, ReadOnly, Tooltip("The readiness of this toggle to be pressed")]
    private ToggleState _currentState;

    [SerializeField, Tooltip("Time it takes for the toggle to between start and stop states")]
    private float _rotateTimeSeconds = 5f;

    void Awake()
    {
        _currentState = ToggleState.WaitingForStart;

        PointsManager.PointsReached += StopMusic;
    }

    private void StopMusic()
    {
        if (_currentState == ToggleState.WaitingForStop)
        {
            OnClickedTrigger();
        }
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

            foreach (Light light in _connectedLights)
            {
                light.enabled = false;
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

            foreach (Light light in _connectedLights)
            {
                light.enabled = true;
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
