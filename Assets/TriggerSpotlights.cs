using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSpotlights : MonoBehaviour
{
    [SerializeField]
    private Light[] _goonLights;

    void Awake()
    {
        foreach (Light light in _goonLights)
        {
            light.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Stick")
        {
            foreach (Light light in _goonLights)
            {
                light.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Stick")
        {
            foreach (Light light in _goonLights)
            {
                light.gameObject.SetActive(false);
            }
        }
    }
}
