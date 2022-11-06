using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUIController : UIStorageController
{
    private static InventoryUIController instance;
    private static readonly object padlock = new object();

    public static InventoryUIController Instance
    {
        get
        {
            lock (padlock)
            {
                return instance;
            }
        }
    }
    
    public enum SortingState
    {
        Price,
        Level,
        Quantity,
        Type,
    }

    public SortingState sortingState;

    public List<InventorySlot> selectedSlots = new List<InventorySlot>();
    public List<InventorySlot> InventoryItems = new List<InventorySlot>();
    
    private VisualElement root;
    private VisualElement slotContainer;
    private static VisualElement ghostIcon;
    private Label moneyText;
    private Label spaceText;
    private Button sellButton;
    private Button exitButton;
    private Label totalSellText;
    private Label highlightedItemText;
    private DropdownField sortingOptions;
    
    private static bool m_IsDragging;
    private static InventorySlot m_OriginalSlot;

    [SerializeField] private PlayerStats playerStats;
    
    private IShop activeShop;
    private UIDocument document;

    private void Awake()
    {
        instance = this;
        document = GetComponent<UIDocument>();
        document.enabled = false;

        InventoryManager.OnInventoryChanged += OnInventoryChanged;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I) && document.visualTreeAsset.name != "ShopScreen")
            InitializeMenu(null);
        
        if (moneyText != null)
            moneyText.text = playerStats.money.ToString() + "€";

        if (spaceText != null)
            spaceText.text = playerStats.GetInventorySpaceText();

        if (totalSellText != null)
            totalSellText.text = CalculateTotalPrice(selectedSlots, false).ToString() + "€";
    }

    public override void InitializeMenu(IShop shop)
    {
        base.InitializeMenu(shop);
        
        if (shop != null)
        {
            activeShop = shop;
        }
        else
        {
            if (document.enabled == false)
            {
                document.enabled = true;
                menuOpen = true;
            }
            else
            {
                menuOpen = false;
                document.enabled = false;
            }
        }

        //Store the root from the UI Document component
        root = document.rootVisualElement;

        if (root == null)
            return;
        
        //Search the root for the SlotContainer Visual Element
        slotContainer = root.Q<VisualElement>("SlotContainer");
        ghostIcon = root.Q<VisualElement>("GhostIcon");
        sellButton = root.Q<Button>("SellButton");
        exitButton = root.Q<Button>("ExitButton");
        totalSellText = root.Q<Label>("TotalSellText");
        highlightedItemText = root.Q<Label>("HighlightedItemText");
        sortingOptions = root.Q<DropdownField>("SortOptions");
              
        //Clear everything before initializing
        ClearMenu(selectedSlots, InventoryItems, slotContainer);
        
        //Create InventorySlots and add them as children to the SlotContainer
        for (int i = 0; i < 24; i++)
        {
            InventorySlot item = new InventorySlot();
            item.owner = this;
            
            if (activeShop != null)
            {
                item.isShopView = true;
            }
            else
            {
                item.isShopView = false;
            }

            InventoryItems.Add(item);
            slotContainer.Add(item);
        }
        

        moneyText = root.Q<Label>("Money");
        spaceText = root.Q<Label>("Space");
        UpdateCurrentHighlightedSlot("", 0, 0);

        moneyText.text = playerStats.money.ToString() + "€";
        spaceText.text = playerStats.GetInventorySpaceText();

        sortingOptions.RegisterValueChangedCallback(SetSortingState);
        ghostIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        ghostIcon.RegisterCallback<PointerUpEvent>(OnPointerUp);
   
        if(activeShop == null)
        {
            totalSellText.style.visibility = Visibility.Hidden;
            totalSellText.style.display = DisplayStyle.None;
            
            sellButton.style.visibility = Visibility.Hidden;
            sellButton.style.display = DisplayStyle.None;
            
            exitButton.style.visibility = Visibility.Visible;
            exitButton.style.display = DisplayStyle.Flex;
            exitButton?.RegisterCallback<ClickEvent>(ev => OnExitButtonClick(selectedSlots, InventoryItems, slotContainer, document));
        }
        else
        {
            totalSellText.style.visibility = Visibility.Visible;
            totalSellText.style.display = DisplayStyle.Flex;
            
            sellButton.style.visibility = Visibility.Visible;
            sellButton.style.display = DisplayStyle.Flex;
            
            exitButton.style.visibility = Visibility.Hidden;
            exitButton.style.display = DisplayStyle.None;
            sellButton?.RegisterCallback<ClickEvent>(ev => OnTradeButtonClick(activeShop, selectedSlots, playerStats, totalSellText, false));
        }
        
        SetSortingState(null);
    }

    private void SetSortingState(ChangeEvent<string> evt)
    {
        string sortOption = sortingOptions.value;

        if (sortOption == "Price")
            sortingState = SortingState.Price;
        else if (sortOption == "Level")
            sortingState = SortingState.Level;
        else if (sortOption == "Quantity")
            sortingState = SortingState.Quantity;
        else if (sortOption == "Type")
            sortingState = SortingState.Type;
            
        InventoryManager.Instance.InventoryChanged();
        
        //ugly solution: rework in future
        if(activeShop != null)
            ShopUIController.Instance.OnShopChanged();
    }
    
    private void OnInventoryChanged(List<PlayerStats.FItemData> inventory)
    {
        inventory = SortItems.Sort(inventory, sortingState, false);
        
        //Reset the ui first
        foreach (var item in InventoryItems)
        {
            item.DropItem();
        }
        
        //Loop through each item and if it has been picked up, add it to the next empty slot
        foreach (PlayerStats.FItemData item in inventory)
        {
            var emptySlot = InventoryItems.FirstOrDefault(x => x.itemData == null);
                        
            if (emptySlot != null)
            {
                emptySlot.HoldItem(item);
            }        
        }
    }
    
    public static void StartDrag(Vector2 position, InventorySlot originalSlot)
    {
        //Set tracking variables
        m_IsDragging = true;
        m_OriginalSlot = originalSlot;

        //Set the new position
        ghostIcon.style.top = position.y - ghostIcon.layout.height / 2;
        ghostIcon.style.left = position.x - ghostIcon.layout.width / 2;

        //Set the image
        ghostIcon.style.backgroundImage = originalSlot.itemData.item.icon.texture;

        //Flip the visibility on
        ghostIcon.style.visibility = Visibility.Visible;
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        //Only take action if the player is dragging an item around the screen
        if (!m_IsDragging)
        {
            return;
        }

        //Set the new position
        ghostIcon.style.top = evt.position.y - ghostIcon.layout.height / 2;
        ghostIcon.style.left = evt.position.x - ghostIcon.layout.width / 2;

    }

    private void OnPointerUp(PointerUpEvent evt)
    {
        if (!m_IsDragging)
        {
            return;
        }

        //Check to see if they are dropping the ghost icon over any inventory slots.
        IEnumerable<InventorySlot> slots = InventoryItems.Where(x => 
            x.worldBound.Overlaps(ghostIcon.worldBound));

        SetSlot();

        //Clear dragging related visuals and data
        m_IsDragging = false;
        m_OriginalSlot = null;
        ghostIcon.style.visibility = Visibility.Hidden;

        void SetSlot()
        {
            //found atleast one
            if (slots.Count() != 0)
            {
                InventorySlot closestSlot = slots.OrderBy(x => Vector2.Distance
                    (x.worldBound.position, ghostIcon.worldBound.position)).First();

                //If item is dropped on itself set to self
                if (closestSlot.itemData == m_OriginalSlot.itemData)
                {
                    closestSlot.HoldItem(m_OriginalSlot.itemData);
                    return;
                }

                //if item exists on slot, swap them instead
                if (closestSlot.itemData != null)
                {
                    PlayerStats.FItemData originalSlotItem = closestSlot.itemData;
                    
                    closestSlot.HoldItem(m_OriginalSlot.itemData);
                    m_OriginalSlot.HoldItem(originalSlotItem);
                    return;
                }
                
                //Set the new inventory slot with the data
                closestSlot.HoldItem(m_OriginalSlot.itemData);
        
                //Clear the original slot
                m_OriginalSlot.DropItem();
            }
            //Didn't find any (dragged off the window)
            else
            {
                m_OriginalSlot.icon.image = m_OriginalSlot.itemData.item.icon.texture;
            }
        }
        
    }

    public override void UpdateCurrentHighlightedSlot(string name, int sliderQuantity, int itemQuantity)
    {
        base.UpdateCurrentHighlightedSlot(name, sliderQuantity, itemQuantity);

        if (activeShop == null)
        {
            if (itemQuantity > 1)
            {
                highlightedItemText.text = $"{name} ({itemQuantity})";
            }
            else
            {
                highlightedItemText.text = $"{name}";
            }
        }
        else
        {
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
}
