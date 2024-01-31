using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class BulbController : MonoBehaviour
{
    public float _bulbFlashSpeed = 1.5f;

    [SerializeField, ReadOnly]
    private Material _bulbMaterial;

    [SerializeField, ReadOnly]
    private ScrapSlot _connectedSlot;

    [SerializeField, ReadOnly]
    private Color _goonEmissionColor = Color.clear;

    [SerializeField]
    private Color _snapIndicatorEmissionColor = Color.white;

    private bool _refreshBulbState = false;

    private void Awake()
    {
        _bulbMaterial = GetComponent<MeshRenderer>().material;
        _connectedSlot = GetComponentInParent<ScrapSlot>();

        SetEmissionColor(_snapIndicatorEmissionColor);
        SetActive(false);

        InputManager.ChangedInputState += UpdateBulbState;
    }

    private void UpdateBulbState(InputManager.InputState newState)
    {
        _refreshBulbState = true;
    }

    private IEnumerator BulbFlash(float flashSpeed)
    {
        SetEmissionColor(_snapIndicatorEmissionColor);

        while (true) 
        {
            SetActive(true);
            yield return new WaitForSeconds(flashSpeed);
            SetActive(false);
            yield return new WaitForSeconds(flashSpeed);
        }
    }

    public void SetGoonEmissionColor(Color goonEmissionColor)
    {
        _goonEmissionColor = goonEmissionColor;
    }

    public void SetEmissionColor(Color newEmissionColor)
    {
        _bulbMaterial.SetColor("_EmissionColor", newEmissionColor);
    }

    public void SetActive(bool active)
    {
        if (active == true)
        {
            _bulbMaterial.EnableKeyword("_EMISSION");
        }
        else
        {
            _bulbMaterial.DisableKeyword("_EMISSION");
        }
    }

    private void OnClickedTrigger()
    {
        Debug.Log("Clicked a bulb. If in fixed mode, select this as next slot.");
    }

    private void LateUpdate()
    {
        if (_refreshBulbState)
        {
            _refreshBulbState = false;

            ScrapSlot.ScrapSlotState slotState = _connectedSlot.GetCurrentSlotState();
            InputManager.InputState newState = InputManager.GetCurrentInputState();

            if (newState == InputManager.InputState.ScrapSelected)
            {
                if (slotState == ScrapSlot.ScrapSlotState.Open)
                {
                    // if slot open, bulbs flash white
                    StartCoroutine(BulbFlash(_bulbFlashSpeed));
                    return;
                }
                return;
            }

            if (newState == InputManager.InputState.Free)
            {
                // Stop the flashing
                StopAllCoroutines();

                if (slotState == ScrapSlot.ScrapSlotState.Open)
                {
                    // if slot filled and not next, bulbs turn off
                    SetActive(false);
                    return;
                }

                if (slotState == ScrapSlot.ScrapSlotState.Filled)
                {
                    // if slot filled and next, bulbs turns green
                    SetEmissionColor(_goonEmissionColor);
                    SetActive(true);
                    return;
                }
            }

        }
    }
}

