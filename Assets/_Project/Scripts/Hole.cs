using System.Collections;
using UnityEngine;

public class Hole : MonoBehaviour
{
    private ParticleSystem _fire;
    
    private void Awake()
    {
        _fire = GetComponentInChildren<ParticleSystem>();
        _fire.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        // if it's a scrap, delete it.
        if (other.gameObject.CompareTag("Scrap"))
        {
            Destroy(other.gameObject);
            StartCoroutine(PlayFire(0.2f));
        }
    }

    private IEnumerator PlayFire(float duration)
    {
        _fire.Play();

        yield return new WaitForSeconds(duration);

        _fire.Stop();
    }
}
