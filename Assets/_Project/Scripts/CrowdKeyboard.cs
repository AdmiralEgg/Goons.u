using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using MoreMountains.Tools;
using System.Collections;
using static UnityEngine.Rendering.DebugUI.Table;

public class CrowdKeyboard : MonoBehaviour
{
    // And, Or
    [SerializeField]
    private EventReference _noteWordAndEvent, _noteWordOrEvent;
    [SerializeField]
    private EventReference _noteAEvent, _noteCEvent, _noteGEvent;

    private EventInstance _noteWordAndInst, _noteWordOrInst, _noteAInst, _noteCInst, _noteGInst;

    [SerializeField]
    private CrowdKeyData _crowdA, _crowdC, _crowdG, _crowdAnd, _crowdOr;
    private List<CrowdKeyData> _allCrowdKeys;

    [SerializeField, AssetsOnly]
    private GameObject _crowdSpotlightPrefab;

    [SerializeField, ReadOnly]
    private List<GameObject> _allSpotlights;

    [SerializeField, ReadOnly]
    private bool _isKeyboardOn;

    private void Awake()
    {
        _isKeyboardOn = false;
        _allCrowdKeys = new List<CrowdKeyData>();
        SetupFMOD();
    }

    private void Start()
    {
        _allCrowdKeys.Add(_crowdA);
        _allCrowdKeys.Add(_crowdC);
        _allCrowdKeys.Add(_crowdG);
        _allCrowdKeys.Add(_crowdAnd);
        _allCrowdKeys.Add(_crowdOr);

        InitiliseSpotlights();
        InitiliseKeySounds();

        TurnOffLightsAndDeactiveCrowdKeys();
    }

    void OnEnable()
    {
        // populate crowd members
        // initialise spotlights

        MusicButtonRunMechanism.MusicMechanismRunStateUpdate += (type, state) =>
        {
            if (state == true)
            {
                StartKeyboard();
            }

            if (state == false)
            {
                StopKeyboard();
            }
        };
    }

    private void OnDisable()
    {
        ReleaseFMOD();
        TurnOffLightsAndDeactiveCrowdKeys();

        MusicButtonRunMechanism.MusicMechanismRunStateUpdate -= (type, state) => { };
    }

    public void StartKeyboard()
    {
        TurnOnLightsAndActiveCrowdKeys();
        _isKeyboardOn = true;
    }

    public void StopKeyboard()
    {
        TurnOffLightsAndDeactiveCrowdKeys();
        _isKeyboardOn = false;
    }

    private void InitiliseSpotlights()
    {
        foreach (CrowdKeyData crowd in _allCrowdKeys)
        {
            GameObject light = Instantiate(_crowdSpotlightPrefab);

            Vector3 crowdPosition = crowd.transform.position;
            Light lightComponent = light.GetComponent<Light>();
            lightComponent.transform.position = new Vector3(crowdPosition.x, crowdPosition.y + 3, crowdPosition.z - 0.5f);
            lightComponent.transform.LookAt(crowd.transform);

            light.transform.parent = crowd.transform;
            _allSpotlights.Add(light);
        }
    }

    private void InitiliseKeySounds()
    {
        _crowdA.SetKeySound(_noteAInst);
        _crowdC.SetKeySound(_noteCInst);
        _crowdG.SetKeySound(_noteGInst);
        _crowdAnd.SetKeySound(_noteWordAndInst);
        _crowdOr.SetKeySound(_noteWordOrInst);
    }

    private void TurnOnLightsAndActiveCrowdKeys()
    {
        // turn on one by one
        StartCoroutine(LightsOnInSequence(_allSpotlights));

        foreach (CrowdKeyData crowd in _allCrowdKeys)
        {
            crowd.IsCrowdKey = true;
        }
        // play sound?
    }

    private IEnumerator LightsOnInSequence(List<GameObject> lights)
    {
        foreach (GameObject light in lights)
        {
            light.GetComponent<Light>().enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void TurnOffLightsAndDeactiveCrowdKeys()
    {
        // turn them all off
        _allSpotlights.ForEach(light => light.GetComponent<Light>().enabled = false);

        foreach (CrowdKeyData crowd in _allCrowdKeys)
        {
            crowd.IsCrowdKey = false;
        }

        // play sound?
    }

    private void SetupFMOD()
    {
        _noteWordAndInst = FMODUnity.RuntimeManager.CreateInstance(_noteWordAndEvent);
        _noteWordOrInst = FMODUnity.RuntimeManager.CreateInstance(_noteWordOrEvent);
        _noteAInst = FMODUnity.RuntimeManager.CreateInstance(_noteAEvent);
        _noteCInst = FMODUnity.RuntimeManager.CreateInstance(_noteCEvent);
        _noteGInst = FMODUnity.RuntimeManager.CreateInstance(_noteGEvent);
    }

    private void OnDestroy()
    {
        TurnOffLightsAndDeactiveCrowdKeys();
        ReleaseFMOD();
    }

    private void ReleaseFMOD()
    {
        _noteWordAndInst.release();
        _noteWordOrInst.release();
        _noteAInst.release();
        _noteCInst.release();
        _noteGInst.release();
    }
}