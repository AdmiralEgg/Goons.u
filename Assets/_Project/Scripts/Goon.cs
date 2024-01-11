using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

public class Goon : MonoBehaviour
{
    public enum GoonType
    {
        Hag,
        Toff,
        Yorky
    }

    private AudioSource _faceAudioSource;

    [SerializeField]
    private GoonType _goonType;

    [SerializeField]
    private WordData[] _wordData;

    // TODO: Use a Queue type instead?
    [SerializeField, ReadOnly]
    private List<WordData> _wordQueue;

    private void OnEnable()
    {
        _faceAudioSource = GetComponentInChildren<AudioSource>();

        // Load two words into the queue
        LoadRandomWords(2);
    }

    // Triggered by InputManager
    private void OnGoonSelected(GameObject gameObject)
    {
        Debug.Log($"Something poked me on the {gameObject.name}! I'm the {_goonType}");

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

        string word = _wordQueue.First<WordData>().Word;
        AudioClip wordAudio = _wordQueue.First<WordData>().WordAudio;
        _wordQueue.RemoveAt(0);
        
        if (wordAudio != null ) 
        {
            _faceAudioSource.PlayOneShot(wordAudio);
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
}
