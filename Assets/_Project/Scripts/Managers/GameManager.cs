using System;
using System.Collections;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { Title, Act1, Act2, Act3, Act4, Act5, Sandbox }
    public enum GameMode { None, Music, Scrap }

    [Header("Game State")]
    [SerializeField, ReadOnly]
    private GameState _currentGameState;
    [SerializeField, ReadOnly]
    private GameMode _currentGameMode;
    [SerializeField]
    private GameState _startingGameState = GameState.Title;

    [Header("Act Data")]
    [SerializeField]
    private ActData _title;
    [SerializeField]
    private ActData _act1;
    [SerializeField]
    private ActData _act2;
    [SerializeField]
    private ActData _act3;
    [SerializeField]
    private ActData _act4;
    [SerializeField]
    private ActData _act5;
    [SerializeField]
    private ActData _sandbox;

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
    private ProjectorEnableMechanism _projector;
    [SerializeField]
    private CurtainEnableMechanism[] _curtains;
    [SerializeField]
    private MusicButtonEnableMechanism _music;
    [SerializeField]
    private ScrapButtonEnableMechanism _scrap;
    [SerializeField]
    private MelodyButtonEnableMechanism _melodyButtonLong;
    [SerializeField]
    private MelodyButtonEnableMechanism[] _melodyButtonShort;

    [Header("TitleUI")]
    [SerializeField]
    private ActTitleTextController _actTitleTextController;

    [Header("Goons")]
    [SerializeField]
    private Goon _hag;
    [SerializeField]
    private Goon _toff;
    [SerializeField]
    private Goon _yorky;

    [Header("Points Manager")]
    [SerializeField]
    private PointsManager _pointsManager;

    public static Action ActFinished;
    public static Action ActStarted;
    public static Action<GameMode> ChangedGameMode;

    void Awake()
    {
        SetGameMode(GameMode.None);
        _actTitleTextController.gameObject.SetActive(false);

        if (_startingGameState == GameState.Title)
        {
            _gameCamera.SetActive(false);
            _projectorCamera.SetActive(true);
        }
        else
        {
            _gameCamera.SetActive(true);
            _projectorCamera.SetActive(false);
        }

        MusicButtonRunMechanism.MusicMechanismRunStateUpdate += MechanismRunStateCheck;
        ScrapButtonRunMechanism.ScrapMechanismRunStateUpdate += MechanismRunStateCheck;
    }

    private void Start()
    {
        if (_startingGameState == GameState.Title)
        {
            SetState(GameState.Title);
        }
        else
        {
            SetState(_startingGameState);
        }
    }

    private async void SetState(GameState state)
    {
        _currentGameState = state;

        switch (state) 
        { 
            case GameState.Title:

                _houseLights.SetActive(false);
                _goonLightsLeft.SetActive(false);

                _projector.EnableAfterAnimation();

                // When the video finishes, end the act
                ProjectorRunMechanism.VideoPlaybackComplete = () =>
                {
                    Debug.Log($"Next act is: {GetNextAct()}");
                    
                    _projector.DisableAfterAnimation();
                    FinishAct(nextAct: GameState.Act1);
                };

                break;
            case GameState.Act1:

                // Switch camera
                _gameCamera.SetActive(true);
                _projectorCamera.SetActive(false);

                await StartAct(GameState.Act1);

                // Goon walks on in darkness
                //_hag.GetComponent<GoonMove>().SetTargetPosition(_hagStagePosition1);

                // Wait a second before goon lights come on...
                StartCoroutine(PauseThenActivate(3.5f, _goonLightsLeft));

                _melodyButtonLong.EnableAfterAnimation();

                // Wait a second before house lights come on...
                StartCoroutine(PauseThenActivate(5, _houseLights));

                // If the crowd are entertained, finish the act
                CrowdController.CrowdEntertained = () => FinishAct(nextAct: GameState.Act2);

                break;
            case GameState.Act2:

                await StartAct(GameState.Act2);

                //_hag.GetComponent<GoonMove>().SetTargetPosition(_hagStagePosition1);

                _goonLightsLeft.SetActive(true);
                _houseLights.SetActive(true);

                foreach (MelodyButtonEnableMechanism button in _melodyButtonShort)
                {
                    button.EnableAfterAnimation();
                };

                CrowdController.CrowdEntertained = () => FinishAct(nextAct: GameState.Act3);

                break;
            case GameState.Act3:

                await StartAct(GameState.Act3);

                //_hag.GetComponent<GoonMove>().SetTargetPosition(_hagStagePosition1);
                //_toff.GetComponent<GoonMove>().SetTargetPosition(_toffStagePosition1);

                _goonLightsLeft.SetActive(true);
                _houseLights.SetActive(true);
                

                CrowdController.CrowdEntertained = () => FinishAct(nextAct: GameState.Act4);

                break;
            case GameState.Act4:

                await StartAct(GameState.Act4);

                //_hag.GetComponent<GoonMove>().SetTargetPosition(_hagStagePosition1);
                //_toff.GetComponent<GoonMove>().SetTargetPosition(_toffStagePosition1);
                
                _scrap.EnableAfterAnimation();
                _goonLightsLeft.SetActive(true);
                _houseLights.SetActive(true);

                CrowdController.CrowdEntertained = () => FinishAct(nextAct: GameState.Act5);

                break;
            case GameState.Act5:

                await StartAct(GameState.Act5);

                //_hag.GetComponent<GoonMove>().SetTargetPosition(_hagStagePosition2);
                //_toff.GetComponent<GoonMove>().SetTargetPosition(_toffStagePosition2);
                //_yorky.GetComponent<GoonMove>().SetTargetPosition(_yorkyStagePosition1);
                _goonLightsLeft.SetActive(true);
                _houseLights.SetActive(true);

                CrowdController.CrowdEntertained = () => FinishAct(nextAct: GameState.Sandbox);

                break;
            case GameState.Sandbox:

                await StartAct(GameState.Sandbox);

                _houseLights.SetActive(true);
                _goonLightsLeft.SetActive(true);
                _gameCamera.SetActive(true);
                _projectorCamera.SetActive(false);

                break;
        }
    }

    public IEnumerator PauseThenActivate(float seconds, GameObject objectToActivate)
    {
        yield return new WaitForSeconds(seconds);
        objectToActivate.SetActive(true);
    }

    async Task StartAct(GameState act)
    {
        // Set points requirements
        _pointsManager.SetupPointsData(act);
        
        // Title On
        _actTitleTextController.DisplayActTitle(act);
        _actTitleTextController.gameObject.SetActive(true);

        // Play sound, wait a few seconds.
        await Task.Delay(2000);

        // Curtain Opens
        foreach (CurtainEnableMechanism curtain in _curtains)
        {
            curtain.EnableAfterAnimation();
        }

        // Play sound, wait a few seconds.
        await Task.Delay(2000);

        // Title Off
        _actTitleTextController.gameObject.SetActive(false);

        ActStarted?.Invoke();

        await Task.Delay(500);

        Debug.Log($"Start of Act: {_currentGameState}");
    }

    public async void FinishAct(GameState nextAct)
    {
        // Wait for crowd react and goon bowing
        await Task.Delay(4000);

        // Start Curtain closing
        foreach (CurtainEnableMechanism curtain in _curtains)
        {
            curtain.DisableAfterAnimation();
        }
        
        // Go offstage
        await Task.Delay(1000);
        _hag.GetComponent<GoonMove>().GoonOffstage();
        _toff.GetComponent<GoonMove>().GoonOffstage();
        _yorky.GetComponent<GoonMove>().GoonOffstage();

        // Wait for the curtain animation to complete
        await WaitForEnableMechanismState(_curtains[0], BaseEnableMechanism.EnabledState.Disabled);

        Debug.Log($"End of Act: {_currentGameState}");

        // Turn off lights
        _goonLightsLeft.SetActive(false);
        await Task.Delay(200);
        _houseLights.SetActive(false);

        // calm the crowd, reset points
        ActFinished?.Invoke();

        // Set the next act
        SetState(nextAct);
    }

    private GameState GetNextAct()
    {
        return (GameState)((int)_currentGameState + 1);
    }

    async Task WaitForEnableMechanismState(BaseEnableMechanism mechanism, BaseEnableMechanism.EnabledState requiredState)
    {
        while (mechanism.GetState() != requiredState)
        {
            await Task.Delay(1000);
        }
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

                // Deactivate the Play button, ignore clicks
                /*
                if (_playButton.GetCurrentState() == ToggleMusic.ToggleState.WaitingForStop ||
                    _playButton.GetCurrentState() == ToggleMusic.ToggleState.RotateToStart)
                {
                    Debug.Log("Hide the music button");
                }
                */
                break;
            case GameMode.Music:
                // Deactivate the Scrap button, ignore clicks
                _music.EnableAfterAnimation();
                _scrap.DisableAfterAnimation();
                break;
            case GameMode.None:
                // Activate both the Play and Scrap buttons
                _music.EnableAfterAnimation();
                _scrap.EnableAfterAnimation();
                break;
        }

        _currentGameMode = newGameMode;
        ChangedGameMode?.Invoke(newGameMode);
    }
}
