using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class InputManager : MonoBehaviour
{
    public enum InputState { Free, ScrapSelected }
    
    [Header("Game States")]
    [SerializeField, ReadOnly]
    private InputState _currentInputState;

    [SerializeField, ReadOnly]
    private Scrap _currentSelectedScrap;
    [SerializeField]
    private GameModeManager _gameModeManager;

    [SerializeField, AssetsOnly]
    private GameObject _puffOfDust;

    private static InputState s_currentInputState;
    public static InputManager s_instance { get; private set; }
    public static PlayerInput s_playerInput { get; private set; }

    public static Action<GameObject> ReportHit;
    public static Action<InputState> ChangedInputState;
    public static Action<WordData> InventoryScrapClicked;
    public static Action<string> EnvironmentTouch;

    void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (s_instance != null && s_instance != this)
        {
            Destroy(this);
        }
        else
        {
            s_instance = this;
        }

        s_playerInput = this.GetComponent<PlayerInput>();

        // On click, figure out what has been hit. Nothing, or scrap.
        s_playerInput.actions["Select"].performed += OnPlayerSelect;
        s_playerInput.actions["Exit"].performed += OnExit;

        // If scrap has been selected, update the state.
        Scrap.ScrapSelected += (scrap) =>
        {
            UpdateInputState(InputState.ScrapSelected, scrap);
        };

        GameModeManager.ChangedGameMode += (gameMode) =>
        {
            if (gameMode == GameModeManager.GameMode.Music || gameMode == GameModeManager.GameMode.None)
            {
                UpdateInputState(InputState.Free);
            }
        };
    }

    private void Update()
    {
        // Show statics in the inspector.
        // TODO: Sort this into a best practice pattern
        _currentInputState = s_currentInputState;
    }

    private void OnPlayerSelect(InputAction.CallbackContext context)
    {
        Vector2 clickPosition = s_playerInput.actions["Position"].ReadValue<Vector2>();
        if (clickPosition == null) return;

        Ray clickPositionRay = Camera.main.ScreenPointToRay(new Vector3(clickPosition.x, clickPosition.y, 0));

        switch (s_currentInputState)
        {
            case InputState.Free:
                OnFreeAction(context, clickPositionRay);
                break;
            case InputState.ScrapSelected:
                OnScrapSelectedAction(context, clickPositionRay);
                break;
        }
    }

    private void OnFreeAction(InputAction.CallbackContext context, Ray clickPositionRay)
    {
        LayerMask mask = LayerMask.GetMask(new string[] { "Goon", "UI", "Crowd", "Environment" } );
        Physics.Raycast(clickPositionRay, out RaycastHit hit, float.PositiveInfinity, mask);

        if (hit.collider == null) return;

        ReportHit?.Invoke(hit.collider.gameObject);

        if (hit.collider.gameObject.tag == "Scrap")
        {
            GameModeManager.GameMode currentMode = _gameModeManager.CurrentGameMode;

            if (currentMode == GameModeManager.GameMode.Music || currentMode == GameModeManager.GameMode.None)
            {
                Scrap scrap = hit.collider.gameObject.GetComponentInParent<Scrap>();

                if (scrap == null) 
                {
                    Debug.Log($"Could not find Scrap component in parents of {hit.collider.name}");
                    return;
                }

                WordData wordData = scrap.GetWordData();
                InventoryScrapClicked?.Invoke(wordData);
                scrap.PlayProdAnimation();
                return;
            }

            if (currentMode == GameModeManager.GameMode.Scrap)
            {
                hit.collider.SendMessageUpwards("OnClickedTrigger", hit.collider.gameObject);
                return;
            }
        }

        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Goon"))
        {
            hit.collider.SendMessageUpwards("OnGoonSelected", hit.collider.gameObject);
        }

        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("UI"))
        {
            hit.collider.SendMessageUpwards("OnClickedTrigger", hit.collider.gameObject, SendMessageOptions.DontRequireReceiver);
        }

        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Crowd"))
        {
            hit.collider.SendMessageUpwards("OnClickedTrigger", hit.collider.gameObject);
            EnvironmentTouch?.Invoke("Crowd");
        }

        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            if (hit.collider.tag != "Pole")
            {
                hit.collider.SendMessageUpwards("OnClickedTrigger", hit.collider.gameObject, SendMessageOptions.DontRequireReceiver);
            }
            
            EnvironmentTouch?.Invoke(hit.collider.tag);
            
            if (hit.collider.tag == "Stage")
            {
                var puff = Instantiate(_puffOfDust);
                puff.transform.position = hit.point;
                puff.transform.Rotate(hit.normal);
            }
        }
    }

    private void OnScrapSelectedAction(InputAction.CallbackContext context, Ray clickPositionRay)
    {
        // If a UI element is selected, check it's a Goon UI element and attack scrap
        // If another scrap is selected, switch to that and stay in selected mode
        LayerMask mask = LayerMask.GetMask(new string[] { "UI", "ScrapDeleteTrigger" });
        Physics.Raycast(clickPositionRay, out RaycastHit hit, float.PositiveInfinity, mask);

        if (hit.collider == null)
        {
            // Go back to free mode and turn off selection lights
            UpdateInputState(InputState.Free);
            return;
        }

        ReportHit?.Invoke(hit.collider.gameObject);

        if (hit.collider.gameObject.GetComponent<Scrap>())
        {
            UpdateInputState(InputState.Free);
            Scrap newScrap = hit.collider.gameObject.GetComponent<Scrap>();
            UpdateInputState(InputState.ScrapSelected, newScrap);
            return;
        }

        if (hit.collider.tag == "ScrapDelete")
        {
            _currentSelectedScrap.SetScrapAttachedState(Scrap.ScrapAttachedState.None);
            _currentSelectedScrap.transform.position = hit.point;
            _currentSelectedScrap.gameObject.transform.parent = null;
            UpdateInputState(InputState.Free);
            return;
        }

        UpdateInputState(InputState.Free);
    }

    private void UpdateInputState(InputState newState, Scrap newScrap = null)
    {
        ChangedInputState?.Invoke(newState);
        
        if (newState == InputState.Free)
        {
            if (_currentSelectedScrap != null)
            {
                _currentSelectedScrap.SetScrapState(Scrap.ScrapState.Free);
            }
            
            _currentSelectedScrap = null;
        }

        if (newState == InputState.ScrapSelected)
        {
            newScrap.SetScrapState(Scrap.ScrapState.Selected);
            _currentSelectedScrap = newScrap;
        }

        // TODO: This is rubbish, fix it
        _currentInputState = newState;
        s_currentInputState = newState;
    }

    public static InputState GetCurrentInputState() 
    {
        return s_currentInputState;
    }

    private void OnExit(InputAction.CallbackContext context)
    {
        Application.Quit();
    }
}
