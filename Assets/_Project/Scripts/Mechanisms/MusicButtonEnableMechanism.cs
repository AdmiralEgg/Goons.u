using Sirenix.Utilities;
using UnityEngine;

public class MusicButtonEnableMechanism : BaseEnableMechanism
{
    [SerializeField]
    private Light[] _buttonLights;

    private void Update()
    {
        if (CurrentEnabledState == EnabledState.Disabled || CurrentEnabledState == EnabledState.InTransition)
        {
            _buttonLights.ForEach(light => light.enabled = false);
        }
        else
        {
            _buttonLights.ForEach(light => light.enabled = true);
        }
    }
}
