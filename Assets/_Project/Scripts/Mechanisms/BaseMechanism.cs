using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseMechanism : MonoBehaviour
{
    [Tooltip("Is the mechanism in the scene, and able to start running")]
    private enum EnabledState { Enabled, Disabled, InTransition }
    [Tooltip("Is the mechanism running")]
    private enum RunningState { Running, Shutdown }

    [SerializeField, ReadOnly]
    private EnabledState _currentEnabledState = EnabledState.Enabled;
    [SerializeField, ReadOnly]
    private RunningState _currentRunningState = RunningState.Shutdown;

    [SerializeField, Tooltip("Awake with the mechanism enabled")]
    private bool _isDisabledOnAwake = false;
    [SerializeField, Tooltip("On Start, transition to the Enabled state")]
    private bool _enableOnStart = true;
    [SerializeField, Tooltip("When enabled immediately start the mechanism")]
    private bool _onEnableStartMechanism = true;

    [Header("Position and rotations for disabling. Mechanisms start in their Enabled positions in the world.")]
    [SerializeField, ReadOnly, Tooltip("Enabled position is the inital gameobject position on Awake")]
    private Vector3 _enabledPosition;
    [SerializeField, ReadOnly, Tooltip("Enabled rotation is the inital gameobject roptation on Awake")]
    private Quaternion _enabledRotation;
    [SerializeField]
    private Vector3 _disabledPosition;
    [SerializeField]
    private Quaternion _disabledRotation;
    [SerializeField]
    private float _transitionDuration = 5f;

    [SerializeField, ReadOnly, Tooltip("Whether a position transition is needed to get into an Enabled state")]
    private bool _hasPositionTransition = false;
    [SerializeField, ReadOnly, Tooltip("Whether a rotation transition is needed to get into an Enabled state")]
    private bool _hasRotationTransition = false;

    /// <summary>
    /// Awake a mechanism in the disabled position by default.
    /// Always awake a mechanism Shutdown.
    /// </summary>
    void Awake()
    {
        if (_isDisabledOnAwake == true)
        {
            this.transform.position = _disabledPosition;
            this.transform.rotation = _disabledRotation;
            _currentEnabledState = EnabledState.Disabled;
        }

        _currentRunningState = RunningState.Shutdown;

        // Set the transition checks
        if (_disabledPosition != Vector3.zero || _disabledPosition != this.transform.position)
        {
            _hasPositionTransition = true;
        }
            
        if (_disabledRotation != Quaternion.identity & _disabledRotation != this.transform.rotation) 
        { 
            _hasRotationTransition = true;
        }
    }

    private void Start()
    {
        if (_enableOnStart)
        {
            this.gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        Debug.Log($"Mechanism: {gameObject.name} Enabled");
        StartCoroutine(StartEnabledTransition(EnabledState.Enabled));
    }

    public void DisableAfterAnimation()
    {
        Debug.Log($"Mechanism: {gameObject.name} Disabled");
        StartCoroutine(StartEnabledTransition(EnabledState.Disabled));
    }

    private IEnumerator StartEnabledTransition(EnabledState targetState)
    {
        if (_currentEnabledState == EnabledState.InTransition)
        {
            Debug.Log($"Mechanism {gameObject.name} already in transition. Will not start EnabledTransition coroutine.");
            yield break;
        }
        
        if (_hasPositionTransition == false && _hasRotationTransition == false) 
        {
            Debug.Log($"No position or rotation transitions to complete, setting mechanism {gameObject.name} to Enabled.");
            _currentEnabledState = EnabledState.Enabled;
            yield break;
        }

        Vector3 targetPosition;
        Quaternion targetRotation;

        // Set our destination positions and rotations. Enabled or Disabled.
        if (_currentEnabledState == EnabledState.Disabled) 
        {
            targetPosition = _enabledPosition;
            targetRotation = _enabledRotation;
        }
        else
        {
            targetPosition = _disabledPosition;
            targetRotation = _disabledRotation;
        }

        // Start transition lerping        
        _currentEnabledState = EnabledState.InTransition;

        float t = 0;
        Vector3 startPosition = this.transform.position;
        Quaternion startRotation = this.transform.rotation;

        while (t < _transitionDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, t / _transitionDuration);
            transform.rotation = Quaternion.RotateTowards(startRotation, targetRotation, t / _transitionDuration);

            t += Time.deltaTime;
            yield return null;
        }

        // Finish transition
        _currentEnabledState = targetState;

        if (targetState == EnabledState.Enabled && _onEnableStartMechanism == true)
        {
            StartMechanism();
        }

        if (targetState == EnabledState.Disabled)
        {
            this.gameObject.SetActive(false);
        }

        yield return null;
    }

    public void OnClickedTrigger()
    {
        if (_currentEnabledState != EnabledState.Enabled) return;

        Debug.Log($"Clicked mechanism: {gameObject.name}.");

        // Swap the state
        if (_currentRunningState == RunningState.Running) 
        {
            StopMechanism();
            return;
        }

        if (_currentRunningState == RunningState.Shutdown)
        {
            StartMechanism();
            return;
        }
    }

    public virtual void StartMechanism() 
    {
        Debug.Log($"Starting mechanism: {gameObject.name}.");
        _currentRunningState = RunningState.Running;
    }
    
    public virtual void StopMechanism() 
    {
        Debug.Log($"Shutting down mechanism: {gameObject.name}.");
        _currentRunningState = RunningState.Shutdown;
    }
}
