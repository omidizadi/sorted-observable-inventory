using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCollectible : MonoBehaviour,ICollectible
{
    public enum Tier
    {
        T1,
        T2,
        T3
    }

    public Tier tier;
    private IInventory inventory;

    void Start()
    {
        inventory = Inventory.Instance;
    }

    private void OnMouseDown()
    {
        inventory.Add(this);
        gameObject.SetActive(false);
    }
    public GameObject collectibleGameObject => gameObject;
}