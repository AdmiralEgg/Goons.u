using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using MoreMountains.Feedbacks;

public class GoonMove : MonoBehaviour
{
    enum GoonMoveState { Idle, Walking, Dancing, Bowing };
    
    [Header("Animation Control")]
    [SerializeField]
    private Animator _goonStickAnimator;
    [SerializeField]
    private MMWiggle _idleWiggle;

    [Header("Goon position on stage values")]
    [SerializeField]
    private StagePositionPoint _awakePosition;
    [SerializeField, ReadOnly]
    private StagePositionPoint _currentPosition;
    [SerializeField]
    private StagePositionPoint _targetPosition;
    [SerializeField]
    private StagePositionPoint _offStagePosition;
    [SerializeField, ReadOnly]
    private GoonMoveState _currentMoveState;

    [SerializeField]
    float _moveSpeed = 3f;
    [SerializeField, ReadOnly]
    Vector3 velocity;

    void Awake()
    {
        if (_awakePosition != null)
        {
            this.transform.position = _awakePosition.GetPositionValue();
            _currentPosition = _awakePosition;
        }
        else
        {
            Debug.LogError($"Missing position {_awakePosition.name} from the Stage Position Points list");
        }

        _currentMoveState = GoonMoveState.Idle;

        Speaker.MusicStopped += GoonIdle;
        PointsManager.PointsReached += GoonBow;
    }

    private void FixedUpdate()
    {
        // if targetposition is updated, stop the coroutine
        if ((_currentPosition != _targetPosition) && (_currentMoveState != GoonMoveState.Walking))
        {
            // start the walk animation
            _goonStickAnimator.Play("Walk");
            _idleWiggle.enabled = false;

            _currentMoveState = GoonMoveState.Walking;
            StartCoroutine(MoveToTarget());
        }
    }

    private IEnumerator MoveToTarget()
    {
        // when we're close, stop walking
        _targetPosition.OnEnterBounds += EnteredTargetBounds;

        // initial rotation
        Quaternion initial = this.transform.rotation;

        // Calculate target rotation based on whether we're walking left or right
        float walkRotationY = 35f;
        
        if (initial.x > _targetPosition.GetPositionValue().x)
        {
            walkRotationY = walkRotationY * -1;
        }
        
        Quaternion target = Quaternion.Euler(10, walkRotationY, 0);

        while (_currentMoveState == GoonMoveState.Walking)
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
    }

    private void EnteredTargetBounds(Collider collider)
    {
        if (this == collider.GetComponentInParent<GoonMove>())
        {
            _goonStickAnimator.Play("Idle");
            _currentPosition = _targetPosition;
            _idleWiggle.enabled = true;
            _currentMoveState = GoonMoveState.Idle;

            _targetPosition.OnEnterBounds -= EnteredTargetBounds;
        }
    }

    public void GoonDance()
    {        
        _idleWiggle.enabled = false;
        _goonStickAnimator.applyRootMotion = false;
        _goonStickAnimator.Play("DanceLeftRight");

        _currentMoveState = GoonMoveState.Dancing;
    }

    public void GoonIdle()
    {
        _idleWiggle.enabled = true;
        _goonStickAnimator.applyRootMotion = true;
        this.transform.position = _currentPosition.GetPositionValue();
        _goonStickAnimator.Play("Idle");

        _currentMoveState = GoonMoveState.Idle;
    }

    public void GoonBow()
    {
        Debug.Log("Goon bowing...");
        
        _idleWiggle.enabled = false;
        _goonStickAnimator.applyRootMotion = false;
        this.transform.position = _currentPosition.GetPositionValue();
        _goonStickAnimator.Play("Bow");

        _currentMoveState = GoonMoveState.Bowing;
    }

    public void GoonOffstage()
    {
        _targetPosition = _offStagePosition;
    }

    public void SetTargetPosition(StagePositionPoint target)
    {
        _targetPosition = target;
    }
}