using UnityEngine;

public class ScrapButtonRunMechanism : BaseRunMechanism
{
    [Header("Triggered Mechanisms")]
    [SerializeField]
    private GameObject[] _trapDoors;
    [SerializeField]
    private GameObject[] _generators;

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

    void Awake()
    {
        _buttonLight.intensity = _disabledIntesity;
        _buttonLight.color = _disabledColour;
    }

    public override void StartMechanism()
    {
        base.StartMechanism();

        // lerp between light colours
        _buttonLight.intensity = _enabledIntesity;
        _buttonLight.color = _enabledColour;

        foreach (GameObject generator in _generators)
        {
            generator.GetComponent<ScrapGenerator>().StartGenerator();
        }

        foreach (GameObject trapdoor in _trapDoors)
        {
            // Open the trapdoors
            trapdoor.GetComponent<ActivateTrapDoor>().OpenTrapDoor();
        }
    }

    public override void StopMechanism()
    {
        base.StopMechanism();

        // lerp between light colours
        _buttonLight.intensity = _disabledIntesity;
        _buttonLight.color = _disabledColour;

        foreach (GameObject generator in _generators)
        {
            generator.GetComponent<ScrapGenerator>().ShutdownGenerator();
        }

        foreach (GameObject trapdoor in _trapDoors)
        {
            // Close the trapdoors
            trapdoor.GetComponent<ActivateTrapDoor>().CloseTrapDoor();
        }
    }

    public override void OnClickedTrigger()
    {
        base.OnClickedTrigger();

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
