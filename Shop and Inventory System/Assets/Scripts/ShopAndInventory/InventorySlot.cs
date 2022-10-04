using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class InventorySlot : VisualElement
{
    public UIStorageController owner;
    
    public Image icon;
    public Label priceText;
    public Label quantityText;
    public VisualElement topHalf;
    public VisualElement botHalf;
    public SliderInt quantitySlider;
    
    public PlayerStats.FItemData itemData;
    public bool isShopView = false;

    private bool isSelected = false;
    
    public InventorySlot()
    {
        itemData = null;

        topHalf = new VisualElement();
        botHalf = new VisualElement();
        icon = new Image();
        priceText = new Label();
        quantityText = new Label();
        quantitySlider = new SliderInt();
        
        Add(icon);
        icon.Add(topHalf);
        icon.Add(botHalf);
        topHalf.Add(quantitySlider);
        botHalf.Add(quantityText);
        botHalf.Add(priceText);
        
        AddUSSToElements();
        
        priceText.text = "";
        quantityText.text = "";

        quantitySlider.style.visibility = Visibility.Hidden;
        quantitySlider.style.display = DisplayStyle.None;
        
        RegisterCallback<PointerDownEvent>(OnPointerDown);
        RegisterCallback<PointerOverEvent>(OnPointerOver);
        RegisterCallback<PointerOutEvent>(OnPointerOut);

        void AddUSSToElements()
        {
            //Add USS style properties to the elements
            icon.AddToClassList("slotIcon");
            topHalf.AddToClassList("slotIconPropertiesContainer");
            botHalf.AddToClassList("slotIconPropertiesContainer");
            priceText.AddToClassList("slotPriceText");
            priceText.AddToClassList("slotText");
            quantityText.AddToClassList("slotText");
            quantityText.AddToClassList("slotQuantityText");
            quantitySlider.AddToClassList("itemQuantitySlider");
            AddToClassList("slotContainer");
        }
    }
    
    public void HoldItem(PlayerStats.FItemData item)
    {
        itemData = item;
        icon.image = item.item.icon.texture;

        quantitySlider.lowValue = 1;
        quantitySlider.highValue = itemData.quantity;

        if (itemData.quantity > 1)
        {
            quantityText.text = itemData.quantity.ToString();
        }
        else
        {
            quantitySlider.style.visibility = Visibility.Hidden;
            quantitySlider.style.display = DisplayStyle.None;
        }
        
        if (isShopView)
        {
            if (owner as ShopUIController)
                priceText.text = itemData.item.baseBuyValue.ToString() + "€";
            
            if(owner as InventoryUIController)
                priceText.text = itemData.item.baseSellValue.ToString() + "€";
        }
    }
    
    public void DropItem()
    {
        itemData = null;
        icon.image = null;
        priceText.text = "";
        quantityText.text = "";
        quantitySlider.value = 1;
        quantitySlider.style.visibility = Visibility.Hidden;
        quantitySlider.style.display = DisplayStyle.None;
    }

    private void OnPointerOver(PointerOverEvent evt)
    {
        if(itemData != null)
            owner.UpdateCurrentHighlightedSlot(itemData.name, quantitySlider.value, itemData.quantity);
    }
    
    private void OnPointerOut(PointerOutEvent evt)
    {
        owner.UpdateCurrentHighlightedSlot("", 1, 1);
    }
    
    private void OnPointerDown(PointerDownEvent evt)
    {
        //Not the left mouse button
        if (evt.button != 0 || itemData == null)
        {
            return;
        }

        if (!isShopView)
        {
            //Clear the image
            icon.image = null;
            priceText.text = "";
            quantityText.text = "";
            
            //Start the drag
            InventoryUIController.StartDrag(evt.position, this);
        }
        else
        {
            if (owner as ShopUIController)
            {
                ShopUIController shop = owner as ShopUIController;
                
                if (!shop.selectedSlots.Contains(this))
                {
                    SetSelection();
                    shop.selectedSlots.Add(this);
                }
                else
                {
                    SetSelection();
                    shop.selectedSlots.Remove(this);
                }
            }
            
            if (owner as InventoryUIController)
            {
                InventoryUIController inventory = owner as InventoryUIController;

                if (!inventory.selectedSlots.Contains(this))
                {
                    SetSelection();
                    inventory.selectedSlots.Add(this);
                }
                else
                {
                    SetSelection();
                    inventory.selectedSlots.Remove(this);
                }
            }
            
            //only show slider if more than 1 exists and selected
            if(itemData.quantity > 1 && isSelected)
            {
                quantitySlider.style.visibility = Visibility.Visible;
                quantitySlider.style.display = DisplayStyle.Flex;
            }        
            else
            {
                quantitySlider.style.visibility = Visibility.Hidden;
                quantitySlider.style.display = DisplayStyle.None;
            }
            
            //if shop item and stackable show slider up to x value
            if(itemData.item.stackable && isSelected && owner as ShopUIController)
            {
                quantitySlider.highValue = 20;
                quantitySlider.style.visibility = Visibility.Visible;
                quantitySlider.style.display = DisplayStyle.Flex;
            }
        }
    }

    public void SetSelection()
    {
        if (icon.tintColor == Color.white)
        {
            icon.tintColor = Color.yellow;
            isSelected = true;
        }
        else if (icon.tintColor == Color.yellow)
        {
            icon.tintColor = Color.white;
            isSelected = false;
        }
    }

    public int GetQuantitySliderValue()
    {
        return quantitySlider.value;
    }

    #region UXML
    [Preserve]
    public new class UxmlFactory : UxmlFactory<InventorySlot, UxmlTraits> { }
    [Preserve]
    public new class UxmlTraits : VisualElement.UxmlTraits { }
    #endregion
}
