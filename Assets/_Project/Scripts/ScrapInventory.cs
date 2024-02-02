using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ScrapInventory : MonoBehaviour
{
    [SerializeField, ReadOnly, Tooltip("Scraps currently being held")]
    private List<Scrap> _scrapsHeld;

    [SerializeField, ReadOnly]
    private List<ScrapSlot> _scrapSlots;

    [SerializeField, ReadOnly]
    private Goon _assignedGoon;
    [SerializeField, ReadOnly, Tooltip("Used to check scrap slots")]
    private Goon.GoonType _assignedGoonType;

    void Awake()
    {
        _scrapsHeld = new List<Scrap>();
        _scrapSlots = new List<ScrapSlot>(GetComponentsInChildren<ScrapSlot>());

        Scrap.ScrapCaught += AddScrapToInventory;
    }

    private void Update()
    {
        // Check for a difference in number of scraps
        if (GetComponentsInChildren<Scrap>().Length != _scrapsHeld.Count)
        {
            _scrapsHeld.Clear();

            foreach (Scrap scrap in GetComponentsInChildren<Scrap>())
            {
                _scrapsHeld.Add(scrap);
            }
        }
    }

    public void AddScrapToInventory(Scrap scrap)
    {
        // Check this slot is for our goon
        if (scrap.GetScrapGoonType() != _assignedGoonType) return;

        // Check we have slots available
        if (CountScrapHeld() >= _scrapSlots.Count)
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
    }

    private int CountScrapHeld()
    {
        return _scrapsHeld.Count;
    }

    public void AssignGoon(Goon goon)
    {
        _assignedGoon = goon;
        _assignedGoonType = goon.GetGoonData().GoonType;
    }
}
