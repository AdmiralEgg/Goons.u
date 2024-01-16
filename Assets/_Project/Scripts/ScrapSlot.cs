using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ScrapSlot : MonoBehaviour
{
    public enum ScrapSlotState { Open, Filled }
    public enum ScrapSlotType { Inventory, Goon }

    [SerializeField, ReadOnly]
    private ScrapSlotState _currentSlotState;

    [SerializeField]
    private ScrapSlotType _slotType;

    [SerializeField, ReadOnly]
    private Scrap _slotScrap;

    public static Action<Scrap> ScrapAttachedToGoon;
    public static Action<Scrap> ScrapAttachedToInventory;

    void Awake()
    {
        _currentSlotState = ScrapSlotState.Open;
    }

    private void Update()
    {
        Scrap scrap = GetComponentInChildren<Scrap>();
        
        // Check for any scrap in this slot
        if (scrap != null)
        {
            _currentSlotState = ScrapSlotState.Filled;
            _slotScrap = scrap;
        }
        else
        {
            _currentSlotState = ScrapSlotState.Open;
            _slotScrap = null;
        }
    }

    public void AddScrapToSlot(Scrap scrap)
    {
        if (_currentSlotState == ScrapSlotState.Filled) 
        {
            Debug.Log("Slot already filled, swap scraps!");
            return;
        }
        
        // Move to the slot
        scrap.transform.SetParent(this.transform);
        scrap.transform.position = this.transform.position;

        _currentSlotState = ScrapSlotState.Filled;

        // Set the scrap state
        if (_slotType == ScrapSlotType.Inventory)
        {
            scrap.SetScrapAttachedState(Scrap.ScrapAttachedState.Inventory);
            ScrapAttachedToInventory?.Invoke(scrap);
        }

        // Set the scrap state
        if (_slotType == ScrapSlotType.Goon)
        {
            scrap.SetScrapAttachedState(Scrap.ScrapAttachedState.Goon);
            ScrapAttachedToGoon?.Invoke(scrap);
        }
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

    public Scrap GetScrap()
    {
        return _slotScrap;
    }
}
