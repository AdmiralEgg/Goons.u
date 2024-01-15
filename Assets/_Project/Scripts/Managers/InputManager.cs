using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public enum InputState
    {
        Free,
        ScrapSelected
    }

    [SerializeField, ReadOnly]
    private InputState _currentInputState;
    
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

        s_playerInput.actions["Select"].performed += (InputAction.CallbackContext context) =>
        {
            Vector2 clickPosition = s_playerInput.actions["Position"].ReadValue<Vector2>();

            Ray clickPositionRay = Camera.main.ScreenPointToRay(new Vector3(clickPosition.x, clickPosition.y, 0));

            if (clickPosition == null) return;

            switch (_currentInputState)
            {
                case InputState.Free:
                    OnFreeAction(context, clickPositionRay);
                    break;
                case InputState.ScrapSelected:
                    OnScrapSelectedAction(context, clickPositionRay);
                    break;
            }
        };
    }

    private void OnFreeAction(InputAction.CallbackContext context, Ray clickPositionRay)
    {
        LayerMask mask = LayerMask.GetMask(new string[] { "Goon", "UI"} );
        Physics.Raycast(clickPositionRay, out RaycastHit hit, float.PositiveInfinity, mask);

        if (hit.collider == null) return;

        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Goon"))
        {
            hit.collider.SendMessageUpwards("OnGoonSelected", hit.collider.gameObject);
        }

        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("UI"))
        {
            hit.collider.SendMessageUpwards("OnClickedTrigger", hit.collider.gameObject);
        }
    }

    private void OnScrapSelectedAction(InputAction.CallbackContext context, Ray clickPositionRay)
    {
        LayerMask mask = LayerMask.GetMask(new string[] { "UI" });
        Physics.Raycast(clickPositionRay, out RaycastHit hit, float.PositiveInfinity, mask);

        if (hit.collider == null) return;

        if (hit.collider.gameObject.GetComponent<ScrapSlot>())
        {
            // attach to the slot
            Debug.Log("Clicked a slot, attach the scrap");
        }
    }
}
