using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Text moneyValueText;
    [SerializeField] private Text notifText;

    private int prevMoney = -1;

    private void OnEnable()
    {
        notifText.text = "";
        
        Shop.onEnterStoreRange += SendNotification;
        Shop.onExitStoreRange += WipeNotification;
    }

    private void OnDisable()
    {    
        Shop.onEnterStoreRange -= SendNotification;
        Shop.onExitStoreRange -= WipeNotification;
    }

    private void Update()
    {
        if (playerStats.money != prevMoney)
        {
            moneyValueText.text = playerStats.money.ToString();
            prevMoney = playerStats.money;
        }
    }

    private void SendNotification(string message)
    {
        notifText.text = message;
    }

    private void WipeNotification()
    {
        notifText.text = "";
    }
}
