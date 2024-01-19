using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class GoonScrapSlotController : MonoBehaviour
{
    [SerializeField]
    private List<ScrapSlot> _goonScrapSlotsList;

    [SerializeField, ReadOnly]
    private bool _hasFixedWords;

    [SerializeField, ReadOnly]
    private int _nextSlotToPlayIndex;

    [SerializeField, ReadOnly]
    private ScrapSlot _nextSlotToPlay;

    [SerializeField, ReadOnly]
    private bool _slotsActive;

    [SerializeField, ReadOnly]
    private bool _refreshSlots;

    [SerializeField, ReadOnly]
    private ButtonController.ButtonType _currentButtonType;

    [SerializeField]
    private WordSelectorController _wordSelectorController;

    public static Action<ScrapSlot> NewNextSlot;
    
    private void Awake()
    {
        _hasFixedWords = false;
        _refreshSlots = false;
        _nextSlotToPlayIndex = 0;


        foreach (ScrapSlot slot in GetComponentsInChildren<ScrapSlot>())
        {
            _goonScrapSlotsList.Add(slot);
        }

        Scrap.ScrapSelected += (scrap) =>
        {
            SetSlotsActive(true);
        };

        GoonScrapSlot.ScrapAttached += (scrap) =>
        {
            _refreshSlots = true;
        };

        InputManager.ScrapDeselected += () =>
        {
            SetSlotsActive(false);
        };

        WordSelectorController.SwitchedMode += (buttonType) =>
        {
            _currentButtonType = buttonType;
            _refreshSlots = true;
        };
    }

    private void Update()
    {
        CheckFixedWords();
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

    private void SetNextSlotToPlay()
    {
        List<ScrapSlot> filledSlots = new List<ScrapSlot>();

        // If no active slots, then error. Shouldn't get to here.
        foreach (ScrapSlot slot in _goonScrapSlotsList)
        {
            if (slot.GetCurrentSlotState() == ScrapSlot.ScrapSlotState.Filled)
            {
                filledSlots.Add(slot);
            };
        }

        switch (filledSlots.Count)
        {
            case 0:
                Debug.LogError("ERROR: Tried to assign a NextSlotToPlay, but no slots are filled. Set next slot to null.");
                _nextSlotToPlay = null;
                _nextSlotToPlayIndex = 0;
                break;
            case 1:
                Debug.Log("One slot filled, returning that slot.");
                _nextSlotToPlay = filledSlots.FirstOrDefault<ScrapSlot>();
                _nextSlotToPlayIndex = _goonScrapSlotsList.IndexOf(_nextSlotToPlay);
                break;
            case int n when (n >= 2):
            {
                int currentSlotIndex = _nextSlotToPlayIndex;
                int checkingSlotIndex = _nextSlotToPlayIndex;

                Debug.Log($"Slots filled: {n}. Calculate next slot.");

                do
                {
                    var currentCheckingSlotIndex = (checkingSlotIndex + 1) % _goonScrapSlotsList.Count;

                    if (_goonScrapSlotsList[currentCheckingSlotIndex].GetCurrentSlotState() == ScrapSlot.ScrapSlotState.Open)
                    {
                        checkingSlotIndex++;
                        continue;
                    }

                    _nextSlotToPlay = _goonScrapSlotsList[currentCheckingSlotIndex];
                    _nextSlotToPlayIndex = currentCheckingSlotIndex;
                    break;
                } 
                while (checkingSlotIndex != currentSlotIndex);

                Debug.Log($"Next slot Index: {_nextSlotToPlayIndex}.");

                break;
            }        
        }
        
        NewNextSlot?.Invoke(_nextSlotToPlay);
    }

    public void SetSlotsActive(bool setActive)
    {
        _slotsActive = setActive;

        foreach (ScrapSlot slot in _goonScrapSlotsList)
        {
            slot.GetComponentInChildren<HingeJoint>().useMotor = setActive;
        }
    }

    public WordData GetNextSlotToPlay()
    {
        WordData wordData = _nextSlotToPlay.GetScrap().GetWordData();

        _refreshSlots = true;

        return wordData;
    }

    private void UpdateBulbs()
    {
        // If in random state
        if (_currentButtonType == ButtonController.ButtonType.Random)
        {
            foreach (GoonScrapSlot slot in _goonScrapSlotsList)
            {
                slot.GetAttachedBulb().SetActive(false);
            }
        }

        // If in fixed state
        if (_currentButtonType == ButtonController.ButtonType.Fixed)
        {
            foreach (GoonScrapSlot slot in _goonScrapSlotsList)
            {
                if (_nextSlotToPlay == slot)
                {
                    slot.GetAttachedBulb().SetActive(true);
                }
                else
                {
                    slot.GetAttachedBulb().SetActive(false);
                }
            }
        }

    }

    public void LateUpdate()
    {
        if (_refreshSlots)
        {
            SetNextSlotToPlay();
            UpdateBulbs();
            _refreshSlots = false;
        }
    }
}
