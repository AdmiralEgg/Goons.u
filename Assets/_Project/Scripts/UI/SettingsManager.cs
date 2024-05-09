using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Responsible for adjusting settings based on user changes in the Settings Modal
/// </summary>
public class SettingsManager : MonoBehaviour
{
    [SerializeField, ReadOnly]
    string _currentRes;
    
    [SerializeField, ReadOnly]
    string _currentRefreshRate;

    [SerializeField, ReadOnly]
    List<string> _supportedResolutions;

    [SerializeField, ReadOnly]
    VisualElement _rootVisualElement;

    List<Resolution> _modernResolutions;
    List<Resolution> _classicResolutions;

    private void Start()
    {
        _currentRes = Screen.currentResolution.ToString();
        _currentRefreshRate = Screen.currentResolution.refreshRateRatio.ToString();
        _rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        _modernResolutions = new List<Resolution>(Screen.resolutions);

        DropdownField resolutionSelector = _rootVisualElement.Q<DropdownField>("ResolutionSelector");

        foreach (Resolution res in _modernResolutions)
        {
            _supportedResolutions.Add(res.ToString());
            resolutionSelector.choices.Add($"{res.width} x {res.height}");
        }

        resolutionSelector.RegisterValueChangedCallback(OnValueChange);
    }

    private void OnValueChange(ChangeEvent<string> evt)
    {
        Debug.Log("Value change!" + evt);
    }
}
