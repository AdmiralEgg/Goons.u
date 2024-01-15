using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Title,
        Theater
    }

    [SerializeField, ReadOnly]
    private GameState _currentGameState;

    void Awake()
    {
        _currentGameState = GameState.Theater;
    }
}
