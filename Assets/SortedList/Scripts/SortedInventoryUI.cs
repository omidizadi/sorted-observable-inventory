using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortedInventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject[] items;
    [SerializeField] private Color[] tierColors;
    private SortedInventory sortedInventory;
    

    // Start is called before the first frame update
    void Start()
    {
        sortedInventory = (SortedInventory) Inventory.Instance;
        sortedInventory.OnItemCollected += UpdateInventoryUI;
    }

    private void UpdateInventoryUI(SortedList<SphereCollectible.Tier, ICollectible> sortedList)
    {
        var index = 0;
        foreach (var item in sortedList)
        {
            items[index].GetComponent<Image>().color = tierColors[(int)item.Key];
            items[index].gameObject.SetActive(true);
            index++;
        }
    }
}