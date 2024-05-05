using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class CrowdDebug : MonoBehaviour
{
    [SerializeField]
    private StageManager _stageManager;

    [SerializeField]
    private int _debugTouches;

    [SerializeField]
    private float _debugTimer;

    private Coroutine _debugCountdown;

    public static Action SwitchSandbox;

    private void Awake()
    {
        _debugTimer = 0;
    }

    public void CrowdDebugPressed()
    {
        Debug.Log("pressed!");
        
        _debugTouches += 1;

        // if timer hasn't started, start it.
        if (_debugTimer == 0)
        {
            _debugCountdown = StartCoroutine(DebugCountdown());
        }

        if (_debugTouches > 10)
        {
            SwitchToSandbox();
            StopCoroutine(_debugCountdown);
            _debugTouches = 0;
        }
    }

    // Reset debug touches after 5 seconds
    public IEnumerator DebugCountdown()
    {
        _debugTimer = 5f;

        while (_debugTimer > 0)
        {
            yield return null;
            _debugTimer -= Time.deltaTime;
        }

        _debugTouches = 0;
        _debugTimer = 0;
    }

    public void SwitchToSandbox()
    {
        Debug.Log("Switching to Sandbox mode!");
        SwitchSandbox?.Invoke();
    }
}