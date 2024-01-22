using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ProjectorMechanism : BaseMechanism
{

    public static Action VideoPlaybackComplete;

    public override void StartMechanism()
    {
        base.StartMechanism();
    
        StartCoroutine(PlayVideo(GetComponent<VideoPlayer>()));
    }

    public IEnumerator PlayVideo(VideoPlayer player)
    {
        player.Play();

        while (player.isPlaying == true)
        {
            yield return new WaitForSeconds(1);
        }

        VideoPlaybackComplete?.Invoke();
        StopMechanism();

        yield return null;
    }

    public override void StopMechanism()
    {
        base.StopMechanism();
        GetComponent<VideoPlayer>().Stop();
        
        DisableAfterAnimation();
    }
}
