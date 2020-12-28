using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortedInventory : Inventory
{
    public event Action<SortedList<SphereCollectible.Tier, ICollectible>> OnItemCollected;

    private SortedList<SphereCollectible.Tier, ICollectible> sortedList =
        new SortedList<SphereCollectible.Tier, ICollectible>(new TierComparer());

    public override void Add(ICollectible collectible)
    {
        var tier = collectible.collectibleGameObject.GetComponent<SphereCollectible>().tier;
        sortedList.Add(tier, collectible);
        OnItemCollected?.Invoke(sortedList);
    }
}