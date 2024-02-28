using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class ProjectorEnableMechanism : BaseEnableMechanism
{
    [Header("Camera")]
    [SerializeField]
    private GameObject _projectorCamera;

    [SerializeField]
    private EventReference _movingEvent;
    private EventInstance _movingInstance;

    private bool _isMovingSoundPlaying;

    public override void Awake()
    {
        base.Awake();
        _isMovingSoundPlaying = false;
        _movingInstance = FMODUnity.RuntimeManager.CreateInstance(_movingEvent);
    }

    public override void EnableAfterAnimation()
    {
        _projectorCamera.SetActive(true);
        base.EnableAfterAnimation();
    }

    public override void DisableAfterAnimation()
    {
        base.DisableAfterAnimation();
        _projectorCamera.SetActive(false);
    }

    private void Update()
    {
        // if InTransition, play sound
        if ((CurrentEnabledState == EnabledState.InTransition) && (_isMovingSoundPlaying == false))
        {
            _movingInstance.start();
            _isMovingSoundPlaying = true;
        }

        if ((CurrentEnabledState != EnabledState.InTransition) && (_isMovingSoundPlaying == true))
        {
            _movingInstance.stop(STOP_MODE.ALLOWFADEOUT);
            _isMovingSoundPlaying = false;
        }
    }

    private void OnDestroy()
    {
        _movingInstance.stop(STOP_MODE.ALLOWFADEOUT);
        _movingInstance.release();
    }

    private void OnDisable()
    {
        _movingInstance.stop(STOP_MODE.ALLOWFADEOUT);
        _movingInstance.release();
    }
}