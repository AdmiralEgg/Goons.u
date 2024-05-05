using FMOD.Studio;
using FMODUnity;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Sirenix.OdinInspector;
using MoreMountains.Feedbacks;
using System.Collections;

public class SpeakerRunMechanism : BaseRunMechanism
{
    public enum MusicMarkerName { MusicStart, MusicEnd };
    
    // FMOD Sounds
    [Header("Audio Data")]
    [SerializeField]
    private EventReference _musicEvent;

    private FMOD.Studio.EventInstance _musicInstance;

    // Callbacks
    private FMOD.Studio.EVENT_CALLBACK _musicCallback;
    private MusicData _musicData = null;
    private GCHandle _musicHandle;

    public static event Action s_BeatEvent;
    public static event Action s_MidBar;
    public static event Action s_EndBar;
    public static event Action s_TriggerRushed;
    public static event Action s_TriggerDragged;
    public static event Action s_PreEndOfBar;
    public static event Action s_MusicStopped;

    [SerializeField]
    private MMScaleShaker _speakerWiggle;

    [SerializeField, ReadOnly]
    private bool _checkBeatEvents;
    [SerializeField, ReadOnly]
    private static int s_lastBeat = 0;
    [SerializeField, ReadOnly]
    private static FMOD.StringWrapper s_lastMarker = new FMOD.StringWrapper();

    [StructLayout(LayoutKind.Sequential)]
    public class MusicData
    {
        public int CurrentBeat = 0;
        public FMOD.StringWrapper LastMarker = new FMOD.StringWrapper();
    }

    private void Awake()
    {
        if (_musicEvent.IsNull == false)
        {
            SetupFMOD();
        }

        _checkBeatEvents = false;
    }

    public void SetBeatEventCheck(bool checkEvents)
    {
        _checkBeatEvents = checkEvents;
    }

    private void Update()
    {
        if ((_checkBeatEvents == true) && (_musicData.CurrentBeat != s_lastBeat))
        {
            s_lastBeat = _musicData.CurrentBeat;
            s_BeatEvent?.Invoke();
            StartCoroutine(InvokeActionAfterSeconds(0.3f, s_TriggerDragged));

            _speakerWiggle.Play();

            if (_musicData.CurrentBeat == 2) 
            {
                s_MidBar?.Invoke();
            }

            if (_musicData.CurrentBeat == 4)
            {
                s_EndBar?.Invoke();
            }
        }

        if ((_checkBeatEvents == true) && (_musicData.LastMarker != s_lastMarker))
        {
            s_lastMarker = _musicData.LastMarker;

            string stringMarker = (string)s_lastMarker;

            if (stringMarker.Contains("Rushed"))
            {
                s_TriggerRushed?.Invoke();
            }

            if (stringMarker.Contains("PreEOB"))
            {
                s_PreEndOfBar?.Invoke();
            }
        }
    }

    private IEnumerator InvokeActionAfterSeconds(float seconds, Action actionName)
    {
        yield return new WaitForSeconds(seconds);
        actionName?.Invoke();
    }

    private void SetupFMOD()
    {
        _musicInstance = FMODUnity.RuntimeManager.CreateInstance(_musicEvent);

        _musicData = new MusicData();
        _musicCallback = new FMOD.Studio.EVENT_CALLBACK(ProcessBeatCallback);
        _musicHandle = GCHandle.Alloc(_musicData, GCHandleType.Pinned);

        _musicInstance.setUserData(GCHandle.ToIntPtr(_musicHandle));
        _musicInstance.setCallback(_musicCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
    }

    static FMOD.RESULT ProcessBeatCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        IntPtr musicDataPtr;
        FMOD.RESULT result = instance.getUserData(out musicDataPtr);

        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError($"Music Callback Error: {result}");
            return FMOD.RESULT.ERR_UNSUPPORTED;
        }

        if (musicDataPtr == IntPtr.Zero)
        {
            Debug.LogError($"Music Pointer is null: {musicDataPtr}");
            return FMOD.RESULT.ERR_UNSUPPORTED;
        }

        GCHandle musicHandle = GCHandle.FromIntPtr(musicDataPtr);
        MusicData musicData = (MusicData)musicHandle.Target;

        switch (type)
        {
            case EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                {
                    var markerParameter = (TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure<TIMELINE_MARKER_PROPERTIES>(parameterPtr);
                    musicData.LastMarker = markerParameter.name;
                    break;
                }
            case EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                {
                    var parameter = (TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure<TIMELINE_BEAT_PROPERTIES>(parameterPtr);
                    musicData.CurrentBeat = parameter.beat;
                    break;
                }
            case EVENT_CALLBACK_TYPE.ALL:
                break;
        }
        
        return FMOD.RESULT.OK;
    }

    public void SkipToDestinationMarker(MusicMarkerName markerName)
    {
        _musicInstance.setParameterByNameWithLabel("MusicStateParameter", markerName.ToString());
    }

    public override void StartMechanism()
    {
        base.StartMechanism();
        _musicInstance.setParameterByNameWithLabel("MusicStateParameter", "MusicIntro");
        _musicInstance.start();
    }

    public override void StopMechanism()
    {
        base.StopMechanism();
        _musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        s_MusicStopped?.Invoke();
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
        _musicInstance.setUserData(IntPtr.Zero);
        _musicInstance.release();
        
        if (_musicHandle.IsAllocated)
        {
            _musicHandle.Free();
        }
    }
}
