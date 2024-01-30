using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.VFX;
using System.IO;

public class CrowdController : MonoBehaviour
{
    public enum CrowdIntensity { Low, LowMedium, Medium, MediumHigh, High}
    
    [SerializeField]
    private PointsManager PointsManager;

    [SerializeField, ReadOnly]
    private CrowdIntensity _currentCrowdIntensity;

    [Header("Crowd Audio")]
    [SerializeField]
    private AudioClip[] _smallCheer;
    [SerializeField]
    private AudioClip[] _mediumCheer;
    [SerializeField]
    private AudioClip[] _bigCheer;
    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private VisualEffect[] _streamers;
    private CrowdMember[] _allCrowd;

    public static Action CrowdEntertained;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _allCrowd = GetComponentsInChildren<CrowdMember>();

        PointsManager.ActCompleted += PlayFinalCrowdCheer;
        PointsManager.UpdatedCrowdIntensity += PlayCrowdReaction;

        foreach (VisualEffect streamer in _streamers)
        {
            streamer.gameObject.SetActive(true);
            streamer.Stop();
        }
    }

    public void PlayCrowdReaction(CrowdIntensity intensity)
    {
        if (intensity <= _currentCrowdIntensity) return;

        switch (intensity)
        { 
            case (CrowdIntensity.High):
                PlayFinalCrowdCheer();
                break;
            case (CrowdIntensity.MediumHigh):
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetCurrentIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetCurrentIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetCurrentIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetCurrentIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetCurrentIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetCurrentIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetCurrentIntensity(CrowdMember.Intensity.High);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetCurrentIntensity(CrowdMember.Intensity.High);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetCurrentIntensity(CrowdMember.Intensity.High);

                // play medium cheer
                _audioSource.PlayOneShot(_mediumCheer[UnityEngine.Random.Range(0, (_mediumCheer.Length - 1))]);
                break;
            case (CrowdIntensity.Medium):
                // get 3 crowd members, switch to Medium intensite
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetCurrentIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetCurrentIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetCurrentIntensity(CrowdMember.Intensity.Medium);

                // play low cheer
                _audioSource.PlayOneShot(_mediumCheer[UnityEngine.Random.Range(0, (_mediumCheer.Length - 1))]);
                break;
            case (CrowdIntensity.LowMedium):
                // get 3 crowd members, switch to Medium intensite
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetCurrentIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetCurrentIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetCurrentIntensity(CrowdMember.Intensity.Medium);

                // play low cheer
                _audioSource.PlayOneShot(_smallCheer[UnityEngine.Random.Range(0, (_smallCheer.Length - 1))]);
                break;
            case (CrowdIntensity.Low):
                // Play low cheer
                _audioSource.PlayOneShot(_smallCheer[UnityEngine.Random.Range(0, (_smallCheer.Length - 1))]);
                break;
        }
    }

    // Temporary trigger to entertain the crowd.
    public void OnClickedTrigger()
    {
        PlayFinalCrowdCheer();
    }

    private void PlayFinalCrowdCheer()
    {
        Debug.Log("Final cheer");
        
        foreach (CrowdMember member in _allCrowd)
        {
            //member.SetCurrentIntensity(CrowdMember.Intensity.High);
            member.FinalCheer();
        }
        
        _audioSource.PlayOneShot(_bigCheer[UnityEngine.Random.Range(0, (_mediumCheer.Length - 1))]);

        StartCoroutine(PlayStreamers());

        // Goon bow?
        CrowdEntertained?.Invoke();
    }

    private IEnumerator PlayStreamers()
    {
        foreach (VisualEffect streamer in _streamers) 
        {
            Debug.Log("Streamer start");
            streamer.Play();
        }
        
        yield return new WaitForSeconds(7);

        foreach (VisualEffect streamer in _streamers)
        {
            Debug.Log("Streamer stop");
            streamer.Stop();
        }
    }
}
