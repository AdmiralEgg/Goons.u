using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Speaker : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField]
    private AudioSource _musicSource;
    [SerializeField]
    private AudioSource _melodySourceLong;
    [SerializeField]
    private AudioSource _melodySourceShort;

    [Header("Music Start/Stop Clips")]
    [SerializeField]
    private AudioClip _startClip;
    [SerializeField]
    private AudioClip _stopClip;

    [Header("Trigger actions on beat intervals")]
    [SerializeField]
    private Intervals[] _intervals;

    private MMScaleShaker _shaker;

    void Awake()
    {
        _shaker = GetComponent<MMScaleShaker>();
        
        _musicSource.playOnAwake = false;
        _musicSource.loop = true;
    }

    public void StartMusic(SongData songData)
    {
        StartCoroutine(StartMusicClip(songData));
    }

    public void StopMusic()
    {
        Debug.Log($"Stopping clip: {_musicSource.clip.name}");
        _musicSource.Stop();
        _musicSource.PlayOneShot(_stopClip);
        StopAllCoroutines();
    }

    private IEnumerator StartMusicClip(SongData songData)
    {
        _musicSource.PlayOneShot(_startClip);

        yield return new WaitForSeconds(_startClip.length);

        _musicSource.clip = songData.AudioClip;
        _musicSource.Play();

        while (true) 
        { 
            foreach (Intervals interval in _intervals)
            {
                float sampledTime = (_musicSource.timeSamples / (_musicSource.clip.frequency * interval.GetBeatLength(songData.BPM)));
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
