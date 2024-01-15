using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GoonScrapSlotController : MonoBehaviour
{
    [SerializeField]
    private ScrapSlot[] _goonScrapSlots;

    private void Awake()
    {
        Scrap.ScrapSelected += (scrap) =>
        {
            ActivateSlots();
        };

        InputManager.ScrapDeselected += DeactivateSlots;
    }

    public void ActivateSlots()
    {
        foreach (ScrapSlot slot in _goonScrapSlots) 
        {
            slot.GetComponentInChildren<HingeJoint>().useMotor = true;
        }
    }

    public void DeactivateSlots()
    {
        foreach (ScrapSlot slot in _goonScrapSlots)
        {
            slot.GetComponentInChildren<HingeJoint>().useMotor = false;
        }
    }
}
