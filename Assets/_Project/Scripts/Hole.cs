using System.Collections;
using UnityEngine;

public class Hole : MonoBehaviour
{
    private ParticleSystem _fire;

    [SerializeField]
    private AudioClip _igniteScrap;
    [SerializeField]
    private AudioClip _flame;

    private AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        
        Scrap.ScrapSelected += (scrap) =>
        {
            Debug.Log("Hole got a changed input state.");

            StartCoroutine(PlayFire(0.5f, _flame));
        };

        Scrap.ScrapDestroyed += (Scrap) =>
        {
            StartCoroutine(PlayFire(0.5f, _igniteScrap));
        };

        _fire = GetComponentInChildren<ParticleSystem>();
        _fire.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        // if it's a scrap, delete it.
        if (other.gameObject.CompareTag("Scrap"))
        {
            Destroy(other.gameObject);
        }
    }

    private IEnumerator PlayFire(float duration, AudioClip clip)
    {
        _fire.Play();

        _source.PlayOneShot(clip);
        yield return new WaitForSeconds(duration);

        _fire.Stop();
    }
}
