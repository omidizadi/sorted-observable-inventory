using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCollectible : MonoBehaviour,ICollectible
{
    public enum Type
    {
        RED,
        BLUE
    }

    private IInventory inventory;
    public Type type;



    private void Start()
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