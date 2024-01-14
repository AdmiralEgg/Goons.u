using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.UIElements;

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

    [SerializeField]
    private AudioClip _niceCatchAudio, _stickTouchAudio, _speakerTouchAudio, _crowdTouchAudio;

    [SerializeField]
    private WordData[] _wordData;

    [SerializeField, ReadOnly]
    private List<WordData> _wordQueue;

    [SerializeField, ReadOnly]
    private GoonState _currentState;

    private ScrapGenerator _scrapGenerator;
    private AudioSource _faceAudioSource;

    private void OnEnable()
    {
        _faceAudioSource = GetComponentInChildren<AudioSource>();
        _scrapGenerator = GetComponentInChildren<ScrapGenerator>();
        _currentState = GoonState.Idle;

        Scrap.ScrapCaught += (scrap) =>
        {
            PlayComment(scrap, _niceCatchAudio);
        };

        // Load two words into the queue
        LoadRandomWords(2);
    }

    // Triggered by InputManager
    private void OnGoonSelected(GameObject gameObject)
    {
        if (_currentState == GoonState.Speaking) return;

        Debug.Log($"Something poked me on the {gameObject.name}! I'm the {_goonData.GoonType}");

        if (gameObject.name == "GoonStick")
        {
            StartCoroutine(Speak(_stickTouchAudio));
            return;
        }

        PlayWord();
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

    private void PlayWord()
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

    private void PlayComment(Scrap caughtScrap, AudioClip clip)
    {
        if (_currentState == GoonState.Speaking) return;

        StartCoroutine(Speak(clip));
    }

    private IEnumerator Speak(AudioClip clip)
    {
        _currentState = GoonState.Speaking;
        
        _faceAudioSource.PlayOneShot(clip);

        yield return new WaitForSeconds(clip.length);

        _currentState = GoonState.Idle;
    }
}
