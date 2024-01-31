using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

public class ScrapGenerator : MonoBehaviour
{
    public enum GeneratorState
    {
        Stopped,
        InTransition,
        Running
    }
    
    [SerializeField, AssetsOnly]
    private GameObject _scrapPrefab;

    [SerializeField]
    private AudioClip _startupSound;

    [SerializeField]
    private AudioClip _shutdownSound;

    [SerializeField]
    private AudioClip _runningSound;

    [SerializeField]
    private AudioClip _printSound;

    [SerializeField]
    private GeneratorState _currentState;

    private AudioSource _audioSource;
    private GoonData _goonData;

    void Awake()
    {
        _goonData = this.GetComponentInParent<Goon>().GetGoonData();

        _audioSource = this.GetComponent<AudioSource>();
        SetupAudioSource();



        _currentState = GeneratorState.Stopped;
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

    private void SetupAudioSource()
    {
        _audioSource = this.GetComponent<AudioSource>();
        _audioSource.loop = true;
        _audioSource.playOnAwake = false;
        _audioSource.clip = _runningSound;
    }

    public void PrintScrap(WordData word)
    {
        if (_currentState != GeneratorState.Running) return;
        
        // Play some printing audio
        _audioSource.PlayOneShot(_printSound);

        GameObject scrap = Instantiate(_scrapPrefab, this.transform, false);

        // apply font and colour to scrap
        scrap.GetComponent<Scrap>().SetWordData(word);
        if (_goonData.WordFont != null) scrap.GetComponent<Scrap>().SetFont(_goonData.WordFont);
        if (_goonData.WordColour != null) scrap.GetComponent<Scrap>().SetFontColor(_goonData.WordColour);
    }
}
