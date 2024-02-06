using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

public class Goon : MonoBehaviour
{
    public enum GoonType { Hag, Toff, Yorky }
    public enum GoonState { Idle, Speaking }

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

    [SerializeField]
    private ScrapInventory _assignedScrapInventory;

    private AudioSource _faceAudioSource;

    public static Action<WordData> GoonSpeak;

    private void OnEnable()
    {
        _assignedScrapInventory.AssignGoon(this);
        
        _faceAudioSource = GetComponentInChildren<AudioSource>();

        _niceCatchAudio = _goonData.NiceCatchAudio;
        _stickTouchAudio = _goonData.StickTouchAudio;
        _speakerTouchAudio = _goonData.SpeakerTouchAudio;
        _crowdTouchAudio = _goonData.CrowdTouchAudio;

        _wordData = _goonData.WordData;

        _currentState = GoonState.Idle;

        // Load two words into the queue
        LoadRandomWords(2);

        InputManager.InventoryScrapClicked += PlayFixedWord;
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

        PlayRandomWord();
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

    private void PlayFixedWord(WordData wordData)
    {
        if (_currentState == GoonState.Speaking) return;
        if (wordData.Goon != _goonData.GoonType) return;

        if (wordData.WordAudio != null)
        {
            StartCoroutine(Speak(wordData.WordAudio));
        }
        else
        {
            Debug.LogError($"Word audio for {wordData.Word} is not defined.");
        }
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
        wordData.SetFont(_goonData.WordFont);
        wordData.SetFontColor(_goonData.WordColor);

        _wordQueue.RemoveAt(0);
        
        if (wordData.WordAudio != null)
        {
            StartCoroutine(Speak(wordData.WordAudio));
            GoonSpeak?.Invoke(wordData);
        }
        else
        {
            Debug.LogError($"Word audio for {wordData.Word} is not defined.");
        }

        LoadRandomWords();
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
