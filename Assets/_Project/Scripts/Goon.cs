using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using FMODUnity;
using FMOD.Studio;
using System.Runtime.InteropServices;

public class Goon : MonoBehaviour
{
    public enum GoonType { Hag, Toff, Yorky }
    public enum GoonState { Idle, Speaking }

    [SerializeField, Required]
    private GoonData _goonData;

    [SerializeField, ReadOnly]
    private GoonState _currentState;

    [SerializeField, ReadOnly]
    private List<WordData> _wordData;

    [SerializeField]
    private ScrapInventory _assignedScrapInventory;
    private GoonMove _goonMove;

    [SerializeField]
    private float DelayBetweenSpeaking = 1f;

    public static Action<WordData> GoonSpeak;

    // FMOD Sounds
    [Header("Audio Data")]
    [SerializeField]
    private EventReference _randomWordEvent;
    [SerializeField]
    private EventReference _singleWordEvent;
    [SerializeField]
    private EventReference _stickTouchEvent;
    [SerializeField, Tooltip("Name of the parameter in FMOD which should be set to control which single words are played.")]
    private string _soundbankParameterName;

    private FMOD.Studio.EventInstance _singleWordInstance;
    private FMOD.Studio.EventInstance _stickTouchInstance;
    private FMOD.Studio.EventInstance _randomWordInstance;

    // Callback fields
    private FMOD.Studio.EVENT_CALLBACK _randomWordCallback;
    private RandomWordData _randomWordData = null;
    private GCHandle _randomWordHandle;

    [StructLayout(LayoutKind.Sequential)]
    public class RandomWordData
    {
        public FMOD.StringWrapper RandomWord = new FMOD.StringWrapper();
        public string WordFileName;
    }

    [SerializeField, ReadOnly]
    private string _lastRandomWordSpoken;

    private void OnEnable()
    {
        SetupFMOD();

        _wordData = new List<WordData>();

        foreach (WordData wordData in _goonData.WordData)
        {
            _wordData.Add(wordData);
        }

        _currentState = GoonState.Idle;

        _goonMove = GetComponent<GoonMove>();
        _assignedScrapInventory.AssignGoon(this);

        InputManager.InventoryScrapClicked += PlaySingleWord;
    }

    private void SetupFMOD()
    {
        _singleWordInstance = FMODUnity.RuntimeManager.CreateInstance(_singleWordEvent);
        _stickTouchInstance = FMODUnity.RuntimeManager.CreateInstance(_stickTouchEvent);
        _randomWordInstance = FMODUnity.RuntimeManager.CreateInstance(_randomWordEvent);

        FMODUnity.RuntimeManager.AttachInstanceToGameObject(_singleWordInstance, this.transform);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(_stickTouchInstance, this.transform);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(_randomWordInstance, this.transform);

        _randomWordData = new RandomWordData();
        _randomWordCallback = new FMOD.Studio.EVENT_CALLBACK(ProcessWordSpokenCallback);
        _randomWordHandle = GCHandle.Alloc(_randomWordData, GCHandleType.Pinned);

        _randomWordInstance.setUserData(GCHandle.ToIntPtr(_randomWordHandle));
        _randomWordInstance.setCallback(_randomWordCallback, EVENT_CALLBACK_TYPE.SOUND_PLAYED);
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT ProcessWordSpokenCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        IntPtr randomWordDataPtr;
        FMOD.RESULT result = instance.getUserData(out randomWordDataPtr);

        if (result != FMOD.RESULT.OK)
        {
            Debug.Log($"Random Word Callback Error: { result }");
            return FMOD.RESULT.ERR_UNSUPPORTED;
        }

        if (randomWordDataPtr == IntPtr.Zero)
        {
            Debug.Log($"Random Word Pointer is null: {randomWordDataPtr}");
            return FMOD.RESULT.ERR_UNSUPPORTED;
        }

        GCHandle randomWordHandle = GCHandle.FromIntPtr(randomWordDataPtr);
        RandomWordData randomWordData = (RandomWordData)randomWordHandle.Target;

        switch (type)
        {
            case EVENT_CALLBACK_TYPE.SOUND_PLAYED:
            {
                FMOD.Sound sound = new FMOD.Sound(parameterPtr);
                string soundName;
                sound.getName(out soundName, 1024);
                
                randomWordData.WordFileName = soundName;
                break;
            }
            case EVENT_CALLBACK_TYPE.ALL:
                break;
        }
        
        return FMOD.RESULT.OK;
    }

    private void Update()
    {
        CheckRandomWordData();
    }

    private void CheckRandomWordData()
    {
        if (_randomWordData.WordFileName == null) return;

        if ((_lastRandomWordSpoken == "") || (_lastRandomWordSpoken != _randomWordData.WordFileName))
        {
            WordData wordData = GetWordDataByFMODWordData(_randomWordData.WordFileName);
            wordData.SetFont(_goonData.WordFont);
            wordData.SetFontColor(_goonData.WordColor);

            GoonSpeak?.Invoke(wordData);
            _lastRandomWordSpoken = _randomWordData.WordFileName;
            return;
        }
    }

    private void OnDisable()
    {
        CleanupFMOD();
    }

    private void OnDestroy()
    {
        CleanupFMOD();
    }

    private void CleanupFMOD()
    {
        _singleWordInstance.release();
        _stickTouchInstance.release();
        _randomWordInstance.release();
        _randomWordInstance.setUserData(IntPtr.Zero);
        
        if (_randomWordHandle.IsAllocated)
        {
            _randomWordHandle.Free();
        }
    }

    // Triggered by InputManager
    private void OnGoonSelected(GameObject gameObject)
    {
        if (_currentState == GoonState.Speaking) return;

        if (gameObject.name == "GoonStick")
        {
            PlayStickTouch();
            return;
        }

        PlayRandomWord();
    }

    private void PlaySingleWord(WordData wordData)
    {
        if (_currentState == GoonState.Speaking) return;

        if (wordData.Goon != _goonData.GoonType) return;

        FMOD.RESULT paramSetResult = _singleWordInstance.setParameterByNameWithLabel(_soundbankParameterName, wordData.FMODWordDataName);

        if (paramSetResult.HasFlag(FMOD.RESULT.OK) == false)
        {
            Debug.LogError($"Param set: {paramSetResult}");
        }

        _singleWordInstance.start();
        StartCoroutine(DelayWhileSpeaking());
    }

    private void PlayRandomWord()
    {
        if (_currentState == GoonState.Speaking) return;

        _randomWordInstance.start();
        StartCoroutine(DelayWhileSpeaking());
    }

    public GoonData GetGoonData()
    {
        return _goonData;
    }

    public GoonState GetGoonState()
    {
        return _currentState;
    }

    private void PlayStickTouch()
    {
        if (_currentState == GoonState.Speaking) return;

        _stickTouchInstance.start();
    }

    private IEnumerator DelayWhileSpeaking()
    {
        _currentState = GoonState.Speaking;

        // play goon prod
        _goonMove.GoonProd();

        yield return new WaitForSeconds(DelayBetweenSpeaking);
        
        _currentState = GoonState.Idle;
    }

    private WordData GetWordDataByFMODWordData(string searchWord)
    {
        WordData data = _wordData.Find(x => x.FMODWordDataName == searchWord);

        Debug.Log($"Returning word: {data.Word} and FMOD file name: {data.FMODWordDataName}");

        return data;
    }
}
