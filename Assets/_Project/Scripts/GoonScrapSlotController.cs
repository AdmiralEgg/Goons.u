using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class GoonScrapSlotController : MonoBehaviour
{
    //[SerializeField]
    //private ScrapSlot[] _goonScrapSlots;

    [SerializeField]
    private List<ScrapSlot> _goonScrapSlotsList;

    [SerializeField, ReadOnly]
    private bool _hasFixedWords;

    [SerializeField, ReadOnly]
    private bool _usingFixedWords;

    [SerializeField, ReadOnly]
    private int _nextSlotToPlayIndex;

    [SerializeField, ReadOnly]
    private ScrapSlot _nextSlotToPlay;

    private void Awake()
    {
        _hasFixedWords = false;
        _usingFixedWords = false;
        _nextSlotToPlayIndex = 0;

        //_goonScrapSlotsList.

        foreach (ScrapSlot slot in GetComponentsInChildren<ScrapSlot>())
        {
            _goonScrapSlotsList.Add(slot);
        }

        Scrap.ScrapSelected += (scrap) =>
        {
            ActivateSlots();
        };

        InputManager.ScrapDeselected += DeactivateSlots;
    }

    private void Update()
    {
        CheckFixedWords();

        if (_hasFixedWords && _usingFixedWords)
        {
            UpdateNextSlotToPlay();
        }
    }

    private void CheckFixedWords()
    {
        _hasFixedWords = false;

        foreach (ScrapSlot slot in _goonScrapSlotsList)
        {
            if (slot.GetCurrentSlotState() == ScrapSlot.ScrapSlotState.Filled)
            {
                _hasFixedWords = true;
                return;
            }
        }
    }

    private void UpdateNextSlotToPlay()
    {
        // starting from the current element, iterate through the length of the list to find the next filled slot
        int currentIndex = _nextSlotToPlayIndex;
        ScrapSlot currentItem = null;

        do
        {
            currentIndex = (currentIndex + 1) % _goonScrapSlotsList.Count;

            if (_goonScrapSlotsList[currentIndex].GetCurrentSlotState() == ScrapSlot.ScrapSlotState.Filled)
            {
                var currentitem = _goonScrapSlotsList[currentIndex];
            }

        } while (currentIndex != _nextSlotToPlayIndex);

        _nextSlotToPlayIndex = currentIndex;
        _nextSlotToPlay = currentItem;
    }

    private void ToggleUsingFixedWords()
    {
        _usingFixedWords = !_usingFixedWords;

        // TODO: set the SlotToPlayIndex to the first item from the top
        for (int i = 0; i < _goonScrapSlotsList.Count; i++)
        {
            if (_goonScrapSlotsList[i].GetCurrentSlotState() == ScrapSlot.ScrapSlotState.Filled)
            {
                _nextSlotToPlayIndex = i;
                _nextSlotToPlay = _goonScrapSlotsList[i];
                break;
            }
        }
    }

    public void ActivateSlots()
    {
        foreach (ScrapSlot slot in _goonScrapSlotsList) 
        {
            slot.GetComponentInChildren<HingeJoint>().useMotor = true;
        }
    }

    public void DeactivateSlots()
    {
        foreach (ScrapSlot slot in _goonScrapSlotsList)
        {
            slot.GetComponentInChildren<HingeJoint>().useMotor = false;
        }
    }

    public bool GetUsingFixedWords()
    { 
        return _usingFixedWords;
    }
}
