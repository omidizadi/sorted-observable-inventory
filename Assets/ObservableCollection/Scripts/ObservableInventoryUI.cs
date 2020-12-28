using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;

public class ObservableInventoryUI : MonoBehaviour
{
    [SerializeField] private Transform collectiblesParent;
    [SerializeField] private CubeCollectible.Type type;
    private ObservableInventory observableInventory;

    private void Start()
    {
        observableInventory = (ObservableInventory) Inventory.Instance;
        observableInventory.collectibles.CollectionChanged += UpdateInventoryUI;
    }

    private void UpdateInventoryUI(object sender, NotifyCollectionChangedEventArgs e)
    {
        var newCollected = (CubeCollectible) e.NewItems[0];
        if (newCollected.type != type) return;
        foreach (Transform tr in collectiblesParent)
        {
            if (tr.gameObject.activeSelf) continue;
            tr.gameObject.SetActive(true);
            break;
        }
    }
}