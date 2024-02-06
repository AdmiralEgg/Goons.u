using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class StagePositionPoint : MonoBehaviour
{
    [SerializeField, ReadOnly, Tooltip("Set by StagePositionController on Awake")]
    private StagePositionController.StagePosition _stagePosition = StagePositionController.StagePosition.None;
    [SerializeField, ReadOnly]
    private SphereCollider _sphereCollider;

    [SerializeField]
    private float _boundsRadius = 0.15f;

    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        _sphereCollider.radius = _boundsRadius;
        _sphereCollider.isTrigger = true;
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
}
