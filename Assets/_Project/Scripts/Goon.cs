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

    private AudioSource _faceAudioSource;

    [SerializeField, Required]
    private GoonData _goonData;

    [SerializeField]
    private WordData[] _wordData;

    // TODO: Use a Queue type instead?
    [SerializeField, ReadOnly]
    private List<WordData> _wordQueue;

    private ScrapGenerator _scrapGenerator;

    [SerializeField]
    private AudioClip _niceCatchAudio, _stickTouchAudio, _speakerTouchAudio, _crowdTouchAudio;

    private void OnEnable()
    {
        _faceAudioSource = GetComponentInChildren<AudioSource>();
        _scrapGenerator = GetComponentInChildren<ScrapGenerator>();

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
        Debug.Log($"Something poked me on the {gameObject.name}! I'm the {_goonData.GoonType}");

        if (gameObject.name == "GoonStick")
        {
            PlayComment(null, _stickTouchAudio);
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
            _faceAudioSource.PlayOneShot(wordAudio);

            if (_scrapGenerator.gameObject.activeInHierarchy)
            {
                _scrapGenerator.PrintScrap(wordData);
            }
        }
        else
        {
            _faceAudioSource.Play();
        }

        LoadRandomWords();
    }

    private List<WordData> GetWordQueue()
    {
        return _wordQueue;
    }

    public void Dance()
    {
        // dance left and right, or up and down
    }

    public GoonData GetGoonData()
    {
        return _goonData;
        // dance left and right, or up and down
    }

    private void PlayComment(Scrap caughtScrap, AudioClip clip)
    {
        _faceAudioSource.PlayOneShot(clip);
    }
}
