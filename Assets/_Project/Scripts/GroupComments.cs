using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

public class GroupComments : MonoBehaviour
{
    private enum GroupCommentType { ScrapCaught, CrowdTouch, ScrapSelected, SpeakerTouch }
    
    [SerializeField, ReadOnly]
    private List<Goon> _allGoons;

    private Goon _lastCommentedGoon = null;

    [SerializeField, ReadOnly]
    private Hashtable _recordedTouches;

    private int _scrapCaughtThreshold, _crowdTouchThreshold, _speakerTouchThreshold;

    [SerializeField]
    private EventReference _scrapGroupEvent, _crowdGroupEvent, _speakerGroupEvent;
    private EventInstance _scrapGroupInstance, _crowdGroupInstance, _speakerGroupInstance;

    void Awake()
    {
        _scrapCaughtThreshold = UnityEngine.Random.Range(3, 7);
        _crowdTouchThreshold = UnityEngine.Random.Range(7, 14);
        _speakerTouchThreshold = UnityEngine.Random.Range(3, 5);

        SetupFMOD();
        
        Scrap.ScrapCaught += (scrap) =>
        {
            PlayComment(GroupCommentType.ScrapCaught);
        };

        Scrap.ScrapSelected += (scrap) =>
        {
            PlayComment(GroupCommentType.ScrapSelected);
        };

        _recordedTouches = new Hashtable();
        InputManager.EnvironmentTouch += RecordTouch;

        RefreshAllGoons();
    }

    private void SetupFMOD()
    {
        _scrapGroupInstance = FMODUnity.RuntimeManager.CreateInstance(_scrapGroupEvent);
        _crowdGroupInstance = FMODUnity.RuntimeManager.CreateInstance(_crowdGroupEvent);
        _speakerGroupInstance = FMODUnity.RuntimeManager.CreateInstance(_speakerGroupEvent);
    }

    private void RecordTouch(string touchTag)
    {        
        if (_recordedTouches.ContainsKey(touchTag))
        {
            _recordedTouches[touchTag] = (int)_recordedTouches[touchTag] + 1;
            UnityEngine.Debug.Log($"Added new touch: {touchTag}. Recorded value is now: {_recordedTouches[touchTag]}");
        }
        else
        {
            _recordedTouches.Add(touchTag, 1);
        }

        switch (touchTag)
        {
            case "Scarp":
                if ((int)_recordedTouches[touchTag] > _scrapCaughtThreshold)
                {
                    PlayComment(GroupCommentType.ScrapCaught);
                    _recordedTouches[touchTag] = 0;
                }
                break;
            case "Speaker":
                if ((int)_recordedTouches[touchTag] > _speakerTouchThreshold)
                {
                    PlayComment(GroupCommentType.SpeakerTouch);
                    _recordedTouches[touchTag] = 0;
                }
                break;
            case "Crowd":
                if ((int)_recordedTouches[touchTag] > _crowdTouchThreshold)
                {
                    PlayComment(GroupCommentType.CrowdTouch);
                    _recordedTouches[touchTag] = 0;
                }
                break;
        }
    }

    private void PlayComment(GroupCommentType commentType)
    {
        switch (commentType)
        {
            case GroupCommentType.ScrapCaught:
                _scrapGroupInstance.start();
                break;
            case GroupCommentType.CrowdTouch:
                _crowdGroupInstance.start();
                break;
            case GroupCommentType.SpeakerTouch:
                _speakerGroupInstance.start();
                break;
        }
    }

    private Goon SelectRandomGoon()
    {
        RefreshAllGoons();

        List<Goon> goonList = _allGoons;
        List<Goon> returnList = new List<Goon>();

        // Remove the goon which spoke last
        goonList.Remove(_lastCommentedGoon);

        // Find goons which aren't speaking and add to the return list
        foreach (Goon goon in goonList)
        {
            if (goon.GetGoonState() != Goon.GoonState.Speaking)
            {
                returnList.Add(goon);
            }
        }

        if (returnList.Count == 0) return null;

        // Randomise between the goons which are left
        int randomGoon = Random.Range(1, (returnList.Count));
        
        return goonList[randomGoon - 1];
    }

    private void RefreshAllGoons()
    {
        _allGoons.Clear();
        
        foreach (Goon goon in GetComponentsInChildren<Goon>())
        {
            _allGoons.Add(goon);
        }
    }

    private void OnDisable()
    {
        ReleaseFMOD();
    }

    private void OnDestroy()
    {
        ReleaseFMOD();
    }

    private void ReleaseFMOD()
    {
        _scrapGroupInstance.release();
        _speakerGroupInstance.release();
        _crowdGroupInstance.release();
    }
}
