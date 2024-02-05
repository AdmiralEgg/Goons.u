using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class ProjectorRunMechanism : BaseRunMechanism
{
    VideoPlayer _player;

    public static Action VideoPlaybackComplete;

    private void Awake()
    {
        _player = GetComponent<VideoPlayer>();
    }

    public override void StartMechanism()
    {
        base.StartMechanism();
        
        StartCoroutine(PlayVideo(_player));
    }

    public IEnumerator PlayVideo(VideoPlayer player)
    {
        player.Play();

        while (player.isPlaying == true)
        {
            yield return new WaitForSeconds(0.1f);
        }

        StopMechanism();

        yield return null;
    }

    public override void StopMechanism()
    {
        base.StopMechanism();
        
        // Stop the play video coroutine if it's running.
        StopAllCoroutines();

        _player.Stop();

        VideoPlaybackComplete?.Invoke();
    }

    public override void OnClickedTrigger()
    {
        base.OnClickedTrigger();
        StopMechanism();
    }
}
