using UnityEngine;

public class ProjectorEnableMechanism : BaseEnableMechanism
{
    [Header("Camera")]
    [SerializeField]
    private GameObject _projectorCamera;

    public override void EnableAfterAnimation()
    {
        _projectorCamera.SetActive(true);
        base.EnableAfterAnimation();
    }

    public override void DisableAfterAnimation()
    {
        base.DisableAfterAnimation();
        _projectorCamera.SetActive(false);
    }
}