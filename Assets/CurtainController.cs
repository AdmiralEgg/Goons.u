using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurtainController : MonoBehaviour
{
    [SerializeField]
    private GameObject _curtainLeft, _curtainRight;

    private Vector3 _drawnPositionLeft;
    private Vector3 _openPositionLeft = new Vector3(-13f, 4f, -2.2f);

    private Vector3 _drawnPositionRight;
    private Vector3 _openPositionRight = new Vector3(13f, 4f, -2.2f);

    void Awake()
    {
        _curtainLeft.transform.position = _openPositionLeft;
        _curtainRight.transform.position = _openPositionRight;
    }
}
