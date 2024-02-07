using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoonEyeController : MonoBehaviour
{
    private enum IdleAction { FocusCamera, FocusLeft, FocusRight, FocusStraight, FocusCeiling, RollEyes, CrossEyes, Think, Shifty, PickCrowd, ScanCrowd }
    
    private enum EyeState { Idle, Focusing }

    [SerializeField, ReadOnly]
    private EyeState _currentEyeState;
    [SerializeField, ReadOnly]
    private IdleAction _currentIdleAction;

    [SerializeField]
    private Goon _goon;

    [SerializeField]
    private GameObject[] _eyes;

    [Title("Fixed Point Focus")]
    [SerializeField]
    private Transform _focusLeft, _focusStraight, _focusRight, _focusCeiling;

    [Title("Scanning Crowd")]
    [SerializeField]
    private Transform _crowdLeft, _crowdRight;

    [Title("Randomly selected crowd member")]
    [SerializeField, ReadOnly]
    private CrowdMember _crowdMember;

    [Title("All Crowd Members")]
    [SerializeField]
    private GameObject _allCrowd;

    [SerializeField]
    private float _scanCrowdSpeed = 0.2f, _quickLookSpeed = 0.7f, _slowLookSpeed = 0.3f;

    [SerializeField, ReadOnly]
    private Transform _currentFocusTransform;

    [SerializeField, ReadOnly]
    private float _currentFocusSpeed;

    [SerializeField]
    private float _timeToNextIdleActionMin = 3f;
    private float _timeToNextIdleActionMax = 7f;
    private float _timeToNextIdleAction = 3f;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.enabled = false;

        _currentEyeState = EyeState.Idle;
    }

    private void Update()
    {
        // start coroutines to pay actions
        if (_timeToNextIdleAction > 0)
        {
            _timeToNextIdleAction -= Time.deltaTime;
        }
        else
        {
            int idleAction = UnityEngine.Random.Range(0, Enum.GetNames(typeof(IdleAction)).Length);
            
            Array actions = Enum.GetValues(typeof(IdleAction));

            IdleAction randomAction = (IdleAction)actions.GetValue(idleAction);
            PlayIdleAction(randomAction);
            
            _timeToNextIdleAction = UnityEngine.Random.Range(_timeToNextIdleActionMin, _timeToNextIdleActionMax);
        }
    }

    private void PlayIdleAction(IdleAction action)
    {
        _animator.enabled = false;
        _currentEyeState = EyeState.Focusing;
        _currentIdleAction = action;

        switch (action)
        {
            case IdleAction.FocusCamera:
                SetFocusPoint(Camera.main.transform);
                break;
            case IdleAction.FocusLeft:
                SetFocusPoint(_focusLeft);
                break;
            case IdleAction.FocusRight:
                SetFocusPoint(_focusRight);
                break;
            case IdleAction.FocusStraight:
                SetFocusPoint(_focusStraight);
                break;
            case IdleAction.FocusCeiling:
                SetFocusPoint(_focusCeiling);
                break;
            case IdleAction.RollEyes:
                PlayAnimation("RollEyes");
                break;
            case IdleAction.CrossEyes:
                PlayAnimation("CrossEyes");
                break;
            case IdleAction.Think:
                PlayAnimation("Thinking");
                break;
            case IdleAction.Shifty:
                PlayAnimation("Shifty");
                break;
            case IdleAction.PickCrowd:
                CrowdMember crowdMember = PickRandomCrowdMember();
                SetFocusPoint(crowdMember.transform);
                break;
            case IdleAction.ScanCrowd:
                StartCoroutine(ScanCrowd());
                break;
        }
    }

    private void SetFocusPoint(Transform focusPoint, float focusSpeed = 0f)
    {
        // Randomise speed
        if (focusSpeed == 0f)
        {
            var speeds = new List<float> { _quickLookSpeed, _slowLookSpeed };
            var random = new System.Random();
            int index = random.Next(speeds.Count);
            focusSpeed = speeds[index];
        }

        _currentFocusTransform = focusPoint;
        _currentFocusSpeed = focusSpeed;
    }

    private void PlayAnimation(string animationName)
    {        
        _animator.enabled = true;

        try
        {
            _animator.Play(animationName);
        }
        catch
        {
            Debug.Log($"Tried to play animation {animationName} and failed");
        }
    }

    private CrowdMember PickRandomCrowdMember()
    {
        CrowdMember[] allCrowdMembers = _allCrowd.GetComponentsInChildren<CrowdMember>();

        int crowdMember = UnityEngine.Random.Range(0, allCrowdMembers.Length);

        return allCrowdMembers[crowdMember];
    }

    private IEnumerator TrackObjectOverTime(GameObject gameObject, float timeInSeconds)
    {
        // Lock to object, update every frame
        // stop locking if timeout, or object disappears

        yield return null;
    }

    private IEnumerator ScanCrowd()
    {
        // Look crowd right
        SetFocusPoint(_crowdRight.transform, _quickLookSpeed);

        // Wait until look is complete
        while (_currentEyeState != EyeState.Idle)
        {
            yield return new WaitForSeconds(0.5f);
        }

        _currentEyeState = EyeState.Focusing;

        // Scan from right to left
        SetFocusPoint(_crowdLeft.transform, _scanCrowdSpeed);

        // Wait until look is complete
        while (_currentEyeState != EyeState.Idle)
        {
            yield return new WaitForSeconds(0.5f);
        }

        yield return null;
    }

    private void LateUpdate()
    {
        if (_currentFocusTransform == null) return;
        if (_currentEyeState == EyeState.Idle) return;

        foreach (GameObject eye in _eyes)
        {
            Vector3 direction = _currentFocusTransform.position - eye.transform.position;
            eye.transform.rotation = Quaternion.RotateTowards(eye.transform.rotation, Quaternion.LookRotation(direction), _currentFocusSpeed);

            // if either eye finishes, set the state back to Idle.
            if (eye.transform.rotation == Quaternion.LookRotation(direction))
            {
                _currentEyeState = EyeState.Idle;
            }
        }
    }
}
