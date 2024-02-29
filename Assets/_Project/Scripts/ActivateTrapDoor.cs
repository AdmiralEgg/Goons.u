using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ActivateTrapDoor : MonoBehaviour
{
    private enum TrapDoorState { Open, Closed }
    
    [Header("Audio Data")]
    [SerializeField]
    private EventReference _trapDoorTouch;
    private FMOD.Studio.EventInstance _trapDoorInstance;

    [SerializeField, ReadOnly]
    private bool _banging;

    [SerializeField, ReadOnly]
    private TrapDoorState _trapDoorState;

    private HingeJoint _hingeJoint;

    private void Awake()
    {
        _trapDoorState = TrapDoorState.Closed;
        _hingeJoint = this.GetComponentInParent<HingeJoint>();
        _banging = false;
        SetupFMOD();
    }

    private void SetupFMOD()
    {
        _trapDoorInstance = FMODUnity.RuntimeManager.CreateInstance(_trapDoorTouch);
    }

    public void OpenTrapDoor()
    {
        _hingeJoint.useMotor = true;
        _trapDoorState = TrapDoorState.Open;
    }

    public void CloseTrapDoor()
    {
        _hingeJoint.useMotor = false;
        _trapDoorState = TrapDoorState.Closed;
    }

    public void OnClickedTrigger()
    {
        if (_banging == true) return;
        if (_trapDoorState == TrapDoorState.Open) return;

        _banging = true;
        _trapDoorInstance.start();

        StartCoroutine(BangOnTrapDoor());
        StartCoroutine(WaitForSound());
    }

    private IEnumerator BangOnTrapDoor()
    {
        for (int i = 0; i < 3; i++)
        {
            this.GetComponentInChildren<Rigidbody>().AddForce(new Vector3(0, 0, -1500));
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator WaitForSound()
    {
        // wait for the sound to start...
        yield return new WaitForSeconds(0.5f);

        PLAYBACK_STATE isPlaying;
        _trapDoorInstance.getPlaybackState(out isPlaying);

        while (isPlaying == PLAYBACK_STATE.PLAYING)
        {
            yield return new WaitForSeconds(0.5f);
            _trapDoorInstance.getPlaybackState(out isPlaying);
        }

        _banging = false;
    }

    private void OnDestroy()
    {
        _trapDoorInstance.release();
    }

    private void OnDisable()
    {
        _trapDoorInstance.release();
    }
}
