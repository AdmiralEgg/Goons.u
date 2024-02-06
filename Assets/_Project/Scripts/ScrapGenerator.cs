using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

public class ScrapGenerator : MonoBehaviour
{
    public enum GeneratorState { Stopped, InTransition, Running }
    
    [SerializeField, AssetsOnly]
    private GameObject _scrapPrefab;

    [Header("Audio")]
    [SerializeField]
    private AudioClip _startupSound, _shutdownSound, _printSound;
    private AudioSource _audioSource;

    [SerializeField, ReadOnly]
    private GeneratorState _currentState;

    void Awake()
    {
        _audioSource = this.GetComponent<AudioSource>();

        _currentState = GeneratorState.Stopped;

        Goon.GoonSpeak += PrintScrap;
    }

    public void StartGenerator()
    {
        // if Running, or Starting, then return
        if ((_currentState == GeneratorState.Running) || (_currentState == GeneratorState.InTransition)) return;

        _currentState = GeneratorState.InTransition;

        StartCoroutine(Startup());
    }

    private IEnumerator Startup()
    {
        // whirring sound
        _audioSource.PlayOneShot(_startupSound);
        _currentState = GeneratorState.Running;

        yield return new WaitForSeconds(_startupSound.length);

        _audioSource.Play();
    }

    public void ShutdownGenerator()
    {
        // if Stopped, or ShuttingDown, then return
        if ((_currentState == GeneratorState.Stopped) || (_currentState == GeneratorState.InTransition)) return;

        _currentState = GeneratorState.InTransition;

        StartCoroutine(Shutdown());
    }

    private IEnumerator Shutdown()
    {
        _audioSource.Stop();
        _audioSource.PlayOneShot(_shutdownSound);

        yield return new WaitForSeconds(_startupSound.length);

        _currentState = GeneratorState.Stopped;
    }

    public void PrintScrap(WordData word)
    {
        if (_currentState != GeneratorState.Running) return;
        
        // Play some printing audio
        _audioSource.PlayOneShot(_printSound);

        GameObject scrap = Instantiate(_scrapPrefab, this.transform, false);

        // apply font and colour to scrap
        scrap.GetComponent<Scrap>().SetWordData(word);
    }
}
