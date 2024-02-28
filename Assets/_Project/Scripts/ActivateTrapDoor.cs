using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using System.Collections;
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

    private void Awake()
    {
        _trapDoorState = TrapDoorState.Closed;
        _banging = false;
        SetupFMOD();
    }

    private void SetupFMOD()
    {
        _trapDoorInstance = FMODUnity.RuntimeManager.CreateInstance(_trapDoorTouch);
    }

    public void OpenTrapDoor()
    {
        this.GetComponentInChildren<Rigidbody>().AddForce(new Vector3(0, 500, 0));
        _trapDoorState = TrapDoorState.Open;
    }

    public void CloseTrapDoor()
    {
        this.GetComponentInChildren<Rigidbody>().AddForce(new Vector3(0, 0, 1000));
        _trapDoorState = TrapDoorState.Closed;
    }

    public void OnClickedTrigger()
    {
        if (_banging == true) return;
        if (_trapDoorState == TrapDoorState.Open) return;

        _banging = true;
        
        Debug.Log("clicked trapdoor");
        _trapDoorInstance.start();

        StartCoroutine(BangOnTrapDoor());
        StartCoroutine(WaitForSound());
    }

    private IEnumerator BangOnTrapDoor()
    {
        for (int i = 0; i < 3; i++) 
        {
            Debug.Log("BANG.");
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
