using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CrowdMember : MonoBehaviour
{
    public enum Timing { Dragging, OnBeat, Rushing, Random }

    public enum Intensity { None, Low, Medium, High }

    [SerializeField, Tooltip("Vigor when bouncing to the beat")]
    private Intensity _currentIntensity;

    [Header("Cosmetics")]
    [SerializeField, ReadOnly]
    private Cosmetic _activeHat;
    [SerializeField, ReadOnly]
    private Cosmetic _activeItem;

    [Header("Animations")]
    private MMPositionShaker _positionShaker;
    private MMRotationShaker _rotationShaker;
    private MMScaleShaker _scaleShaker;

    [SerializeField, ReadOnly]
    private Timing _timing = Timing.OnBeat;

    [SerializeField, ReadOnly]
    Coroutine _bounceCoroutine;

    public UnityEvent BounceTrigger;

    void Awake()
    {        
        _positionShaker = GetComponent<MMPositionShaker>();
        _positionShaker.enabled = false;
        _rotationShaker = GetComponent<MMRotationShaker>();
        _rotationShaker.enabled = false;
        _scaleShaker = GetComponent<MMScaleShaker>();
        _scaleShaker.enabled = false;

        BounceTrigger.AddListener(Bounce);

        // Randomise their timing
        float i = Random.Range(0f, 1f);
        
        switch (i)
        {
            case float x when (x <= 0.15):
                _timing = Timing.Dragging;
                SpeakerRunMechanism.s_TriggerDragged += Bounce;
                break;
            case float x when (x >= 0.95):
                _timing = Timing.Random;
                SpeakerRunMechanism.s_BeatEvent += Bounce;
                break;
            case float x when (x >= 0.85):
                _timing = Timing.Rushing;
                SpeakerRunMechanism.s_TriggerRushed += Bounce;
                break;
            default:
                _timing = Timing.OnBeat;
                SpeakerRunMechanism.s_BeatEvent += Bounce;
                break;
        }
    }

    private IEnumerator GoWild(float totalTime)
    {
        float t = 0;

        while (t < totalTime) 
        {
            Bounce();
            
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.2f, 0.6f));
            t += Time.deltaTime;
        }
    }

    public void Bounce()
    {
        if (_timing == Timing.Random)
        {
            StartCoroutine(RandomBounce());
            return;
        }
        
        _positionShaker.Play();
    }

    private IEnumerator RandomBounce()
    {
        float randomWait = UnityEngine.Random.Range(0.1f, 0.4f);
        yield return new WaitForSeconds(randomWait);
        _positionShaker.Play();
    }

    public void SetMemberIntensity(Intensity newIntensity)
    {
        if (_bounceCoroutine != null)
        {
            StopCoroutine(_bounceCoroutine);
            _bounceCoroutine = null;
        }
        
        switch (newIntensity)
        {
            case Intensity.None:
                _positionShaker.ShakeSpeed = 10;
                _positionShaker.ShakeRange = 0.2f;
                _positionShaker.ShakeMainDirection = new Vector3(0, 1, 0);
                break;
            case Intensity.Low:
                _positionShaker.ShakeSpeed = 12;
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
                
                if (_bounceCoroutine == null)
                {
                    _bounceCoroutine = StartCoroutine(GoWild(3f));
                }
                
                break;
        }

        _currentIntensity = newIntensity;
    }

    public void ResetCosmetics()
    {
        // Get a random hat or item from the pool (50% chance)
        float rand = Random.Range(minInclusive: 0, maxInclusive: 1f);

        if (rand > 0.5f)
        {
            Cosmetic cosmetic = CosmeticsPool.s_Instance.Pool.Get();

            if (cosmetic.GetCosmeticType() == Cosmetic.CosmeticType.Hat)
            {
                _activeHat = cosmetic;
                _activeHat.gameObject.transform.SetParent(this.transform);
                _activeHat.EquipCosmetic(this);
            }

            if (cosmetic.GetCosmeticType() == Cosmetic.CosmeticType.Item)
            {
                _activeItem = cosmetic;
                _activeItem.gameObject.transform.SetParent(this.transform);
                _activeItem.EquipCosmetic(this);
            }
        }
    }

    public void ResetMember()
    {
        SetMemberIntensity(Intensity.None);
    }

    // Throw hats, throw items
    public void ThrowCosmetics()
    {
        if (_activeHat != null) 
        {
            _activeHat.Throw();
        }

        if (_activeItem != null)
        {
            _activeItem.Throw();
        }
    }

    public Timing GetTiming()
    {
        return _timing;
    }

    private void OnClickedTrigger()
    {
        _scaleShaker.Play();
        _rotationShaker.Play();

        // If we're a debug crowd member
        if (GetComponent<CrowdDebug>() != null) 
        {
            gameObject.SendMessageUpwards("CrowdDebugPressed");
        }

        // If they're part of the keyboard
        if (GetComponent<CrowdKeyData>().IsCrowdKey) 
        {
            gameObject.SendMessageUpwards("CrowdKeyPressed", this);
        }
        else
        {
            gameObject.SendMessageUpwards("CrowdPoked", this);
        }
    }
}
