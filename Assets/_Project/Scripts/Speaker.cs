using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
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
    private List<Intervals> _intervals;

    [SerializeField]
    private CrowdController _crowd;
    [SerializeField, ReadOnly]
    private CrowdMember[] _allCrowd;

    public static Action MusicStopped;
    public static Action MusicStarted;

    private MMScaleShaker _shaker;

    void Awake()
    {        
        // Get all the crowd and add them to random intervals
        _allCrowd = _crowd.GetComponentsInChildren<CrowdMember>();

        foreach (CrowdMember member in _allCrowd)
        {
            CrowdMember.Timing timing = member.GetTiming();

            switch (timing)
            {
                case CrowdMember.Timing.Dragging:
                    _intervals.Add(new Intervals(0.9f, member.BounceTrigger));
                    break;
                case CrowdMember.Timing.OnBeat:
                    _intervals.Add(new Intervals(1f, member.BounceTrigger));
                    break;
                case CrowdMember.Timing.Rushing:
                    _intervals.Add(new Intervals(1.1f, member.BounceTrigger));
                    break;
                case CrowdMember.Timing.Random:
                    float randomTiming = UnityEngine.Random.Range(0.2f, 0.8f);
                    _intervals.Add(new Intervals(randomTiming, member.BounceTrigger));
                    break;
            }
        }

        _shaker = GetComponent<MMScaleShaker>();
        
        _musicSource.playOnAwake = false;
        _musicSource.loop = true;
    }

    public void PlayMusicStartupSound()
    {
        _musicSource.PlayOneShot(_startClip);
    }

    public void StartMusic(SongData songData)
    {
        StartCoroutine(StartMusicClip(songData));
    }

    public void PlayMusicStopSound()
    {
        _musicSource.PlayOneShot(_stopClip);
    }

    public void StopMusic()
    {
        if (_musicSource.isPlaying == false) return;
        
        Debug.Log($"Stopping clip: {_musicSource.clip.name}");
        _musicSource.Stop();
        StopAllCoroutines();
        MusicStopped?.Invoke();
    }

    private IEnumerator StartMusicClip(SongData songData)
    {
        Debug.Log("Starting music clip...");
        
        MusicStarted?.Invoke();

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

    public Intervals(float steps, UnityEvent trigger)
    {
        _steps = steps;
        _trigger = trigger;
    }

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

    public float GetStepsValue()
    {
        return _steps;
    }
}
