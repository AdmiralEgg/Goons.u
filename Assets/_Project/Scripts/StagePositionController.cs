using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StagePositionController : MonoBehaviour
{
    public enum StagePosition
    {
        None,
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
    private List<StagePositionReference> _positionReferences;

    private void Awake()
    {
        // set stage position enums
        foreach (StagePositionReference reference in _positionReferences)
        {
            reference.PositionPoint.SetStagePosition(reference.StagePosition);
        }
    }

    public StagePositionPoint GetStagePositionPoint(StagePosition position)
    {
        var positionReference = _positionReferences.FirstOrDefault<StagePositionReference>(p => p.StagePosition == position);
        return positionReference.PositionPoint;
    }
}

[Serializable]
public class StagePositionReference
{
    public StagePositionController.StagePosition StagePosition;
    public StagePositionPoint PositionPoint;
}
