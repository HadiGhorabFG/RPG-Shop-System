using System.Collections.Generic;
using UnityEngine;

public class FoodShop : ShopInWorld
{
    [SerializeField] private GameObject shopCanvas;
    [SerializeField] private List<int> shopLevels;
    
    private void Start()
    {
        ShopLevels = shopLevels;
        InitializeShop(Item.type.food, this);
    }

    public override void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(TagConsts.playerTag))
        {
            base.OnTriggerEnter(other);
        }
    }
    
    public override void OnTriggerExit(Collider other)
    {
        if(other.CompareTag(TagConsts.playerTag))
        {
            base.OnTriggerExit(other);
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canOpenShop)
        {
            OpenMenu(shopCanvas);
        }
    }
}
