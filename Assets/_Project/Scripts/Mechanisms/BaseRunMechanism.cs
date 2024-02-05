using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BaseRunMechanism : MonoBehaviour
{
    protected enum RunningState { Running, TransitionToShutdown, Shutdown, TransitionToRunning }

    [SerializeField]
    protected RunningState _currentRunningState;

    [SerializeField, Tooltip("If set a check will be made for EnableMechanism.Enabled before triggering the mechanism.")]
    protected BaseEnableMechanism _enableMechanism;

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

    public virtual void OnClickedTrigger()
    {
        Debug.Log($"Clicked mechanism: {gameObject.name}.");
    }
}
