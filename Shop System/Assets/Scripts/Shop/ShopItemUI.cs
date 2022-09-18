using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour, IInteractableUIItem
{
    public Item item;

    public enum SlotState
    {
        Active, Empty
    }

    public SlotState slotState;
    
    public enum TradeState
    {
        Buying, Selling
    }

    public TradeState tradeState;
    
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text tradeValueText;
    [SerializeField] private Text itemLevelText;
    [SerializeField] private Text itemQuantityText;
    [SerializeField] private PlayerStats playerStats;
    
    public Toggle toggle;
    
    public bool isSelected = false;

    private void Start()
    {
        toggle = GetComponent<Toggle>();
    }

    public void OnPress()
    {
        ChangeToggleSelection();
        ShopUI.Instance.totalCostsDirtyFlag = true;
    }

    public void SetItem(Item item, SlotState slotState, TradeState tradeState)
    {
        this.item = item;
        
        if (slotState == SlotState.Active)
        {
            switch (tradeState)
            {
                case TradeState.Buying:
                    this.tradeState = tradeState;
                    tradeValueText.text = item.baseBuyValue.ToString();
                    break;
                    
                case TradeState.Selling:
                    this.tradeState = tradeState;
                    tradeValueText.text = item.baseSellValue.ToString();
                    itemQuantityText.text = playerStats.GetItem(item).quantity.ToString();
                    break;                
            }
            
            this.slotState = slotState;
            itemIcon.sprite = item.icon;
            itemLevelText.text = item.itemLevel.ToString();
            GetComponent<Toggle>().interactable = true;
        }
        else if (slotState == SlotState.Empty)
        {
            this.slotState = slotState;
            itemIcon.sprite = null;
            tradeValueText.text = "";
            itemLevelText.text = "";
            itemQuantityText.text = "";
            GetComponent<Toggle>().interactable = false;
        }
    }

    private void UpdateToggleColors(Toggle t)
    {
        if (t.isOn)
        {
            ColorBlock cb = t.colors;
            cb.normalColor = new Color(35f/255f, 253f/255f, 198f/255f);
            cb.selectedColor = new Color(35f/255f, 253f/255f, 198f/255f);
            t.colors = cb;   
        }
        else
        {
            ColorBlock cb = t.colors;
            cb.normalColor = new Color(243f/255f, 97f/255f, 97f/255f);
            cb.selectedColor = new Color(243f/255f, 97f/255f, 97f/255f);
            t.colors = cb;
        }
    }

    public void ChangeToggleSelection()
    {
        if (toggle.isOn)
        {
            isSelected = true;
        }
        else
        {
            isSelected = false;
        }
        
        UpdateToggleColors(toggle);
    }
}