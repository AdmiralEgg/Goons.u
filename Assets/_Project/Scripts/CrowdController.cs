using System;
using System.Collections;
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
    private AudioClip[] _smallCheer;
    [SerializeField]
    private AudioClip[] _mediumCheer;
    [SerializeField]
    private AudioClip[] _bigCheer;
    [SerializeField]
    private AudioSource _audioSourceCrowdChatter;
    [SerializeField]
    private AudioSource _audioSourceCrowdReact;

    [SerializeField]
    private VisualEffect[] _streamers;
    private CrowdMember[] _allCrowd;

    public static Action CrowdEntertained;

    private void Awake()
    {
        _allCrowd = GetComponentsInChildren<CrowdMember>();

        PointsManager.PointsReached += PlayFinalCrowdCheer;
        PointsManager.UpdatedCrowdIntensity += PlayCrowdReaction;
        StageManager.ActFinished += ResetCrowd;
        StageManager.ActStarted += HushCrowd;

        foreach (VisualEffect streamer in _streamers)
        {
            streamer.gameObject.SetActive(true);
            streamer.Stop();
        }
    }

    private void Start()
    {
        ResetCrowd();
    }

    public void PlayCrowdReaction(CrowdIntensity intensity = CrowdIntensity.None)
    {
        if (intensity <= _currentCrowdIntensity) return;

        switch (intensity)
        {
            case (CrowdIntensity.Maximum):

                foreach (CrowdMember member in _allCrowd)
                {
                    member.SetMemberIntensity(CrowdMember.Intensity.High);
                    member.ThrowCosmetics();
                }

                _audioSourceCrowdChatter.Stop();
                _audioSourceCrowdReact.PlayOneShot(_bigCheer[UnityEngine.Random.Range(0, (_bigCheer.Length - 1))]);
                break;
            case (CrowdIntensity.High):
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.High);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.High);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.High);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.High);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.High);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.High);

                // play medium cheer
                _audioSourceCrowdChatter.Stop();
                _audioSourceCrowdReact.PlayOneShot(_mediumCheer[UnityEngine.Random.Range(0, (_mediumCheer.Length - 1))]);
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

                // play medium cheer
                _audioSourceCrowdChatter.Stop();
                _audioSourceCrowdReact.PlayOneShot(_mediumCheer[UnityEngine.Random.Range(0, (_mediumCheer.Length - 1))]);
                break;
            case (CrowdIntensity.Medium):
                // get 3 crowd members, switch to Medium intensite
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.Medium);

                // play low cheer
                _audioSourceCrowdChatter.Stop();
                _audioSourceCrowdReact.PlayOneShot(_mediumCheer[UnityEngine.Random.Range(0, (_mediumCheer.Length - 1))]);
                break;
            case (CrowdIntensity.LowMedium):
                // get 3 crowd members, switch to Medium intensite
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.Medium);
                _allCrowd[UnityEngine.Random.Range(0, _allCrowd.Length)].SetMemberIntensity(CrowdMember.Intensity.Medium);

                // play low cheer
                _audioSourceCrowdChatter.Stop();
                _audioSourceCrowdReact.PlayOneShot(_smallCheer[UnityEngine.Random.Range(0, (_smallCheer.Length - 1))]);
                break;
            case (CrowdIntensity.Low):
                // Play low cheer
                _audioSourceCrowdChatter.Stop();
                _audioSourceCrowdReact.PlayOneShot(_smallCheer[UnityEngine.Random.Range(0, (_smallCheer.Length - 1))]);
                break;
            case (CrowdIntensity.Hushed):
                StartCoroutine(VolumeChange(_audioSourceCrowdChatter, 0.05f, 2f));
                break;
            case (CrowdIntensity.Murmering):

                _audioSourceCrowdChatter.volume = 0.05f;
                _audioSourceCrowdChatter.Stop();
                _audioSourceCrowdChatter.Play();
                StartCoroutine(VolumeChange(_audioSourceCrowdChatter, 0.2f, 2f));
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

    private void HushCrowd()
    {
        PlayCrowdReaction(CrowdIntensity.Hushed);
    }

    private IEnumerator VolumeChange(AudioSource source, float targetVolume, float lerpTime)
    {
        float elapsedTime = 0;
        float initialVolume = source.volume;

        while (elapsedTime < lerpTime)
        {
            source.volume = Mathf.Lerp(initialVolume, targetVolume, (elapsedTime / lerpTime));
            elapsedTime += Time.deltaTime;

            yield return null;
        }
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
