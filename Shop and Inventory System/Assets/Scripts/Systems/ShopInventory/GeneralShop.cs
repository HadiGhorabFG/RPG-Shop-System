using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GeneralShop : ShopInWorld
{
    [SerializeField] private string shopName;
    [SerializeField] private List<int> shopLevels;
    
    private void Start()
    {
        ShopLevels = shopLevels;
        ShopName = shopName;
        InitializeShop(Item.type.general, this);
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
            OpenMenu();
        }
    }
}
