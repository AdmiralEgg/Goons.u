using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class StagePositionPoint : MonoBehaviour
{
    public enum StagePosition
    {
        OffStageLeft,
        Left,
        LeftMid,
        Middle,
        RightMid,
        Right,
        OffStageRight1,
        OffStageRight2
    }

    [SerializeField]
    private StagePosition _stagePosition;
    [SerializeField, ReadOnly]
    private SphereCollider _sphereCollider;

    [SerializeField]
    private float _boundsRadius = 1f;

    public event Action<Collider> OnEnterBounds;

    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        _sphereCollider.radius = _boundsRadius;
        _sphereCollider.isTrigger = true;
    }

    public StagePosition GetStagePosition()
    {
        return _stagePosition;
    }

    public Vector3 GetPositionValue()
    {
        return this.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        OnEnterBounds?.Invoke(other);
    }
}
