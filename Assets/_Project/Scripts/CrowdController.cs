using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CrowdController : MonoBehaviour
{
    private CrowdMember[] _allCrowd;

    [SerializeField, ReadOnly]
    private double _lastClickTime;
    [SerializeField, ReadOnly]
    private double _lastClickPercentageFromBeat;

    public static Action CrowdEntertained;

    private void Awake()
    {
        _allCrowd = GetComponentsInChildren<CrowdMember>();
    }



    // Temporary trigger to entertain the crowd.
    public void OnClickedTrigger()
    {
        //Debug.Log("Crowd entertained");
        //CrowdEntertained?.Invoke();
    }
}
