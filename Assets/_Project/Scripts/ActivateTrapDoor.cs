using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateTrapDoor : MonoBehaviour
{
    void Awake()
    {
        
    }

    public void OpenTrapDoor()
    {
        this.GetComponent<Rigidbody>().AddForce(new Vector3(0, 500, 0));
    }

    public void CloseTrapDoor()
    {
        this.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 1000));
    }
}
