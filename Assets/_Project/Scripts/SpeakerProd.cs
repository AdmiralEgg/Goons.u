using FMOD.Studio;
using FMODUnity;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class SpeakerProd : MonoBehaviour
{
    [SerializeField]
    private EventReference _speakerProdEvent;
    private EventInstance _speakerProdInstance;

    [SerializeField]
    private VisualEffect[] _sparks;

    [SerializeField, ReadOnly]
    private bool _sparksPlaying;

    private MMWiggle _speakerRotateWiggle;

    void Awake()
    {
        _speakerRotateWiggle = GetComponent<MMWiggle>();
        _speakerProdInstance = FMODUnity.RuntimeManager.CreateInstance(_speakerProdEvent);
        _sparksPlaying = false;
    }

    private void OnClickedTrigger()
    {
        if (_sparksPlaying == true) return;
        
        // unify with the sound

        StartCoroutine(PlaySparks());
        _speakerRotateWiggle.WiggleRotation(1.5f);
        _speakerProdInstance.start();
    }

    private IEnumerator PlaySparks()
    {
        if (_sparks.Length == 0) yield return null;
        
        _sparksPlaying = true;
        
        int sparksToPlay = 3;
        
        // play 3 times, randomly, hold onto playing value
        for (int i = 0; i < sparksToPlay; i++)
        {
            _sparks[UnityEngine.Random.Range(0, _sparks.Length)].Play();
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 2f));
        }

        _sparksPlaying = false;
        yield return null;
    }

    private void OnDisable()
    {
        _speakerProdInstance.release();
    }

    private void OnDestroy()
    {
        _speakerProdInstance.release();
    }
}