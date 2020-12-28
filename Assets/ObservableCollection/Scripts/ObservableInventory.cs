using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;

public class ObservableInventory : Inventory
{
    public ObservableCollection<ICollectible> collectibles { get; } = new ObservableCollection<ICollectible>();
    public override void Add(ICollectible collectible)
    {
        collectibles.Add(collectible);
    }

}