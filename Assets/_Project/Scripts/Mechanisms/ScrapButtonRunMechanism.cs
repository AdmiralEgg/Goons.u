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

    [Header("Button Setup")]
    [SerializeField]
    private AudioClip _mechanismRunStartClip;
    [SerializeField]
    private AudioClip _mechanismRunStopClip;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private AudioSource _startStopSource;

    public static Action<Type, bool> ScrapMechanismRunStateUpdate;

    void Awake()
    {
        _enableMechanism = this.GetComponent<ScrapButtonEnableMechanism>();
        _buttonLight.intensity = _disabledIntesity;
        _buttonLight.color = _disabledColour;
    }

    public override void StartMechanism()
    {
        base.StartMechanism();

        // lerp between light colours
        _buttonLight.intensity = _enabledIntesity;
        _buttonLight.color = _enabledColour;

        _animator.Play("PushIn");
        _startStopSource.PlayOneShot(_mechanismRunStopClip);

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
        _startStopSource.PlayOneShot(_mechanismRunStopClip);

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
