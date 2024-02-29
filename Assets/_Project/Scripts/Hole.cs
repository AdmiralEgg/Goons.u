using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [SerializeField]
    private EventReference _fireIgniteScrapEvent, _fireIndicateEvent;
    private EventInstance _fireIgniteScrapInstance, _fireIndicateInstance;

    [SerializeField]
    private ParticleSystem _fire;

    private void Awake()
    {
        _fireIgniteScrapInstance = FMODUnity.RuntimeManager.CreateInstance(_fireIgniteScrapEvent);
        _fireIndicateInstance = FMODUnity.RuntimeManager.CreateInstance(_fireIndicateEvent);
        
        Scrap.ScrapSelected += (scrap) =>
        {
            Debug.Log("Hole got a changed input state.");

            StartCoroutine(PlayFire(0.5f, _fireIndicateInstance));
        };

        Scrap.ScrapDestroyed += (Scrap) =>
        {
            StartCoroutine(PlayFire(0.5f, _fireIgniteScrapInstance));
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit hole trigger");
        
        // if it's a scrap, delete it.
        if (other.gameObject.CompareTag("Scrap"))
        {
            Destroy(other.gameObject);
        }
    }

    private IEnumerator PlayFire(float duration, EventInstance eventInstance)
    {
        _fire.Play();

        eventInstance.start();
        yield return new WaitForSeconds(duration);

        _fire.Stop();
    }

    private void OnDestroy()
    {
        _fireIgniteScrapInstance.release();
        _fireIndicateInstance.release();
    }

    private void OnDisable()
    {
        _fireIgniteScrapInstance.release();
        _fireIndicateInstance.release();
    }
}
