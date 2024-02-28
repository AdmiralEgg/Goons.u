using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using System.Collections;
using FMODUnity;
using FMOD.Studio;

public class StagePositionPoint : MonoBehaviour
{
    [SerializeField, ReadOnly, Tooltip("Set by StagePositionController on Awake")]
    private StagePositionController.StagePosition _stagePosition = StagePositionController.StagePosition.None;
    
    [SerializeField, ReadOnly]
    private SphereCollider _sphereCollider;

    [SerializeField]
    private EventReference _spotlightOnEvent;
    private EventInstance _spotlightOnInstance;
    
    [SerializeField]
    private float _boundsRadius = 0.15f;

    [SerializeField]
    private Light[] _goonLights;

    private void Awake()
    {
        SetupFMOD();
        
        _sphereCollider = GetComponent<SphereCollider>();
        _sphereCollider.radius = _boundsRadius;
        _sphereCollider.isTrigger = true;

        foreach (Light light in _goonLights)
        {
            light.gameObject.SetActive(false);
        }

        GoonMove.SpotlightSwitchOn += SpotlightSwitch;
    }

    private void SetupFMOD()
    {
        _spotlightOnInstance = FMODUnity.RuntimeManager.CreateInstance(_spotlightOnEvent);
    }

    private void SpotlightSwitch(StagePositionPoint position, bool switchOn)
    {
        if (this != position) return;
        if (_goonLights.Count() == 0) return;

        StartCoroutine(HitTheLights(switchOn));
    }

    private IEnumerator HitTheLights(bool switchOn)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.2f, 0.3f));
        
        foreach (Light light in _goonLights)
        {
            if (switchOn)
            {
                _spotlightOnInstance.start();
            }
            
            light.gameObject.SetActive(switchOn);
            yield return new WaitForSeconds(UnityEngine.Random.Range(1.2f, 1.5f));
        }
    }

    public void SetStagePosition(StagePositionController.StagePosition stagePosition)
    {
        _stagePosition = stagePosition;
    }

    public Vector3 GetPositionValue()
    {
        return this.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Stick")
        {
            other.SendMessageUpwards("NewStagePosition", this);
        }
    }

    private void OnDestroy()
    {
        ReleaseFMOD();
    }

    private void OnDisable()
    {
        ReleaseFMOD();
    }

    private void ReleaseFMOD()
    {
        _spotlightOnInstance.release();
    }
}
