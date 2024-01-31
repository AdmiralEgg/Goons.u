using UnityEngine;

public class ActivateTrapDoor : MonoBehaviour
{
    void Awake()
    {
        
    }

    public void OpenTrapDoor()
    {
        this.GetComponentInChildren<Rigidbody>().AddForce(new Vector3(0, 500, 0));
    }

    public void CloseTrapDoor()
    {
        this.GetComponentInChildren<Rigidbody>().AddForce(new Vector3(0, 0, 1000));
    }
}
