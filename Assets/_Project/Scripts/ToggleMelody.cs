using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMelody : MonoBehaviour
{
    private enum ButtonState { On, Moving, Off };
    
    [SerializeField] 
    private AudioClip _clip;

    [SerializeField]
    private AudioSource _source;

    [SerializeField]
    private bool _startEnabled = false;

    [SerializeField, ReadOnly]
    private ButtonState _currentButtonState;

    [SerializeField]
    private Vector3 _disabledPosition = new Vector3 (5f, 7.7f, 0f);
    
    [SerializeField]
    private Vector3 _enabledPosition = new Vector3(5f, 5.8f, 0f);

    private void Awake()
    {
        // move to disabled position
        this.transform.position = _disabledPosition;
        _currentButtonState = ButtonState.Off;
    }

    private void Start()
    {
        this.gameObject.SetActive(_startEnabled);
    }

    private void OnEnable()
    {
        // move into view
        Debug.Log("Melody button enabled");

        StartCoroutine(MoveToPosition(_enabledPosition, ButtonState.On));
    }

    // Disable after animation is complete
    private void DisableAfterAnimation()
    {
        StartCoroutine(MoveToPosition(_disabledPosition, ButtonState.Off));
    }

    private IEnumerator MoveToPosition(Vector3 position, ButtonState buttonStateOnComplete)
    {
        _currentButtonState = ButtonState.Moving;

        Debug.Log("Starting move coroutine");
        
        float t = 0;
        float _moveDuration = 5.0f;
        Vector3 startPosition = this.transform.position;

        while (t < _moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, position, t / _moveDuration);
            
            t += Time.deltaTime;
            yield return null;
        }

        if (buttonStateOnComplete == ButtonState.Off) 
        {
            _currentButtonState = ButtonState.Off;
            this.gameObject.SetActive(false);
        }

        if (buttonStateOnComplete == ButtonState.On)
        {
            _currentButtonState = ButtonState.On;
            this.gameObject.SetActive(true);
        }
    }

    private void OnClickedTrigger()
    {
        // If button is moving, do nothing
        if (_currentButtonState == ButtonState.Moving) return;
        
        // Start the music
        Debug.Log("button clicked!");
        //DisableAfterAnimation();

        // Play a melody
        _source.PlayOneShot(_clip);
    }
}
