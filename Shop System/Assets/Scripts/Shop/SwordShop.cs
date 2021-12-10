using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordShop : Shop
{
    [SerializeField] private GameObject shopCanvas;
    [SerializeField] private List<int> shopLevels;
    
    private void Start()
    {
        ShopLevels = shopLevels;
        InitializeShop(Item.type.sword, this);
    }

    public override void InitializeShop(Item.type type, IShop activeShop)
    {
        base.InitializeShop(type, activeShop);
    }

    public override void Buy(List<Item> itemsToBuy, PlayerStats buyerStats)
    {
        base.Buy(itemsToBuy, buyerStats);
    }    
    
    public override void Sell(List<Item> itemsToSell, PlayerStats sellerStats)
    {
        base.Sell(itemsToSell, sellerStats);
    }

    public override void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            base.OnTriggerEnter(other);
        }
    }
    
    public override void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            base.OnTriggerExit(other);
        }
    }
    public override void OpenMenu(GameObject shopUI)
    {
        base.OpenMenu(shopUI);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canOpenShop)
        {
            OpenMenu(shopCanvas);
        }
    }
}
