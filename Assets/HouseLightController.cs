using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using System.Collections;

public class HouseLightController : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private Light[] _houseLights;

    [Header("Light Audio")]
    [SerializeField]
    private EventReference _bigLightOnEvent, _bigLightOffEvent;
    private EventInstance _bigLightOnInstance, _bigLightOffInstance;

    void Awake()
    {
        SetupFMOD();

        _houseLights = GetComponentsInChildren<Light>();
        MusicButtonRunMechanism.HouseLights += SwitchLights;
        StageManager.HouseLights += SwitchLights;
        StageManager.HouseLightsAfterPause += HouseLightsAfterPause;
    }

    private void SetupFMOD()
    {
        _bigLightOnInstance = FMODUnity.RuntimeManager.CreateInstance(_bigLightOnEvent);
        _bigLightOffInstance = FMODUnity.RuntimeManager.CreateInstance(_bigLightOffEvent);
    }

    private void SwitchLights(bool switchOn)
    {
        foreach (Light light in _houseLights) 
        { 
            // if we aren't already in the enabled state
            if (light.enabled != switchOn)
            {
                light.enabled = switchOn;
                if (switchOn)
                {
                    _bigLightOnInstance.start();
                }
                else
                {
                    _bigLightOffInstance.start();
                } 
            }
        }
    }

    private void HouseLightsAfterPause(bool switchOn, float pause)
    {
        StartCoroutine(PauseThenSwitchLight(switchOn, pause));
    }

    public IEnumerator PauseThenSwitchLight(bool switchOn, float secondsPause)
    {
        yield return new WaitForSeconds(secondsPause);
        SwitchLights(switchOn);
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
        _bigLightOnInstance.release();
        _bigLightOffInstance.release();
    }
}
