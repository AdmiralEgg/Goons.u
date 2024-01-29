using System;
using System.Collections;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Title,
        Act1,
        Act2,
        Act3,
        Act4,
        Act5,
        Sandbox
    }

    [Header("Game State")]
    [SerializeField, ReadOnly]
    private GameState _currentGameState;
    [SerializeField]
    private GameState _startingGameState = GameState.Title;

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
    private ScrapButtonEnableMechanism _scrap;
    [SerializeField]
    private MelodyButtonEnableMechanism _melodyButtonLong;
    [SerializeField]
    private MelodyButtonEnableMechanism[] _melodyButtonShort;
    [SerializeField]
    private GameObject[] _record;

    [Header("TitleUI")]
    [SerializeField]
    private ActTitleTextController _actTitleTextController;

    [Header("Goons")]
    [SerializeField]
    private Goon _hag;

    [Header("Act 1")]
    [SerializeField]
    private StagePositionPoint _hagStagePositionAct1;

    void Awake()
    {
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
                _hag.GetComponent<GoonMove>().SetTargetPosition(_hagStagePositionAct1);

                // Wait a second before goon lights come on...
                StartCoroutine(PauseThenActivate(3, _goonLightsLeft));

                // Wait a second before house lights come on...
                StartCoroutine(PauseThenActivate(5, _houseLights));

                _melodyButtonLong.EnableAfterAnimation();

                // If the crowd are entertained, finish the act
                CrowdController.CrowdEntertained = () => FinishAct(nextAct: GameState.Act2);

                break;
            case GameState.Act2:

                await StartAct(GameState.Act2);

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

                _goonLightsLeft.SetActive(true);
                _houseLights.SetActive(true);
                _scrap.EnableAfterAnimation();

                CrowdController.CrowdEntertained = () => FinishAct(nextAct: GameState.Act4);

                break;
            case GameState.Act4:

                await StartAct(GameState.Act4);

                _goonLightsLeft.SetActive(true);
                _houseLights.SetActive(true);

                CrowdController.CrowdEntertained = () => FinishAct(nextAct: GameState.Act5);

                break;
            case GameState.Act5:

                await StartAct(GameState.Act5);

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
        // Title On
        _actTitleTextController.DisplayActTitle(act);
        _actTitleTextController.gameObject.SetActive(true);

        // Play sound, wait a few seconds.
        await Task.Delay(4000);

        // Title Off
        _actTitleTextController.gameObject.SetActive(false);

        await Task.Delay(500);

        // Curtain Opens
        foreach (CurtainEnableMechanism curtain in _curtains)
        {
            curtain.EnableAfterAnimation();
        }

        // Wait for the curtain animation to complete
        await WaitForEnableMechanismState(_curtains[0], BaseEnableMechanism.EnabledState.Enabled);

        Debug.Log($"Start of Act: {_currentGameState}");
    }

    public async void FinishAct(GameState nextAct)
    {
        // Goon bowing

        // Start Curtain closing
        foreach (CurtainEnableMechanism curtain in _curtains)
        {
            curtain.DisableAfterAnimation();
        }

        // Wait for the curtain animation to complete
        await WaitForEnableMechanismState(_curtains[0], BaseEnableMechanism.EnabledState.Disabled);

        Debug.Log($"End of Act: {_currentGameState}");

        // Turn off lights
        _goonLightsLeft.SetActive(false);
        _houseLights.SetActive(false);

        // Set the next act
        SetState(nextAct);
    }

    async Task WaitForEnableMechanismState(BaseEnableMechanism mechanism, BaseEnableMechanism.EnabledState requiredState)
    {
        while (mechanism.GetState() != requiredState)
        {
            await Task.Delay(1000);
        }
    }
}
