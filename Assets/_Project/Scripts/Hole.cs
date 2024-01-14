using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // if it's a scrap, delete it.
        if (other.gameObject.CompareTag("Scrap"))
        {
            Destroy(other.gameObject);
        }
    }
}
