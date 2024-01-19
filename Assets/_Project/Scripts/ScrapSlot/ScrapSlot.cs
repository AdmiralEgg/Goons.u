using System;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class ScrapSlot : MonoBehaviour
{
    public enum ScrapSlotState { Open, Filled }

    [SerializeField, ReadOnly]
    private ScrapSlotState _currentSlotState;

    [SerializeField, ReadOnly]
    private Scrap _slotScrap;

    public static Action<Scrap> ScrapAttached;

    protected void Awake()
    {
        _currentSlotState = ScrapSlotState.Open;
    }

    protected void Update()
    {
        RefreshSlotState();
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.25f);
    }

    protected void RefreshSlotState()
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
        SetScrapAttachedState(scrap);
        ScrapAttached?.Invoke(scrap);

        RefreshSlotState();
    }

    public abstract void SetScrapAttachedState(Scrap scrap);

    public ScrapSlotState GetCurrentSlotState()
    {
        return _currentSlotState;
    }

    public Scrap GetScrap()
    {
        return _slotScrap;
    }
}
