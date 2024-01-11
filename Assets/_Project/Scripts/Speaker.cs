using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Speaker : MonoBehaviour
{
    [SerializeField]
    private AudioSource _speaker;

    [SerializeField]
    private AudioSource _startSoundPlayer;

    [SerializeField]
    private AudioSource _stopSoundPlayer;

    [SerializeField]
    private AudioClip _startClip;

    [SerializeField]
    private AudioClip _stopClip;

    [SerializeField]
    private Intervals[] _intervals;

    private MMScaleShaker _shaker;

    void Awake()
    {
        _shaker = GetComponent<MMScaleShaker>();
        
        _speaker.playOnAwake = false;
        _speaker.loop = true;

        _startSoundPlayer.clip = _startClip;
        _startSoundPlayer.playOnAwake = false;
        _startSoundPlayer.loop = false;

        _stopSoundPlayer.clip = _stopClip;
        _stopSoundPlayer.playOnAwake = false;
        _stopSoundPlayer.loop = false;
    }

    public void StartMusic(SongData songData)
    {
        StartCoroutine(StartMusicClip(songData));
    }

    public void StopMusic()
    {
        Debug.Log($"Stopping clip: {_speaker.clip.name}");
        _stopSoundPlayer.Play();
        _speaker.Stop();
        StopAllCoroutines();
    }

    private IEnumerator StartMusicClip(SongData songData)
    {
        _startSoundPlayer.Play();

        yield return new WaitForSeconds(_startClip.length);

        _speaker.clip = songData.AudioClip;
        _speaker.Play();

        while (true) 
        { 
            foreach (Intervals interval in _intervals)
            {
                float sampledTime = (_speaker.timeSamples / (_speaker.clip.frequency * interval.GetBeatLength(songData.BPM)));
                interval.CheckForNewBeat(sampledTime);
            }

            yield return null;
        }
    }
}

[Serializable]
public class Intervals
{
    [SerializeField]
    private float _steps;

    [SerializeField]
    private UnityEvent _trigger;

    private int _lastInterval;

    public float GetBeatLength(float bpm)
    {
       return 60f / (bpm * _steps);
    }

    public void CheckForNewBeat(float interval)
    {
        if (Mathf.FloorToInt(interval) != _lastInterval)
        {
            _lastInterval = Mathf.FloorToInt(interval);
            _trigger.Invoke();
        }
    }
}
