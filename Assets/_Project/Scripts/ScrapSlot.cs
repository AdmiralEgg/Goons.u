using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ScrapSlot : MonoBehaviour
{
    public enum ScrapSlotState
    {
        Open,
        Filled
    }

    public enum ScrapSlotType
    {
        Inventory,
        Goon
    }

    [SerializeField, ReadOnly]
    private ScrapSlotState _currentSlotState;

    [SerializeField]
    private ScrapSlotType _slotType;

    [SerializeField, ReadOnly]
    private Scrap _slotScrap;

    void Awake()
    {
        _currentSlotState = ScrapSlotState.Open;
    }

    public void AddScrapToSlot(Scrap scrap)
    {
        // Move to the slot
        scrap.transform.SetParent(this.transform);
        scrap.transform.position = this.transform.position;
        
        // Set the scrap state
        if (_slotType == ScrapSlotType.Inventory)
        {
            scrap.SetScrapState(Scrap.ScrapState.Inventory);
        }
        
        _slotScrap = scrap;
        _currentSlotState = ScrapSlotState.Filled;
    }

    public void RemoveScrapFromSlot()
    {
        _slotScrap.SetScrapState(Scrap.ScrapState.Free);
        
        _currentSlotState = ScrapSlotState.Open;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.25f);
    }

    public ScrapSlotState GetCurrentSlotState()
    {
        return _currentSlotState;
    }
}
