using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BulbController : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private Material _bulbMaterial;

    private void Awake()
    {
        _bulbMaterial = GetComponent<MeshRenderer>().material;

        SetBulbEmissionColor(Color.blue);
    }

    public void SetBulbEmissionColor(Color emissionColor)
    {
        _bulbMaterial.SetColor("_EmissionColor", emissionColor);
    }

    public void SetBulbActive(bool active)
    {
        if (active == true)
        {
            _bulbMaterial.EnableKeyword("_EMISSION");
        }
        else
        {
            _bulbMaterial.DisableKeyword("_EMISSION");
        }
    }

    private void OnClickedTrigger()
    {
        Debug.Log("Clicked a bulb.");
    }
}

