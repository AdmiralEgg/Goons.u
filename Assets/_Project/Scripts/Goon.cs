using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.UIElements;
using Sirenix.OdinInspector.Editor.Drawers;
using UnityEngine.InputSystem.Interactions;

public class Goon : MonoBehaviour
{
    public enum GoonType
    {
        Hag,
        Toff,
        Yorky
    }

    public enum GoonState
    {
        Idle,
        Speaking
    }

    [SerializeField, Required]
    private GoonData _goonData;

    [SerializeField, ReadOnly]
    private GoonState _currentState;

    [SerializeField, ReadOnly]
    private AudioClip _niceCatchAudio, _stickTouchAudio, _speakerTouchAudio, _crowdTouchAudio;

    [SerializeField, ReadOnly]
    private WordData[] _wordData;

    [SerializeField, ReadOnly]
    private List<WordData> _wordQueue;

    [SerializeField, ReadOnly]
    private ScrapSlot[] _allScrapSlots;

    private GoonScrapSlotController _goonScrapSlotController;
    private WordSelectorController _wordSelectorController;
    private ScrapGenerator _scrapGenerator;
    private AudioSource _faceAudioSource;

    private void OnEnable()
    {
        _faceAudioSource = GetComponentInChildren<AudioSource>();
        _scrapGenerator = GetComponentInChildren<ScrapGenerator>();
        _allScrapSlots = GetComponentsInChildren<ScrapSlot>();
        _goonScrapSlotController = GetComponentInChildren<GoonScrapSlotController>();
        _wordSelectorController = GetComponentInChildren<WordSelectorController>();

        _niceCatchAudio = _goonData.NiceCatchAudio;
        _stickTouchAudio = _goonData.StickTouchAudio;
        _speakerTouchAudio = _goonData.SpeakerTouchAudio;
        _crowdTouchAudio = _goonData.CrowdTouchAudio;

        _wordData = _goonData.WordData;

        _currentState = GoonState.Idle;

        foreach (BulbController bulb in GetComponentsInChildren<BulbController>()) 
        {
            bulb.SetGoonEmissionColor(_goonData.WordColour);
        }

        // Load two words into the queue
        LoadRandomWords(2);
    }

    // Triggered by InputManager
    private void OnGoonSelected(GameObject gameObject)
    {
        if (_currentState == GoonState.Speaking) return;

        if (gameObject.name == "GoonStick")
        {
            PlayComment(_stickTouchAudio);
            return;
        }

        if (_wordSelectorController.GetSelectedButtonType() == ButtonController.ButtonType.Random)
        {
            PlayRandomWord();
        }

        if (_wordSelectorController.GetSelectedButtonType() == ButtonController.ButtonType.Fixed)
        {
            PlayNextFixedWord();
        }
    }

    private void LoadRandomWords(int wordsToLoad = 1)
    {
        for (int i =  0; i < wordsToLoad; i++)
        {
            System.Random random = new System.Random();
            int randomNumber = random.Next(_wordData.Length);

            _wordQueue.Add(_wordData[randomNumber]);
        }
    }

    private void PlayNextFixedWord()
    {
        WordData wordData = _goonScrapSlotController.GetNextSlotToPlay();

        // Play this word
        StartCoroutine(Speak(wordData.WordAudio));
        _scrapGenerator.PrintScrap(wordData);
        return;
    }

    private void PlayRandomWord()
    {
        if (_currentState == GoonState.Speaking) return;
        
        if (_wordQueue.Count == 0)
        {
            Debug.LogError("No words found in the queue, cannot play.");
            return;
        }

        WordData wordData = _wordQueue.First<WordData>();

        string word = wordData.Word;
        AudioClip wordAudio = wordData.WordAudio;
        _wordQueue.RemoveAt(0);
        
        if (wordAudio != null ) 
        {
            StartCoroutine(Speak(wordAudio));

            if (_scrapGenerator.gameObject.activeInHierarchy)
            {
                _scrapGenerator.PrintScrap(wordData);
            }
        }
        else
        {
            Debug.LogError($"Word audio for {word} is not defined.");
        }

        LoadRandomWords();
    }

    public void Dance()
    {
        // dance left and right, or up and down
    }

    public GoonData GetGoonData()
    {
        return _goonData;
    }

    public GoonState GetGoonState()
    {
        return _currentState;
    }

    private void PlayComment(AudioClip clip)
    {
        if (_currentState == GoonState.Speaking) return;

        StartCoroutine(Speak(clip));
    }

    public void PlayGroupComment(AudioClip clip)
    {
        if (_currentState == GoonState.Speaking) return;

        StartCoroutine(Speak(clip));
    }

    private IEnumerator Speak(AudioClip clip)
    {
        _currentState = GoonState.Speaking;
        
        _faceAudioSource.PlayOneShot(clip);

        yield return new WaitForSeconds(0.75f);

        _currentState = GoonState.Idle;
    }
}
