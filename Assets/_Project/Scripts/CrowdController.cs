using System;
using System.Collections;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

public class CrowdController : MonoBehaviour
{
    public enum CrowdIntensity { None, Murmering, Hushed, Low, LowMedium, Medium, MediumHigh, High, Maximum }

    [SerializeField]
    private PointsManager PointsManager;

    [SerializeField, ReadOnly]
    private CrowdIntensity _currentCrowdIntensity = CrowdIntensity.None;

    [Header("Crowd Audio")]
    [SerializeField]
    private EventReference _crowdPokeEvent;
    [SerializeField]
    private EventReference _murmerEvent;
    [SerializeField]
    private EventReference _smallCheerEvent;
    [SerializeField]
    private EventReference _mediumCheerEvent;
    [SerializeField]
    private EventReference _bigCheerEvent;

    private EventInstance _crowdPokeInstance;
    private EventInstance _murmerInstance;
    private EventInstance _smallCheerInstance;
    private EventInstance _mediumCheerInstance;
    private EventInstance _bigCheerInstance;

    [SerializeField]
    private VisualEffect[] _streamers;
    private CrowdMember[] _allCrowd;

    public static Action CrowdEntertained;

    private void Awake()
    {
        SetupFMOD();
        
        _allCrowd = GetComponentsInChildren<CrowdMember>();

        PointsManager.PointsReached += PlayFinalCrowdCheer;
        PointsManager.UpdatedCrowdIntensity += PlayCrowdReaction;
        StageManager.ActFinished += ResetCrowd;
        StageManager.ActStarted += () => PlayCrowdReaction(CrowdIntensity.Hushed);

        foreach (VisualEffect streamer in _streamers)
        {
            streamer.gameObject.SetActive(true);
            streamer.Stop();
        }
    }

    public void SetupFMOD()
    {
        _crowdPokeInstance = FMODUnity.RuntimeManager.CreateInstance(_crowdPokeEvent);
        _murmerInstance = FMODUnity.RuntimeManager.CreateInstance(_murmerEvent);
        _smallCheerInstance = FMODUnity.RuntimeManager.CreateInstance(_smallCheerEvent);
        _mediumCheerInstance = FMODUnity.RuntimeManager.CreateInstance(_mediumCheerEvent);
        _bigCheerInstance = FMODUnity.RuntimeManager.CreateInstance(_bigCheerEvent);
    }

    private void Start()
    {
        ResetCrowd();
        _murmerInstance.start();
    }

    public void PlayCrowdReaction(CrowdIntensity intensity = CrowdIntensity.None)
    {
        if (intensity <= _currentCrowdIntensity) return;

        if ((intensity != CrowdIntensity.Murmering) || (intensity != CrowdIntensity.Hushed))
        {
            _murmerInstance.setParameterByName("CrowdMurmerValues", 0.05f);
        }

        switch (intensity)
        {
            case (CrowdIntensity.Maximum):
                foreach (CrowdMember member in _allCrowd)
                {
                    member.SetMemberIntensity(CrowdMember.Intensity.High);
                    member.ThrowCosmetics();
                }

                _bigCheerInstance.start();
                break;
            case (CrowdIntensity.High):
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.High);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.High);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.High);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.High);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.High);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.High);

                _mediumCheerInstance.start();
                break;
            case (CrowdIntensity.MediumHigh):
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.High);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.High);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.High);

                _mediumCheerInstance.start();
                break;
            case (CrowdIntensity.Medium):
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.Medium);

                _mediumCheerInstance.start();
                break;
            case (CrowdIntensity.LowMedium):
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.Medium);

                _smallCheerInstance.start();
                break;
            case (CrowdIntensity.Low):
                _smallCheerInstance.start();
                break;
            case (CrowdIntensity.Hushed):
                _murmerInstance.setParameterByName("CrowdMurmerValues", 0.03f);
                break;
            case (CrowdIntensity.Murmering):
                _murmerInstance.setParameterByName("CrowdMurmerValues", 1f);
                break;
        }

        _currentCrowdIntensity = intensity;
    }

    private void PlayFinalCrowdCheer()
    {
        if (_currentCrowdIntensity == CrowdIntensity.Maximum) return;

        PlayCrowdReaction(CrowdIntensity.Maximum);
        StartCoroutine(PlayStreamers());
        CrowdEntertained?.Invoke();
    }

    private void ResetCrowd()
    {
        _currentCrowdIntensity = CrowdIntensity.None;

        foreach (CrowdMember member in _allCrowd)
        {
            member.ResetMember();
            member.ResetCosmetics();
        }

        PlayCrowdReaction(CrowdIntensity.Murmering);
    }

    private IEnumerator PlayStreamers()
    {
        foreach (VisualEffect streamer in _streamers) 
        {
            UnityEngine.Debug.Log("Streamer start");
            streamer.Play();
        }
        
        yield return new WaitForSeconds(7);

        foreach (VisualEffect streamer in _streamers)
        {
            UnityEngine.Debug.Log("Streamer stop");
            streamer.Stop();
        }
    }

    private void CrowdPoked(CrowdMember crowd)
    {
        _crowdPokeInstance.start();
    }

    private void OnDestroy()
    {
        ReleaseFMOD();
    }

    private void OnDisable()
    {
        ReleaseFMOD();    
    }

    private void ReleaseFMOD()
    {
        _smallCheerInstance.release();
        _mediumCheerInstance.release();
        _bigCheerInstance.release();
    }
}
