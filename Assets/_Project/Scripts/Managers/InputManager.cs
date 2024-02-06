using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public enum InputState { Free, ScrapSelected }
    
    [Header("Game States")]
    [SerializeField, ReadOnly]
    private InputState _currentInputState;
    [SerializeField, ReadOnly]
    private GameModeManager.GameMode _currentGameMode;
    [SerializeField, ReadOnly]
    private Scrap _currentSelectedScrap;
    [SerializeField]
    private GameModeManager _gameModeManager;

    private static InputState s_currentInputState;
    public static InputManager s_instance { get; private set; }
    public static PlayerInput s_playerInput { get; private set; }

    public static Action<GameObject> ReportHit;
    public static Action<InputState> ChangedInputState;
    public static Action<WordData> InventoryScrapClicked;

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

        // If scrap has been selected, update the state.
        Scrap.ScrapSelected += (scrap) =>
        {
            UpdateInputState(InputState.ScrapSelected, scrap);
        };

        GameModeManager.ChangedGameMode += CheckGameMode;
        _currentGameMode = GameModeManager.GameMode.None;
    }

    private void Update()
    {
        // Show statics in the inspector. 
        // TODO: Sort this into a best practice pattern
        _currentInputState = s_currentInputState;
    }

    private void CheckGameMode(GameModeManager.GameMode gameMode)
    {
        Debug.Log("Input manager checking game mode");
        _currentGameMode = gameMode;
        
        if (gameMode == GameModeManager.GameMode.Music || gameMode == GameModeManager.GameMode.None)
        {
            UpdateInputState(InputState.Free);
        }
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
        LayerMask mask = LayerMask.GetMask(new string[] { "Goon", "UI", "Crowd" } );
        Physics.Raycast(clickPositionRay, out RaycastHit hit, float.PositiveInfinity, mask);

        if (hit.collider == null) return;

        ReportHit?.Invoke(hit.collider.gameObject);

        if (hit.collider.gameObject.tag == "Scrap")
        {
            if (_currentGameMode == GameModeManager.GameMode.Music || _currentGameMode == GameModeManager.GameMode.None)
            {
                // Send message to all goons that scrap has been clicked.
                // Play animation on the scrap.
                Scrap scrap = hit.collider.gameObject.GetComponent<Scrap>();
                WordData wordData = scrap.GetWordData();

                Debug.Log("Play the scrap through the goon");
                InventoryScrapClicked?.Invoke(wordData);
                scrap.PlayProdAnimation();
                return;
            }

            if (_currentGameMode == GameModeManager.GameMode.Scrap)
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
            hit.collider.SendMessageUpwards("OnClickedTrigger", hit.collider.gameObject);
        }

        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Crowd"))
        {
            hit.collider.SendMessageUpwards("OnClickedTrigger", hit.collider.gameObject);
        }
    }

    private void OnScrapSelectedAction(InputAction.CallbackContext context, Ray clickPositionRay)
    {
        // If a UI element is selected, check it's a Goon UI element and attack scrap
        // If another scrap is selected, switch to that and stay in selected mode
        LayerMask mask = LayerMask.GetMask(new string[] { "UI" });
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
            // Select a scrap
            Debug.Log("Clicked a scrap, switch selected to that one");

            UpdateInputState(InputState.Free);
            Scrap newScrap = hit.collider.gameObject.GetComponent<Scrap>();
            UpdateInputState(InputState.ScrapSelected, newScrap);
            return;
        }

        if (hit.collider.tag == "ScrapDelete")
        {
            Debug.Log("Clicked the hole with scrap selected. Burn the scrap!");
            GameObject _scrapToDestroy = _currentSelectedScrap.gameObject;
            UpdateInputState(InputState.Free);
            Destroy(_scrapToDestroy);
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
}
