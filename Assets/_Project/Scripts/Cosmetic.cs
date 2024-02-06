using System.Collections;
using UnityEngine;

public class Cosmetic : MonoBehaviour
{
    public enum CosmeticType { Hat, Item };

    [SerializeField] 
    private CosmeticType _type;

    private Rigidbody _rigidBody;
    private Animator _animator;

    void Awake()
    {
        _rigidBody = this.GetComponent<Rigidbody>();
        _animator = this.GetComponentInChildren<Animator>();
    }

    public void EquipCosmetic(CrowdMember crowdMember)
    {
        // Fix for variety of hat heights
        this.transform.position = new Vector3(crowdMember.transform.position.x, transform.position.y, crowdMember.transform.position.z);

        if (_type == CosmeticType.Hat)
        {
            this.transform.Rotate(new Vector3(0, 1, 0), 30);
            _animator.SetBool("HatOn", true);
        }

        if (_type == CosmeticType.Item)
        {
            this.transform.position += new Vector3(0.5f, 0.7f, 0.2f);
        }
    }

    public void Throw()
    {
        _rigidBody.isKinematic = false;

        if (_type == CosmeticType.Hat)
        {
            _rigidBody.AddForce(new Vector3(UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(8, 12), UnityEngine.Random.Range(-0.5f, -2)), ForceMode.Impulse);
            _rigidBody.AddTorque(new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(1, 3), UnityEngine.Random.Range(0.5f, 1)), ForceMode.Impulse);
        }

        if (_type == CosmeticType.Item)
        {
            _rigidBody.AddForce(new Vector3(UnityEngine.Random.Range(-2, 2), UnityEngine.Random.Range(8, 11), UnityEngine.Random.Range(3, 5)), ForceMode.Impulse);
            _rigidBody.AddTorque(new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0, UnityEngine.Random.Range(0.5f, 1)), ForceMode.Impulse);
        }

        // start deactivate coroutine
        StartCoroutine(DeactivateOverTime(6f));
    }

    public IEnumerator DeactivateOverTime(float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(this.gameObject);
    }

    public CosmeticType GetCosmeticType()
    {
        return _type;
    }
}
