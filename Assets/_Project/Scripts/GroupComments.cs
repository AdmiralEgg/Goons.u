using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GroupComments : MonoBehaviour
{
    private enum GroupCommentType
    {
        ScrapCaught,
        CrowdTouch,
        ScrapSelected,
        SpeakerTouch
    }
    
    [SerializeField, ReadOnly]
    private List<Goon> _allGoons;

    private Goon _lastCommentedGoon = null;

    void Awake()
    {        
        Scrap.ScrapCaught += (scrap) =>
        {
            PlayComment(GroupCommentType.ScrapCaught);
        };

        Scrap.ScrapSelected += (scrap) =>
        {
            PlayComment(GroupCommentType.ScrapSelected);
        };

        RefreshAllGoons();
    }

    private void PlayComment(GroupCommentType commentType)
    {
        Goon g = SelectRandomGoon();

        if (g == null) return;

        AudioClip clip = null; 

        switch (commentType)
        {
            case GroupCommentType.ScrapCaught:
                //clip = g.GetGoonData().NiceCatchAudio;
                break;
            case GroupCommentType.CrowdTouch:
                //clip = g.GetGoonData().CrowdTouchAudio;
                break;
            case GroupCommentType.ScrapSelected:
                break;
            case GroupCommentType.SpeakerTouch:
                //clip = g.GetGoonData().SpeakerTouchAudio;
                break;
        }

        if (clip == null) return;

        //g.PlayGroupComment(clip);

        if (_allGoons.Count > 1)
        {
            _lastCommentedGoon = g;
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
}
