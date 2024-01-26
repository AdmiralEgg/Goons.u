using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor.Experimental.GraphView;
using MoreMountains.Feedbacks;
using static StagePositionPoint;
using Unity.Android.Types;

public class GoonMove : MonoBehaviour
{
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
    [SerializeField, ReadOnly]
    private bool _isWalking;

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
    }

    private void Update()
    {
        // Update CurrentPostion

    }

    private void FixedUpdate()
    {
        // if targetposition is updated, stop the coroutine
        
        if ((_currentPosition != _targetPosition) && _isWalking == false)
        {
            // start the walk animation
            _goonStickAnimator.Play("Walk");
            _idleWiggle.enabled = false;

            _isWalking = true;
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
        
        if (initial.x < _targetPosition.GetPositionValue().x)
        {
            walkRotationY = walkRotationY * -1;
        }
        
        Quaternion target = Quaternion.Euler(10, walkRotationY, 0);

        while (_isWalking)
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
            _isWalking = false;

            _targetPosition.OnEnterBounds -= EnteredTargetBounds;
        }
    }
}