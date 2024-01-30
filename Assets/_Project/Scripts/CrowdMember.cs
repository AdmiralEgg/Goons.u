using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CrowdMember : MonoBehaviour
{
    public enum Timing { Dragging, OnBeat, Rushing, Random }

    public enum Intensity { Low, Medium, High }

    [SerializeField, Tooltip("Vigor when bouncing to the beat")]
    private Intensity _currentIntensity;

    [Header("Cosmetics")]
    [SerializeField, Tooltip("Hats to be thrown up")]
    private GameObject[] _hats;
    [SerializeField, Tooltip("Items to be thrown on stage")]
    private GameObject[] _items;
    [SerializeField, ReadOnly]
    private GameObject _activeHat;
    [SerializeField, ReadOnly]
    private Rigidbody _activeHatRigidbody;
    [SerializeField, ReadOnly]
    private GameObject _activeItem;
    [SerializeField, ReadOnly]
    private Rigidbody _activeItemRigidbody;

    [Header("Animations")]
    private MMPositionShaker _positionShaker;
    private MMRotationShaker _rotationShaker;

    [SerializeField, ReadOnly]
    private Timing _timing = Timing.OnBeat;

    public UnityEvent BounceTrigger;

    void Awake()
    {
        _currentIntensity = Intensity.Low;
        
        _positionShaker = GetComponent<MMPositionShaker>();
        _positionShaker.enabled = false;
        _rotationShaker = GetComponent<MMRotationShaker>();
        _rotationShaker.enabled = false;

        BounceTrigger.AddListener(Bounce);

        // Randomise their timing
        float i = Random.Range(0f, 1f);
        
        switch (i)
        {
            case float x when (x <= 0.15):
                _timing = Timing.Dragging;
                break;
            case float x when (x >= 0.95):
                _timing = Timing.Random;
                break;
            case float x when (x >= 0.85):
                _timing = Timing.Rushing;
                break;
            default:
                _timing = Timing.OnBeat;
                break;
        }

        // Randomise a hat
        float hatRand = Random.Range(0f, 1f);

        if (hatRand > 0.85)
        {
            _activeHat = _hats[Random.Range(0, _hats.Length)];

            _activeHatRigidbody = _activeHat.AddComponent<Rigidbody>();
            _activeHatRigidbody.isKinematic = true;

            _activeHat.transform.Rotate(new Vector3(0, Random.Range(-10,10),0));
            _activeHat.SetActive(true);
        }

        // Randomise an item
        float itemRand = Random.Range(0f, 1f);

        if (itemRand > 0.85)
        {
            _activeItem = _items[Random.Range(0, _items.Length)];

            _activeItemRigidbody = _activeItem.AddComponent<Rigidbody>();
            _activeItemRigidbody.isKinematic = true;

            _activeItem.transform.Rotate(new Vector3(0, Random.Range(-10, 10), 0));
            _activeItem.SetActive(true);
        }
    }

    public void Bounce()
    {
        switch (_currentIntensity)
        {
            case Intensity.Low:
                _positionShaker.ShakeSpeed = 10;
                _positionShaker.ShakeRange = 0.2f;
                _positionShaker.ShakeMainDirection = new Vector3(0, 1, 0);
                break;
            case Intensity.Medium:
                _positionShaker.ShakeSpeed = 15;
                _positionShaker.ShakeRange = 0.4f;
                _positionShaker.ShakeMainDirection = new Vector3(0, 1, 0);
                break;
            case Intensity.High:
                _positionShaker.ShakeSpeed = 20;
                _positionShaker.ShakeRange = 0.5f;
                _positionShaker.ShakeMainDirection = new Vector3(0.1f, 1, 0);
                break;
        }
        
        _positionShaker.Play();
    }

    // Throw hats, throw items
    public void FinalCheer()
    {
        if (_activeHat != null)
        {
            _activeHatRigidbody.isKinematic = false;
            _activeHatRigidbody.AddForce(new Vector3(UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(8, 12), UnityEngine.Random.Range(-0.5f, -2)), ForceMode.Impulse);
            _activeHatRigidbody.AddTorque(new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(1, 3), UnityEngine.Random.Range(0.5f, 1)), ForceMode.Impulse);
        }

        if (_activeItemRigidbody != null)
        {
            Debug.Log("Throw item!");
            _activeItemRigidbody.isKinematic = false;
            _activeItemRigidbody.AddForce(new Vector3(UnityEngine.Random.Range(-2, 2), UnityEngine.Random.Range(8, 11), UnityEngine.Random.Range(3, 5)), ForceMode.Impulse);
            _activeItemRigidbody.AddTorque(new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0, UnityEngine.Random.Range(0.5f, 1)), ForceMode.Impulse);
        }
    }

    public Timing GetTiming()
    {
        return _timing;
    }

    public void SetCurrentIntensity(Intensity newIntensity)
    {
        _currentIntensity = newIntensity;
    }
}
