using Sirenix.OdinInspector;
using UnityEngine;

public class InventoryScrapSlot : ScrapSlot
{
    public override void SetScrapAttachedState(Scrap scrap)
    {
        scrap.SetScrapAttachedState(Scrap.ScrapAttachedState.Inventory);
    }
}