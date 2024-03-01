using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class GameModeManager : MonoBehaviour
{
    public enum GameMode { None, TransitionToMusic, Music, TransitionToScrap, Scrap }

    [SerializeField, ReadOnly]
    private GameMode _currentGameMode;

    public GameMode CurrentGameMode
    {
        get { return _currentGameMode; }
        private set { }
    }

    [Header("Mechanism")]
    [SerializeField]
    private MusicButtonEnableMechanism _music;
    [SerializeField]
    private ScrapButtonEnableMechanism _scrap;

    public static Action<GameMode> ChangedGameMode;

    private void Awake()
    {
        SetGameMode(GameMode.None);
    }

    private void OnEnable()
    {
        MusicButtonRunMechanism.MusicMechanismRunStateUpdate += MechanismRunStateCheck;
        ScrapButtonRunMechanism.ScrapMechanismRunStateUpdate += MechanismRunStateCheck;
    }

    private void OnDisable()
    {
        MusicButtonRunMechanism.MusicMechanismRunStateUpdate -= MechanismRunStateCheck;
        ScrapButtonRunMechanism.ScrapMechanismRunStateUpdate -= MechanismRunStateCheck;
    }

    private void Update()
    {
        // Wait for mechanisms to transition to the correct mode before switching game modes.
        switch (_currentGameMode)
        {
            case GameMode.TransitionToMusic:
                if ((_music.CurrentEnabledState == BaseEnableMechanism.EnabledState.Enabled) && _scrap.CurrentEnabledState == BaseEnableMechanism.EnabledState.Disabled)
                {
                    SetGameMode(GameMode.Music);
                }
                break;
            case GameMode.TransitionToScrap:
                if ((_music.CurrentEnabledState == BaseEnableMechanism.EnabledState.Disabled) && _scrap.CurrentEnabledState == BaseEnableMechanism.EnabledState.Enabled)
                {
                    SetGameMode(GameMode.Scrap);
                }
                break;
            default: 
                break;
        }
        
        if ((_currentGameMode == GameMode.TransitionToMusic) || (_currentGameMode == GameMode.TransitionToScrap)) 
        {
            //Debug.Log($"Transition between game modes, checking music and scrap states. Music: {_music.CurrentEnabledState}. Scrap: {_scrap.CurrentEnabledState}.");
        }
    }

    private void MechanismRunStateCheck(Type mechanismType, bool isStarting)
    {
        if (mechanismType == typeof(MusicButtonRunMechanism))
        {
            if (isStarting)
            {
                SetGameMode(GameMode.TransitionToMusic);
                //SetGameMode(GameMode.Music);
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
                SetGameMode(GameMode.TransitionToScrap);
                //SetGameMode(GameMode.Scrap);
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

        Debug.Log($"Setting game mode: {newGameMode}");

        switch (newGameMode)
        {
            case GameMode.TransitionToScrap:
                _scrap.EnableAfterAnimation();
                _music.DisableAfterAnimation();
                break;
            case GameMode.TransitionToMusic:
                _scrap.DisableAfterAnimation();
                _music.EnableAfterAnimation();
                break;
            case GameMode.Scrap:
                _music.DisableAfterAnimation();
                break;
            case GameMode.Music:
                _scrap.DisableAfterAnimation();
                break;
            case GameMode.None:
                _music.EnableAfterAnimation();
                _scrap.EnableAfterAnimation();
                break;
        }

        _currentGameMode = newGameMode;
        ChangedGameMode?.Invoke(newGameMode);
    }
}
