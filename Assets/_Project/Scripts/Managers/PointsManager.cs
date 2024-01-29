using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    [SerializeField]
    private Speaker _speakerData;

    [SerializeField, ReadOnly]
    private double _lastClickPercentageFromBeat;

    [Header("Beat lengths")]
    [SerializeField, ReadOnly]
    private double _previousBeatTime;
    [SerializeField, ReadOnly]
    private double _currentBeatTime;
    [SerializeField, ReadOnly]
    private double _beatLength;

    // create a fixed sized queue of last 8 beats
    private List<double> _lastEightBeats;

    void Awake()
    {
        _previousBeatTime = AudioSettings.dspTime;
        _currentBeatTime = AudioSettings.dspTime;

        _lastEightBeats = new List<double>();
        _lastEightBeats.Capacity = 8;

        /* Subscripe to,
         * Goon talk
         * Intervals
         * Melody Buttons
         * Scrap caught
        */
        MelodyButtonRunMechanism.MelodyPlayed += AddPoints;

        Goon.GoonSpeak += AddPoints;

        Scrap.ScrapCaught += (scrap) =>
        {
            AddPoints();
            // add some points
        };

        // if music is played, clear lists
    }

    private void AddPoints()
    {
        Debug.Log("did a thing!!");
        double clickTime = AudioSettings.dspTime;

        // time from previous and next beats
        double fromPrevious = clickTime - _previousBeatTime;
        _lastClickPercentageFromBeat = (fromPrevious / _beatLength);

        // take the best value
    }

    public void CalculateBeats()
    {
        // do audio calcs here?
        _previousBeatTime = _currentBeatTime;
        _currentBeatTime = AudioSettings.dspTime;
        _beatLength = _currentBeatTime - _previousBeatTime;

        _lastEightBeats.Add(_beatLength);
    }
}
