using FMOD.Studio;
using Sirenix.OdinInspector;
using UnityEngine;

public class CrowdKeyData : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private bool _isCrowdKey = false;

    public bool IsCrowdKey
    {
        get { return _isCrowdKey;}
        set { _isCrowdKey = value;}
    }

    [SerializeField]
    private EventInstance _keyInstance;

    public void CrowdKeyPressed()
    {
        Debug.Log("Crowd key pressed");
        _keyInstance.start();
    }

    public void SetKeySound(EventInstance instance)
    {
        _keyInstance = instance;
    }
}