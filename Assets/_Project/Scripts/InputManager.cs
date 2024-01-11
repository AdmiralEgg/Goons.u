using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager s_instance { get; private set; }

    public static PlayerInput s_playerInput { get; private set; }

    void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (s_instance != null && s_instance != this)
        {
            Destroy(this);
        }
        else
        {
            s_instance = this;
        }

        s_playerInput = this.GetComponent<PlayerInput>();

        s_playerInput.actions["Select"].performed += OnSelect;
    }

    private void OnSelect(InputAction.CallbackContext context)
    {
        Vector2 selectPosition = s_playerInput.actions["Position"].ReadValue<Vector2>();

        if (selectPosition == null) return;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(selectPosition.x, selectPosition.y, 0));

        LayerMask mask = LayerMask.GetMask(new string[] { "Goon", "UI"} );
        Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, mask);

        if (hit.collider == null) return;

        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Goon"))
        {
            hit.collider.SendMessageUpwards("OnGoonSelected", hit.collider.gameObject);
        }

        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("UI"))
        {
            hit.collider.SendMessageUpwards("OnToggleMusic", hit.collider.gameObject);
        }
    }

}
