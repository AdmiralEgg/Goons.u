using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using MoreMountains.Feedbacks;
using UnityEngine.InputSystem.LowLevel;
using System;

public class GoonMove : MonoBehaviour
{
    public enum GoonMoveState { Idle, Walking, Dancing, Bowing };

    [Header("Animation Control")]
    [SerializeField]
    private Animator _goonStickAnimator;

    [Header("Goon position on stage values")]
    [SerializeField]
    private StagePositionPoint _offStagePosition;
    [SerializeField, ReadOnly]
    private StagePositionPoint _currentPosition;
    [SerializeField]
    private StagePositionPoint _targetPosition;
    [SerializeField, ReadOnly]
    private GoonMoveState _currentMoveState;

    [SerializeField]
    private string _danceMove = "DanceLeftRight";

    [SerializeField]
    private float _moveSpeed = 3f;
    
    [SerializeField, ReadOnly]
    private Vector3 velocity;

    public static Action<StagePositionPoint, bool> SpotlightSwitchOn;

    private void Awake()
    {
        if (_offStagePosition != null)
        {
            this.transform.position = _offStagePosition.GetPositionValue();
            _currentPosition = _offStagePosition;
        }
        else
        {
            Debug.LogError($"Missing off stage position {_offStagePosition.name} from the Stage Position Points list");
        }

        _currentMoveState = GoonMoveState.Idle;

        Speaker.MusicStopped += GoonIdle;
        PointsManager.PointsReached += GoonBow;
        //InputManager.InventoryScrapClicked += GoonProd;

        SpeakerRunMechanism.s_BeatEvent += GoonDance;
        SpeakerRunMechanism.s_MusicStopped += GoonIdle;
    }

    private void Update()
    {
        // if targetposition is updated, stop the coroutine
        if ((_currentPosition != _targetPosition) && (_currentMoveState != GoonMoveState.Walking))
        {
            SpotlightSwitchOn(_currentPosition, false);
            
            // start the walk animation
            _goonStickAnimator.Play("Walk");
            _currentMoveState = GoonMoveState.Walking;

            StartCoroutine(MoveToTarget());
        }
    }

    private IEnumerator MoveToTarget()
    {
        // initial rotation
        Quaternion initial = this.transform.rotation;

        // Calculate target rotation based on whether we're walking left or right
        float walkRotationY = 35f;

        if (initial.x > _targetPosition.GetPositionValue().x)
        {
            walkRotationY = walkRotationY * -1;
        }

        Quaternion target = Quaternion.Euler(10, walkRotationY, 0);

        while (_targetPosition != _currentPosition)
        {
            transform.rotation = Quaternion.RotateTowards(this.transform.rotation, target, 6);
            transform.position = Vector3.SmoothDamp(transform.position, _targetPosition.GetPositionValue(), ref velocity, smoothTime: 0.2f, _moveSpeed);
            yield return null;
        }

        // while we aren't facing audience, turn
        while (transform.rotation != initial)
        {
            transform.rotation = Quaternion.RotateTowards(this.transform.rotation, initial, 0.5f);
            yield return null;
        }

        GoonIdle();
        SpotlightSwitchOn(_currentPosition, true);
    }

    public void GoonIdle()
    {
        if (_currentMoveState == GoonMoveState.Bowing) return;
        
        _goonStickAnimator.Play("Idle");
        _currentMoveState = GoonMoveState.Idle;
    }

    public void GoonBow()
    {
        Debug.Log("Bow activated");
        _goonStickAnimator.StopPlayback();
        _goonStickAnimator.Play("Bow");
        _currentMoveState = GoonMoveState.Bowing;
    }

    public void GoonDance()
    {
        if ((_currentMoveState == GoonMoveState.Bowing) || (_currentMoveState == GoonMoveState.Walking)) return;
        
        _goonStickAnimator.Play(_danceMove);
        _currentMoveState = GoonMoveState.Dancing;
    }

    public void GoonProd()
    {
        _goonStickAnimator.Play("Prod", _goonStickAnimator.GetLayerIndex("ProdLayer"));
    }

    public void GoonOffstage()
    {
        _targetPosition = _offStagePosition;
    }

    public void SetTargetPosition(StagePositionPoint target)
    {
        _targetPosition = target;
    }

    public void NewStagePosition(StagePositionPoint newPosition)
    {
        _currentPosition = newPosition;
    }

    /*
    private void OnGoonSelected(GameObject gameObject)
    {
        _goonStickAnimator.Play("Prod", _goonStickAnimator.GetLayerIndex("ProdLayer"));
    }
    */
}