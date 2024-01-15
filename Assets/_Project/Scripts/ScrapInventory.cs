using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public class ScrapInventory : MonoBehaviour
{
    [Tooltip("Maximum slots that can be used to hold words")]
    public int _maxSlots = 5;

    [SerializeField, ReadOnly, Tooltip("Scraps currently being held")]
    private List<Scrap> _scrapsHeld;

    [SerializeField, ReadOnly]
    private List<ScrapSlot> _scrapSlots;

    [Tooltip("Plane on which scraps are arranged")]
    private Mesh _scrapMesh;

    void Awake()
    {
        _scrapsHeld = new List<Scrap>();
        _scrapSlots = new List<ScrapSlot>(GetComponentsInChildren<ScrapSlot>());

        Scrap.ScrapCaught += AddScrapToInventory;
    }

    public void AddScrapToInventory(Scrap scrap)
    {
        // Check we have slots available
        if (CountScrapHeld() >= _maxSlots)
        {
            Debug.Log("Get rid of the oldest scrap? Or 'Oops, no room!'?");
            return;
        }

        // Iterate through all slots and find a spare
        foreach (ScrapSlot slot in GetComponentsInChildren<ScrapSlot>())
        {
            if (slot.GetCurrentSlotState() == ScrapSlot.ScrapSlotState.Open)
            {
                slot.AddScrapToSlot(scrap);
                return;
            }
        }

        //OrganiseInventory();
    }

    /// <summary>
    /// Organises the scraps held
    /// </summary>
    private void OrganiseInventory()
    {
        /*
        // Iterate through all slots and find a spare
        foreach (ScrapSlot scrap in _scrapSlots)
        {

        }

        // Check for children of the scrap object
        foreach (ScrapSlot scrap in _scrapSlots)
        {
            scrap.transform.position = new Vector3(slotStart, this.transform.position.y, this.transform.position.z);
        }
        */
    }

    private int CountScrapHeld()
    {
        return GetComponentsInChildren<Scrap>().Length;
    }
}
