using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

public class ShopUIController : UIStorageController
{
    private static ShopUIController instance;
    private static readonly object padlock = new object();

    public static ShopUIController Instance
    {
        get
        {
            lock (padlock)
            {
                return instance;
            }
        }
    } 

    public List<InventorySlot> selectedSlots = new List<InventorySlot>();
    public List<InventorySlot> ShopItems = new List<InventorySlot>();
    
    private VisualElement root;
    private VisualElement slotContainer;
    private Button buyButton;
    private Button exitButton;
    private Label totalBuyText;
    private Label shopNameText;
    private Label highlightedItemText;
    
    private static InventorySlot originalSlot;

    [SerializeField] private PlayerStats playerStats;

    private UIDocument shopDocument;
    private IShop activeShop;

    private void Awake()
    {
        instance = this;
        shopDocument = GetComponent<UIDocument>();
        shopDocument.enabled = false;
    }

    private void Update()
    {
        if(totalBuyText != null)
            totalBuyText.text = CalculateTotalPrice(selectedSlots, true).ToString() + "€";
    }

    public override void InitializeMenu(IShop shop)
    {
        base.InitializeMenu(shop);
        
        if (shopDocument.enabled == false)
        {
            shopDocument.enabled = true;
            menuOpen = true;
        }
        else
        {
            menuOpen = false;
            shopDocument.enabled = false;
        }
        
        //Store the root from the UI Document component
        root = shopDocument.rootVisualElement;

        if (root == null)
            return;
        
        slotContainer = root.Q<VisualElement>("ShopSlotContainer");
        buyButton = root.Q<Button>("BuyButton");
        exitButton = root.Q<Button>("ShopExitButton");
        totalBuyText = root.Q<Label>("TotalBuyText");
        shopNameText = root.Q<Label>("ShopName");
        highlightedItemText = root.Q<Label>("ShopHighlightedItemText");
            
        ClearMenu(selectedSlots, ShopItems, slotContainer);
        
        for (int i = 0; i < 16; i++)
        {
            InventorySlot item = new InventorySlot();
            item.owner = this;
            item.isShopView = true;
            
            ShopItems.Add(item);
            slotContainer.Add(item);
        }
        
        OnShopChanged();

        totalBuyText.text = "0" + "€";
        shopNameText.text = activeShop.ShopName;
        UpdateCurrentHighlightedSlot("", 1, 1);

        buyButton?.RegisterCallback<ClickEvent>(ev => OnTradeButtonClick(activeShop, selectedSlots, playerStats, totalBuyText, true));
        exitButton?.RegisterCallback<ClickEvent>(ev => OnExitButtonClick(selectedSlots, ShopItems, slotContainer, shopDocument));
    }

    public void OnShopChanged()
    {
        activeShop.BuyingItems = SortItems.Sort(activeShop.BuyingItems, InventoryUIController.Instance.sortingState, true);
        
        //Reset the ui first
        foreach (var item in ShopItems)
        {
            item.DropItem();
        }
        
        foreach (PlayerStats.FItemData item in activeShop.BuyingItems)
        {
            var emptySlot = ShopItems.FirstOrDefault(x => x.itemData == null);
                        
            if (emptySlot != null)
            {
                emptySlot.HoldItem(item);
            }        
        }
    }
    
    public void SetCurrentShop(IShop activeShop)
    {
        this.activeShop = activeShop;
        InitializeMenu(null);
        InventoryUIController.Instance.InitializeMenu(activeShop);
        InventoryManager.Instance.InventoryChanged();
    }
    
    public override void UpdateCurrentHighlightedSlot(string name, int sliderQuantity, int itemQuantity)
    {
        base.UpdateCurrentHighlightedSlot(name, sliderQuantity, itemQuantity);

        if (sliderQuantity > 1)
        {
            highlightedItemText.text = $"{name} ({sliderQuantity})";
        }
        else
        {
            highlightedItemText.text = $"{name}";
        }
    }
}

