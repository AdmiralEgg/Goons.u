using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoonEyeController : MonoBehaviour
{
    
    
    [SerializeField]
    private GameObject _leftEye, _rightEye;

    [Header("Fixed Point Focus")]
    [SerializeField]
    private Transform _focusLeft, _focusStraight, _focusRight, _focusCeiling;

    private Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        
        FocusCeiling();
    }

    private void FocusLeft()
    {
        _leftEye.transform.LookAt(_focusLeft.position);
        _rightEye.transform.LookAt(_focusLeft.position);
    }

    private void FocusCeiling()
    {
        _leftEye.transform.LookAt(_focusCeiling.position);
        _rightEye.transform.LookAt(_focusCeiling.position);
    }

    // SnapLook, SlowLook
    // Blink
    // Roll (animation)
    // track point over time
    // Idle scanning
}
