using UnityEngine;

public class ToggleLights : MonoBehaviour
{
    [SerializeField]
    private Light[] lights;

    public void ToggleAllLights() 
    { 
        foreach (Light light in lights) 
        {
            light.enabled = !light.enabled;
        }
    }
}
