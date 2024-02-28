using FMODUnity;
using System;
using UnityEngine;

public class ScrapButtonRunMechanism : BaseRunMechanism
{
    [Header("Triggered Mechanisms")]
    [SerializeField]
    private ActivateTrapDoor _trapDoor;
    [SerializeField]
    private ScrapGenerator _scrapGenerator;

    [Header("Light Setup")]
    [SerializeField]
    private Light _buttonLight;
    [SerializeField]
    private float _enabledIntesity = 0.5f;
    [SerializeField]
    private Color _enabledColour = new Color(0.7830189f, 0.7396523f, 0.4986205f);
    [SerializeField]
    private float _disabledIntesity = 0.4f;
    [SerializeField]
    private Color _disabledColour = new Color(0.3294118f, 0.2039216f, 0.1215686f);

    [Header("Audio Setup")]
    [SerializeField]
    private EventReference _mechanismRunStartEvent;
    [SerializeField]
    private EventReference _mechanismRunStopEvent;

    private FMOD.Studio.EventInstance _mechanismRunStartInstance;
    private FMOD.Studio.EventInstance _mechanismRunStopInstance;

    [Header("Animation Setup")]
    [SerializeField]
    private Animator _animator;

    public static Action<Type, bool> ScrapMechanismRunStateUpdate;

    void Awake()
    {
        SetupFMOD();
        
        _enableMechanism = this.GetComponent<ScrapButtonEnableMechanism>();
        _buttonLight.intensity = _disabledIntesity;
        _buttonLight.color = _disabledColour;
    }

    private void SetupFMOD()
    {
        _mechanismRunStartInstance = FMODUnity.RuntimeManager.CreateInstance(_mechanismRunStartEvent);
        _mechanismRunStopInstance = FMODUnity.RuntimeManager.CreateInstance(_mechanismRunStopEvent);
    }

    public override void StartMechanism()
    {
        base.StartMechanism();

        // lerp between light colours
        _buttonLight.intensity = _enabledIntesity;
        _buttonLight.color = _enabledColour;

        _animator.Play("PushIn");
        _mechanismRunStartInstance.start();

        _scrapGenerator.StartGenerator();
        _trapDoor.OpenTrapDoor();
        ScrapMechanismRunStateUpdate?.Invoke(this.GetType(), true);
    }

    public override void StopMechanism()
    {
        base.StopMechanism();

        // lerp between light colours
        _buttonLight.intensity = _disabledIntesity;
        _buttonLight.color = _disabledColour;

        _animator.Play("PopOut");
        _mechanismRunStopInstance.start();

        _scrapGenerator.ShutdownGenerator();
        _trapDoor.CloseTrapDoor();
        ScrapMechanismRunStateUpdate?.Invoke(this.GetType(), false);
    }

    public override void OnClickedTrigger()
    {
        base.OnClickedTrigger();

        if (_enableMechanism?.GetState() != BaseEnableMechanism.EnabledState.Enabled) return;

        if (_currentRunningState == RunningState.Shutdown)
        {
            StartMechanism();
            return;
        }

        if (_currentRunningState == RunningState.Running)
        {
            StopMechanism();
            return;
        }
    }
}