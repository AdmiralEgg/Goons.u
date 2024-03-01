using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfBarLightFlash : MonoBehaviour
{
    [SerializeField]
    private Light _light;

    void Awake()
    {
        SpeakerRunMechanism.s_PreEndOfBar += LightOff;
        SpeakerRunMechanism.s_EndBar += LightOn;
        SpeakerRunMechanism.s_MusicStopped += LightOn;
    }

    private void LightOff()
    {
        _light.enabled = false;
    }

    private void LightOn()
    {
        _light.enabled = true;
    }
}
