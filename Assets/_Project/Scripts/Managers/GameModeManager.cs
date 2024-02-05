using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class GameModeManager : MonoBehaviour
{
    public enum GameMode { None, Music, Scrap }

    [SerializeField, ReadOnly]
    private GameMode _currentGameMode;

    [Header("Mechanism")]
    [SerializeField]
    private MusicButtonEnableMechanism _music;
    [SerializeField]
    private ScrapButtonEnableMechanism _scrap;

    [Header("Lights")]
    [SerializeField]
    private GameObject _houseLights;
    [SerializeField]
    private GameObject _goonLightsLeft;

    public static Action<GameMode> ChangedGameMode;

    private void OnEnable()
    {
        SetGameMode(GameMode.None);

        MusicButtonRunMechanism.MusicMechanismRunStateUpdate += MechanismRunStateCheck;
        ScrapButtonRunMechanism.ScrapMechanismRunStateUpdate += MechanismRunStateCheck;
    }

    private void OnDisable()
    {
        SetGameMode(GameMode.None);

        MusicButtonRunMechanism.MusicMechanismRunStateUpdate -= MechanismRunStateCheck;
        ScrapButtonRunMechanism.ScrapMechanismRunStateUpdate -= MechanismRunStateCheck;
    }

    public GameMode GetCurrentGameMode()
    {
        return _currentGameMode;
    }

    private void MechanismRunStateCheck(Type mechanismType, bool isStarting)
    {
        if (mechanismType == typeof(MusicButtonRunMechanism))
        {
            if (isStarting)
            {
                SetGameMode(GameMode.Music);
            }
            else
            {
                SetGameMode(GameMode.None);
            }
        }

        if (mechanismType == typeof(ScrapButtonRunMechanism))
        {
            if (isStarting)
            {
                SetGameMode(GameMode.Scrap);
            }
            else
            {
                SetGameMode(GameMode.None);
            }
        }
    }

    public void SetGameMode(GameMode newGameMode)
    {
        if (newGameMode == _currentGameMode) return;

        switch (newGameMode)
        {
            case GameMode.Scrap:
                _music.DisableAfterAnimation();
                _scrap.EnableAfterAnimation();

                // house lights up
                _houseLights.SetActive(true);
                _goonLightsLeft.SetActive(false);

                break;
            case GameMode.Music:
                _music.EnableAfterAnimation();
                _scrap.DisableAfterAnimation();

                // house lights down
                _houseLights.SetActive(false);
                _goonLightsLeft.SetActive(true);

                break;
            case GameMode.None:
                _music.EnableAfterAnimation();
                _scrap.EnableAfterAnimation();

                _houseLights.SetActive(true);
                _goonLightsLeft.SetActive(false);

                break;
        }

        _currentGameMode = newGameMode;
        ChangedGameMode?.Invoke(newGameMode);
    }
}
