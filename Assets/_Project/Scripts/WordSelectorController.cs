using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class WordSelectorController : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private ButtonController _randomButtonController;
    [SerializeField, ReadOnly]
    private ButtonController _fixedButtonController;

    [SerializeField, ReadOnly]
    private ButtonController.ButtonType _selectedButtonType;

    void Awake()
    {
        foreach (ButtonController button in GetComponentsInChildren<ButtonController>())
        {
            if (button.GetCurrentButtonType() == ButtonController.ButtonType.Random)
            {
                _randomButtonController = button;
            }

            if (button.GetCurrentButtonType() == ButtonController.ButtonType.Fixed)
            {
                _fixedButtonController = button;
            }
        };

        OnButtonClicked(ButtonController.ButtonType.Random);
    }

    public void OnButtonClicked(ButtonController.ButtonType type)
    {
        _selectedButtonType = type;

        if (type == ButtonController.ButtonType.Random)
        {
            _randomButtonController.ButtonLightOn();
            _fixedButtonController.ButtonLightOff();
        }

        if (type == ButtonController.ButtonType.Fixed)
        {
            _randomButtonController.ButtonLightOff();
            _fixedButtonController.ButtonLightOn();
        }
    }

    public ButtonController.ButtonType GetSelectedButtonType()
    {
        return _selectedButtonType;
    }
}
