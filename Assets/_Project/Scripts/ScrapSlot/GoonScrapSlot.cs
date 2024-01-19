using Sirenix.OdinInspector;
using UnityEngine;

public class GoonScrapSlot : ScrapSlot
{
    [SerializeField, ReadOnly]
    private BulbController _attachedBulb;

    void Start()
    {
        _attachedBulb = GetComponentInChildren<BulbController>();
    }

    public BulbController GetAttachedBulb()
    {
        return _attachedBulb;
    }

    public override void SetScrapAttachedState(Scrap scrap)
    {
        scrap.SetScrapAttachedState(Scrap.ScrapAttachedState.Goon);
    }
}
