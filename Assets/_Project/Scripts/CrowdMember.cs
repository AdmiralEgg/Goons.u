using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CrowdMember : MonoBehaviour
{
    public enum Timing { Dragging, OnBeat, Rushing, Random }
    public enum CheerVolume { Quiet, Loud }

    private MMPositionShaker _positionShaker;

    [SerializeField, ReadOnly]
    private Timing _timing = Timing.OnBeat;

    public UnityEvent BounceTrigger;

    void Awake()
    {
        _positionShaker = GetComponent<MMPositionShaker>();
        BounceTrigger.AddListener(Bounce);

        // Randomise their timing
        float i = Random.Range(0f, 1f);
        
        switch (i)
        {
            case float x when (x <= 0.15):
                _timing = Timing.Dragging;
                break;
            case float x when (x >= 0.95):
                _timing = Timing.Random;
                break;
            case float x when (x >= 0.85):
                _timing = Timing.Rushing;
                break;
            default:
                _timing = Timing.OnBeat;
                break;
        }
    }

    public void Bounce()
    {
        _positionShaker.Play();
    }

    public void Cheer(CheerVolume cheerVolume)
    {
        switch (cheerVolume)
        {
            case CheerVolume.Quiet:

                break;
            case CheerVolume.Loud:
                break;

        }
    }

    public Timing GetTiming()
    {
        return _timing;
    }
}
