using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Title,
        Tutorial1,
        Tutorial2,
        Tutorial3,
        Tutorial4,
        Tutorial5,
        Sandbox
    }

    [Header("Camera")]
    [SerializeField]
    private GameObject _gameCamera;
    [SerializeField]
    private GameObject _projectorCamera;


    [Header("Lights")]
    [SerializeField]
    private GameObject _houseLights;
    [SerializeField]
    private GameObject _goonLightsLeft;

    [Header("Mechanism")]
    [SerializeField]
    private ProjectorMechanism _projector;
    [SerializeField]
    private CurtainController _curtains;
    [SerializeField]
    private GameObject[] _scrap;
    [SerializeField]
    private GameObject[] _melody;
    [SerializeField]
    private GameObject[] _record;

    [SerializeField, ReadOnly]
    private GameState _currentGameState;

    [SerializeField]
    private bool _startSandbox = false;

    void Awake()
    {
        if(_startSandbox)
        {
            SetState(GameState.Sandbox);
        }
        else
        {
            SetState(GameState.Title);
        }
    }

    private void SetState(GameState state)
    {
        _currentGameState = state;

        switch (state) 
        { 
            case GameState.Title:
                
                _houseLights.SetActive(false);
                _goonLightsLeft.SetActive(false);
                _gameCamera.SetActive(false);
                _projectorCamera.SetActive(true);

                ProjectorMechanism.VideoPlaybackComplete += () =>
                {
                    SetState(GameState.Tutorial1);
                };

                break;
            case GameState.Tutorial1:
                // Start
                // Game Camera On
                // House Lights On
                // Goon Walk On
                // Goon Intro

                _houseLights.SetActive(true);
                _goonLightsLeft.SetActive(true);
                _gameCamera.SetActive(true);
                _projectorCamera.SetActive(false);



                break;
            case GameState.Tutorial2:
                break;
            case GameState.Tutorial3:
                break;
            case GameState.Tutorial4:
                break;
            case GameState.Tutorial5:
                break;
            case GameState.Sandbox:

                _houseLights.SetActive(true);
                _goonLightsLeft.SetActive(true);
                _gameCamera.SetActive(true);
                _projectorCamera.SetActive(false);

                break;
        }
    }
}
