using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseEnableMechanism : MonoBehaviour
{
    [Tooltip("Is the mechanism in the scene, and able to start running")]
    public enum EnabledState { Enabled, Disabled, InTransition }
    [Tooltip("How does the mechanism move between enabled states")]
    public enum EnableMovement { Position, Hinge }

    [Header("Current States")]
    [SerializeField, ReadOnly]
    private EnabledState _currentEnabledState;

    public EnabledState CurrentEnabledState
    {
        get { return _currentEnabledState; }
        private set { _currentEnabledState = value; }
    }

    [SerializeField]
    private Coroutine _stateTransitionCoroutine;

    [SerializeField, ReadOnly, Tooltip("Whether GameObject has a RunMechanism component")]
    private BaseRunMechanism _runMechanism;

    [Header("Movement Setup")]
    [SerializeField]
    private EnableMovement _enableMovement = EnableMovement.Position;
    [SerializeField, ShowIf("_enableMovement", EnableMovement.Hinge)]
    private HingeJoint _hinge;

    [Header("Action Setup")]
    [SerializeField, Tooltip("Awake in the disabled position, in the Disabled state")]
    private EnabledState _onAwakeState = EnabledState.Disabled;
    [SerializeField, Tooltip("On Start, transition to the Enabled state")]
    private EnabledState _onStartTransitionTo = EnabledState.Enabled;
    [SerializeField, Tooltip("When enabled immediately start the mechanism")]
    private bool _onEnableStartMechanism = false;
    [SerializeField, Tooltip("When disabled, deactivate the game object")]
    private bool _onDisableDeactivateGameObject = false;

    [Header("Position and rotations for disabling. Mechanisms start in their Enabled positions in the world.")]
    [ShowIf("_enableMovement", EnableMovement.Position), SerializeField, ReadOnly, Tooltip("Enabled position is the inital gameobject position on Awake")]
    private Vector3 _enabledPosition;
    [ShowIf("_enableMovement", EnableMovement.Position), SerializeField, ReadOnly, Tooltip("Enabled rotation is the inital gameobject rotation on Awake")]
    private Quaternion _enabledRotation;
    [ShowIf("_enableMovement", EnableMovement.Position), SerializeField]
    private Vector3 _disabledWorldPosition;
    [ShowIf("_enableMovement", EnableMovement.Position), SerializeField]
    private Quaternion _disabledWorldRotation;
    [ShowIf("_enableMovement", EnableMovement.Position), SerializeField]
    private float _transitionDuration = 5f;

    /// <summary>
    /// Awake a mechanism in the disabled position by default.
    /// Always awake a mechanism Shutdown.
    /// </summary>
    public virtual void Awake()
    {
        if (_enableMovement == EnableMovement.Position)
        {
            _enabledPosition = this.transform.position;
            _enabledRotation = this.transform.rotation;

            if (_onAwakeState == EnabledState.Disabled)
            {
                this.transform.position = _disabledWorldPosition;
                this.transform.rotation = _disabledWorldRotation;
                CurrentEnabledState = EnabledState.Disabled;
            }
        }

        if (_enableMovement == EnableMovement.Hinge)
        {
            if (_onAwakeState == EnabledState.Disabled)
            {
                SetHingeMotorState(true);
            }
            else
            {
                SetHingeMotorState(false);
            }
        }

        _runMechanism = GetComponent<BaseRunMechanism>();
    }

    private void Start()
    {
        if ((_onStartTransitionTo == EnabledState.Disabled) && (CurrentEnabledState != EnabledState.Disabled))
        {
            DisableAfterAnimation();
            return;
        }

        /*
        if ((_onStartTransitionTo) == EnabledState.Enabled && (CurrentEnabledState != EnabledState.Enabled))
        {
            EnableAfterAnimation();
            return;
        }
        */
    }

    public virtual void EnableAfterAnimation()
    {
        if (_enableMovement == EnableMovement.Position)
        {
            if (_stateTransitionCoroutine != null)
            {
                Debug.Log("Stopping transition coroutine");
                StopCoroutine(_stateTransitionCoroutine);
            }

            _stateTransitionCoroutine = StartCoroutine(StartEnabledTransition(EnabledState.Enabled));
        }

        if (_enableMovement == EnableMovement.Hinge)
        {
            Debug.Log("Enable with Hinge Motor");
            SetHingeMotorState(false);

            // Finish transition
            CurrentEnabledState = EnabledState.Enabled;

            if (_onEnableStartMechanism == true && _runMechanism != null)
            {
                _runMechanism.StartMechanism();
            }
        }
    }

    public virtual void DisableAfterAnimation()
    {
        if (_enableMovement == EnableMovement.Position)
        {
            if (_stateTransitionCoroutine != null)
            {
                Debug.Log("Stopping the disable transition coroutine");
                StopCoroutine(_stateTransitionCoroutine);
            }

            _stateTransitionCoroutine = StartCoroutine(StartEnabledTransition(EnabledState.Disabled));
        }

        if (_enableMovement == EnableMovement.Hinge)
        {
            Debug.Log("Disable with Hinge Motor");
            SetHingeMotorState(true);
            
            // Finish transition
            CurrentEnabledState = EnabledState.Disabled;

            if (_onDisableDeactivateGameObject == true)
            {
                this.gameObject.SetActive(false);
            }
        }
    }

    private void SetHingeMotorState(bool started)
    {
        _hinge.useMotor = started;
    }

    private IEnumerator StartEnabledTransition(EnabledState targetState)
    {
        if (CurrentEnabledState == targetState)
        {
            yield break;
        }

        Vector3 targetPosition;
        Quaternion targetRotation;

        // Set our destination positions and rotations. Enabled or Disabled.
        if (targetState == EnabledState.Enabled)
        {
            targetPosition = _enabledPosition;
            targetRotation = _enabledRotation;
        }
        else
        {
            targetPosition = _disabledWorldPosition;
            targetRotation = _disabledWorldRotation;
        }

        // Start transition lerping        
        CurrentEnabledState = EnabledState.InTransition;

        float t = 0;
        Vector3 startPosition = this.transform.position;
        Quaternion startRotation = this.transform.rotation;

        Vector3 smoothdampVelocity = Vector3.zero;
        float smoothTime = 1.5f;

        while (Vector3.Distance(transform.position, targetPosition) > 0.04f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref smoothdampVelocity, smoothTime);

            transform.rotation = Quaternion.RotateTowards(startRotation, targetRotation, t / _transitionDuration);

            t += Time.deltaTime;
            yield return null;
        }

        // Finish transition
        CurrentEnabledState = targetState;

        if ((targetState == EnabledState.Enabled) && (_onEnableStartMechanism == true) && (_runMechanism != null))
        {
            _runMechanism.StartMechanism();
        }

        if (targetState == EnabledState.Disabled && _onDisableDeactivateGameObject == true)
        {
            CurrentEnabledState = EnabledState.Disabled;
            this.gameObject.SetActive(false);
        }

        yield return null;
    }
}