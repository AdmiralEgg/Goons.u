using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurtainController : MonoBehaviour
{
    private enum CurtainState { Open, Closed, Opening, Closing }

    [SerializeField]
    private GameObject _curtainLeft, _curtainRight;

    private float _closedPositionLeftX = -5f;
    private float _openPositionLeftX = -13f;

    private float _closedPositionRightX = 5f;
    private float _openPositionRightX = 13f;

    [SerializeField]
    private bool _startOpen = true;

    [SerializeField, ReadOnly]
    private CurtainState _currentState;

    void Awake()
    {
        if (_startOpen)
        {
            _curtainLeft.transform.position = new Vector3(_openPositionLeftX, _curtainLeft.transform.position.y, _curtainLeft.transform.position.z);
            _curtainRight.transform.position = new Vector3(_openPositionRightX, _curtainRight.transform.position.y, _curtainRight.transform.position.z);
            _currentState = CurtainState.Open;
        }
        else
        {
            _curtainLeft.transform.position = new Vector3(_closedPositionLeftX, _curtainLeft.transform.position.y, _curtainLeft.transform.position.z);
            _curtainRight.transform.position = new Vector3(_closedPositionRightX, _curtainRight.transform.position.y, _curtainRight.transform.position.z);
            _currentState = CurtainState.Closed;
        }
    }

    private void Start()
    {
        // If they aren't open, open them
        if (_currentState == CurtainState.Closed)
        {
            StartCoroutine(OpenCurtains());
        }
    }

    private IEnumerator OpenCurtains()
    {
        _currentState = CurtainState.Opening;

        // lerp tug + wait
        float elapsedTime = 0;
        float lerpSpeed = 0.2f;

        Vector3 initialPositionLeft = _curtainLeft.transform.position;
        Vector3 targetPositionLeft = new Vector3(_openPositionLeftX, _curtainLeft.transform.position.y, _curtainLeft.transform.position.z);

        Vector3 initialPositionRight = _curtainRight.transform.position;
        Vector3 targetPositionRight = new Vector3(_openPositionRightX, _curtainRight.transform.position.y, _curtainRight.transform.position.z);

        while (elapsedTime < 5f)
        {
            Debug.Log($"t: {elapsedTime}");
            _curtainLeft.transform.position = Vector3.Lerp(initialPositionLeft, targetPositionLeft, elapsedTime);
            _curtainRight.transform.position = Vector3.Lerp(initialPositionRight, targetPositionRight, elapsedTime);
            elapsedTime += Time.deltaTime * lerpSpeed;
            yield return null;
        }

        _currentState = CurtainState.Open;
    }
}
