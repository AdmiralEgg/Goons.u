using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ProjectorController : MonoBehaviour
{
    private enum ButtonState { On, Moving, Off };

    [SerializeField]
    private bool _startEnabled = false;

    [SerializeField, ReadOnly]
    private ButtonState _currentButtonState;

    void Start()
    {
        this.gameObject.SetActive(_startEnabled);
    }
}
