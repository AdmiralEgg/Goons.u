using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using static StageManager;

public class StageManager : MonoBehaviour
{
    [System.Serializable]
    public enum GameState { Title, Act1, Act2, Act3, Act4, Act5, Sandbox }

    [Header("Game State")]
    [SerializeField, ReadOnly]
    private GameState _currentGameState;
    [SerializeField]
    private GameState _startingGameState = GameState.Title;

    [SerializeField]
    private List<GameStateActData> _actData;
    [SerializeField]
    private StagePositionController _stagePositionController;

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

    [Header("Game Mode Manager")]
    [SerializeField]
    private GameModeManager _gameModeManager;

    public static Action ActFinished;
    public static Action ActStarted;

    void Awake()
    {
        _actTitleTextController.gameObject.SetActive(false);
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
        ActData actData = GetActData(state);

        switch (state) 
        { 
            case GameState.Title:

                _houseLights.SetActive(false);
                _goonLightsLeft.SetActive(false);

                _projector.EnableAfterAnimation();

                // When the video finishes, end the act
                ProjectorRunMechanism.VideoPlaybackComplete = () =>
                {
                    _projector.DisableAfterAnimation();
                    FinishAct(actData, addDelay: false);
                };

                break;
            case GameState.Act1:

                await StartAct(actData);

                // Wait a second before goon lights come on...
                StartCoroutine(PauseThenActivate(3.5f, _goonLightsLeft));

                // Wait a second before house lights come on...
                StartCoroutine(PauseThenActivate(5, _houseLights));

                // If the crowd are entertained, finish the act
                CrowdController.CrowdEntertained = () => FinishAct(actData);

                break;
            case GameState.Act2:

                await StartAct(actData);

                _goonLightsLeft.SetActive(true);
                _houseLights.SetActive(true);

                CrowdController.CrowdEntertained = () => FinishAct(actData);

                break;
            case GameState.Act3:

                await StartAct(actData);

                _goonLightsLeft.SetActive(true);
                _houseLights.SetActive(true);
                
                CrowdController.CrowdEntertained = () => FinishAct(actData);

                break;
            case GameState.Act4:

                await StartAct(actData);

                _goonLightsLeft.SetActive(true);
                _houseLights.SetActive(true);

                CrowdController.CrowdEntertained = () => FinishAct(actData);

                break;
            case GameState.Act5:

                await StartAct(actData);

                _goonLightsLeft.SetActive(true);
                _houseLights.SetActive(true);

                CrowdController.CrowdEntertained = () => FinishAct(actData);

                break;
            case GameState.Sandbox:

                await StartAct(actData);

                _houseLights.SetActive(true);
                _goonLightsLeft.SetActive(true);

                break;
        }
    }

    public IEnumerator PauseThenActivate(float seconds, GameObject objectToActivate)
    {
        yield return new WaitForSeconds(seconds);
        objectToActivate.SetActive(true);
    }

    async Task StartAct(ActData actData)
    {
        // Set points requirements
        _pointsManager.SetupPointsData(actData.Act);
        
        // Title On
        _actTitleTextController.DisplayActTitle(actData.ActTitle, actData.ActSubtitle);
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

        GoonsOnStage(actData);
        EnableMechanismsForAct(actData);
        ActivateGameModeManager(actData);

        Debug.Log($"Start of Act: {_currentGameState}");
    }

    public async void FinishAct(ActData actData, bool addDelay = true)
    {
        // Wait for crowd react and goon bowing
        if (addDelay)
        {
            await Task.Delay(4000);
        }
        
        // Start Curtain closing
        foreach (CurtainEnableMechanism curtain in _curtains)
        {
            curtain.DisableAfterAnimation();
        }

        // Go offstage
        if (addDelay)
        {
            await Task.Delay(1000);
        }

        GoonsOnStage(actData, allGoonsOffStage: true);

        // Wait for the curtain animation to complete
        await WaitForEnableMechanismState(_curtains[0], BaseEnableMechanism.EnabledState.Disabled);

        Debug.Log($"End of Act: {_currentGameState}");

        // Turn off lights
        _goonLightsLeft.SetActive(false);

        if (addDelay)
        {
            await Task.Delay(200);
        }

        _houseLights.SetActive(false);

        // calm the crowd, reset points
        ActFinished?.Invoke();

        // Set the next act
        SetState(actData.NextAct);
    }

    async Task WaitForEnableMechanismState(BaseEnableMechanism mechanism, BaseEnableMechanism.EnabledState requiredState)
    {
        while (mechanism.GetState() != requiredState)
        {
            await Task.Delay(1000);
        }
    }

    private ActData GetActData(GameState gameState)
    {
        return _actData.FirstOrDefault(d => d.GameState == gameState).ActData;
    }

    private void GoonsOnStage(ActData actData, bool allGoonsOffStage = false)
    {
        if (allGoonsOffStage == true)
        {
            _hag.GetComponent<GoonMove>().GoonOffstage();
            _toff.GetComponent<GoonMove>().GoonOffstage();
            _yorky.GetComponent<GoonMove>().GoonOffstage();
            return;
        }

        if (actData.HagEnabled == true)
        {
            StagePositionPoint stagePositionPoint = _stagePositionController.GetStagePositionPoint(actData.HagStagePosition);
            Debug.Log($"StagePosPoint Hag: {stagePositionPoint}");
            _hag.GetComponent<GoonMove>().SetTargetPosition(stagePositionPoint);
        }

        if (actData.ToffEnabled == true)
        {
            StagePositionPoint stagePositionPoint = _stagePositionController.GetStagePositionPoint(actData.ToffStagePosition);
            _toff.GetComponent<GoonMove>().SetTargetPosition(stagePositionPoint);
        }

        if (actData.YorkyEnabled == true)
        {
            StagePositionPoint stagePositionPoint = _stagePositionController.GetStagePositionPoint(actData.YorkyStagePosition);
            _yorky.GetComponent<GoonMove>().SetTargetPosition(stagePositionPoint);
        }
    }

    private void ActivateGameModeManager(ActData actData)
    {
        if (actData.ScrapEnabled && actData.MusicEnabled)
        {
            _gameModeManager.gameObject.SetActive(true);
        }
        else
        {
            _gameModeManager.gameObject.SetActive(false);
        }
    }

    private void EnableMechanismsForAct(ActData actData)
    {        
        if (actData.MusicEnabled)
        {
            _music.EnableAfterAnimation();
        }
        else
        {
            _music.DisableAfterAnimation();
        }

        if (actData.ScrapEnabled)
        {
            _scrap.EnableAfterAnimation();
        }
        else
        {
            _scrap.DisableAfterAnimation();
        }

        if (actData.MelodyLong1Enabled)
        {
            _melodyButtonLong.EnableAfterAnimation();
        }
        else
        {
            _melodyButtonLong.DisableAfterAnimation();
        }

        if (actData.MelodyShort1Enabled)
        {
            _melodyButtonShort[0].EnableAfterAnimation();
        }
        else
        {
            _melodyButtonShort[0].DisableAfterAnimation();
        }

        if (actData.MelodyShort2Enabled)
        {
            _melodyButtonShort[1].EnableAfterAnimation();
        }
        else
        {
            _melodyButtonShort[1].DisableAfterAnimation();
        }
    }
}

[Serializable]
public class GameStateActData
{
    public GameState GameState;
    public ActData ActData;
}