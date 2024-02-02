using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static CrowdController;
using static GameManager;

public class PointsManager : MonoBehaviour
{
    [SerializeField]
    private Speaker _speakerData;

    [Header("Point Data")]
    [SerializeField]
    private List<GameStatePointsData> _gameStatePointsData;

    [Header("Points")]
    [SerializeField, ReadOnly]
    private float _totalPoints;
    [SerializeField, ReadOnly]
    private float _targetPoints;
    [SerializeField, ReadOnly]
    private float _percentageToTarget;

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
    [SerializeField, ReadOnly]
    private Queue<double> _lastEightBeats;
    [SerializeField, ReadOnly]
    private double _lastEightMode;

    public static Action<CrowdIntensity> UpdatedCrowdIntensity;
    public static Action PointsReached;

    void Awake()
    {
        ClearPointsData();
    }

    public void SetupPointsData(GameState gameState)
    {
        ClearPointsData();

        // Get points data linked to game state, then set up
        PointsData pointsData = _gameStatePointsData.FirstOrDefault(p => p.GameState == gameState).PointsData;
                
        Speaker.MusicStarted = () =>
        {
            // Add beat based points
            AddPoints(pointsData.MusicPointsPerBeat);
        };

        MelodyButtonRunMechanism.MelodyPlayed = () =>
        {
            // Add standard points
            AddPoints(pointsData.MelodyPoints);

            // Add beat based points
        };

        Goon.GoonSpeak += (wordData) =>
        {
            // Add standard points
            AddPoints(pointsData.GoonSpeakPoints);

            // Add beat based points
        };

        Scrap.ScrapCaught += (scrap) =>
        {
            // Add standard points
            AddPoints(pointsData.ScrapCaught);
        };

        _targetPoints = pointsData.PointsTarget;
    }

    private void AddPoints(float points = 0)
    {
        double clickTime = AudioSettings.dspTime;

        // time from previous beat
        double fromPrevious = clickTime - _previousBeatTime;
        _lastClickPercentageFromBeat = (fromPrevious / _lastEightMode);

        _totalPoints += points;

        _percentageToTarget = _totalPoints / _targetPoints;

        // Check act completed
        if (_totalPoints >= _targetPoints)
        {
            // Act complete!
            PointsReached?.Invoke();
        }
        else
        {
            switch (_percentageToTarget)
            {
                case float x when (x > 0.90):
                    UpdatedCrowdIntensity?.Invoke(CrowdIntensity.High);
                    break;
                case float x when (x > 0.75):
                    UpdatedCrowdIntensity?.Invoke(CrowdIntensity.MediumHigh);
                    break;
                case float x when (x > 0.45):
                    UpdatedCrowdIntensity?.Invoke(CrowdIntensity.Medium);
                    break;
                case float x when (x > 0.35):
                    UpdatedCrowdIntensity?.Invoke(CrowdIntensity.LowMedium);
                    break;
                case float x when (x > 0.15):
                    UpdatedCrowdIntensity?.Invoke(CrowdIntensity.Low);
                    break;
            }
        }
    }

    public void CalculateBeats()
    {
        // do audio calcs here?
        _currentBeatTime = AudioSettings.dspTime;
        _beatLength = _currentBeatTime - _previousBeatTime;
        _previousBeatTime = _currentBeatTime;

        _lastEightBeats.Enqueue(_beatLength);
        
        if (_lastEightBeats.Count() > 8)
        {
            _lastEightBeats.Dequeue();
        }

        _lastEightMode = _lastEightBeats.ToArray().GroupBy(v => v)
            .OrderByDescending(g => g.Count())
            .First()
            .Key;
    }

    public void ClearPointsData()
    {
        _totalPoints = 0;
        _percentageToTarget = 0;

        _previousBeatTime = AudioSettings.dspTime;
        _currentBeatTime = AudioSettings.dspTime;

        _lastEightBeats = new Queue<double>(8);
    }

    public float GetPercentageToTarget()
    {
        return _percentageToTarget;
    }
}

[Serializable]
public class GameStatePointsData
{
    public GameState GameState;
    public PointsData PointsData;
}
