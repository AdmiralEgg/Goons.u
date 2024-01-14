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

    [Tooltip("Plane on which scraps are arranged")]
    private Mesh _scrapMesh;

    void Awake()
    {
        _scrapsHeld = new List<Scrap>();

        Scrap.ScrapCaught += AddScrapToInventory;
    }

    public void AddScrapToInventory(Scrap scrap)
    {
        if (_scrapsHeld.Count >= _maxSlots)
        {
            Debug.Log("Get rid of the oldest scrap? Or 'Oops, no room!'?");
            return;
        }
       
        scrap.transform.SetParent(this.transform);
        scrap.SetScrapState(Scrap.ScrapState.Inventory);
        scrap.transform.position = this.transform.position;
        _scrapsHeld.Add(scrap);

        SortInventory();
    }

    /// <summary>
    /// Organises the scraps held
    /// </summary>
    private void SortInventory()
    {
        // Divide the max slots by width of the mesh to get slots
        float slotStart = this.transform.localScale.x / _maxSlots;
        
        // Check for children of the scrap object
        foreach (Scrap scrap in GetComponentsInChildren<Scrap>())
        {
            scrap.transform.position = new Vector3(slotStart, this.transform.position.y, this.transform.position.z);
        }
    }
}
