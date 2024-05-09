using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigureSettings : MonoBehaviour
{
    void Awake()
    {
        RuntimePlatform platform = Application.platform;

        switch (platform)
        {
            case RuntimePlatform.Android:
                Debug.Log("Running on Android");
                Screen.SetResolution(2100, 900, true);
                break;
            case RuntimePlatform.WindowsPlayer:
                Debug.Log("Running on Windows");
                Screen.SetResolution(1878, 805, true);
                break;
        }
    }
}
