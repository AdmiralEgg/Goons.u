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

    [SerializeField]
    private GameObject[] _lights;

    [SerializeField]
    private CurtainController _curtains;

    [SerializeField]
    private ProjectorController _screen;

    [SerializeField]
    private GameObject[] _scrapFeatures;

    [SerializeField]
    private GameObject[] _melodyFeatures;

    [SerializeField]
    private GameObject[] _recordFeatures;

    [SerializeField, ReadOnly]
    private GameState _currentGameState;

    void Awake()
    {
        SetState(GameState.Tutorial1);  
    }

    private void SetState(GameState state)
    {
        _currentGameState = state;

        switch (state) 
        { 
            case GameState.Title:
                // Zoom in on 
                break;
            case GameState.Tutorial1:
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
                break;
        }
    }
}
