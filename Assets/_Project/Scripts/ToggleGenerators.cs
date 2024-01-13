using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ToggleGenerators : MonoBehaviour
{
    public enum ToggleState
    {
        Off,
        InTransition,
        On
    }
    
    [SerializeField]
    private GameObject[] _trapDoors;

    [SerializeField]
    private GameObject[] _generators;

    [SerializeField, ReadOnly]
    private ToggleState _currentState;

    private Light _buttonLight;

    private float _enabledIntesity = 0.5f;
    private Color _enabledColour = new Color(0.7830189f, 0.7396523f, 0.4986205f);

    private float _disabledIntesity = 0.4f;
    private Color _disabledColour = new Color(0.3294118f, 0.2039216f, 0.1215686f);

    void Awake()
    {
        _currentState = ToggleState.Off;

        _buttonLight = this.GetComponentInChildren<Light>();

        _buttonLight.intensity = _disabledIntesity;
        _buttonLight.color = _disabledColour;
    }

    private void OnClickedTrigger()
    {
        Debug.Log("Recieved click");
        
        if (_currentState == ToggleState.InTransition) return;

        if (_currentState == ToggleState.On)
        {
            Debug.Log("Stopping coroutines");
            
            StopAllCoroutines();
            _currentState = ToggleState.InTransition;
            StartCoroutine(StoppingScrapGeneration());
            return;
        }

        if (_currentState == ToggleState.Off)
        {
            Debug.Log("Starting coroutines");

            StopAllCoroutines();
            _currentState = ToggleState.InTransition;
            StartCoroutine(StartingScrapGeneration());
            return;
        }
    }

    private IEnumerator StartingScrapGeneration()
    {
        Debug.Log("Scraps switch has got a click! Enabling scraps...");

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

        _currentState = ToggleState.On;
        yield return null;
    }

    private IEnumerator StoppingScrapGeneration()
    {
        Debug.Log("Scraps switch has got a click! Disabling scraps...");

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

        _currentState = ToggleState.Off;
        yield return null;
    }
}
