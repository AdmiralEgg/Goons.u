using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public enum ButtonType { Random, Fixed };
    
    [SerializeField, ReadOnly]
    private Material _buttonMaterial;

    [SerializeField, ReadOnly]
    private Color _buttonColor = Color.white; 

    [SerializeField]
    public ButtonType CurrentButtonType;

    void Awake()
    {
        _buttonMaterial = GetComponent<MeshRenderer>().material;
        _buttonMaterial.SetColor("_EmissionColor", _buttonColor);

        ButtonLightOff();
    }

    public void ButtonLightOn()
    {
        _buttonMaterial.EnableKeyword("_EMISSION");
    }

    public void ButtonLightOff()
    {
        _buttonMaterial.DisableKeyword("_EMISSION");
    }

    void OnClickedTrigger()
    {
        SendMessageUpwards("OnButtonClicked", CurrentButtonType);
    }

    public ButtonType GetCurrentButtonType()
    {
        return CurrentButtonType;
    }
}
