using System;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    private static UIHandler instance;
    private static readonly object padlock = new object();
    public static UIHandler Instance
    {
        get
        {
            lock (padlock)
            {
                return instance;
            }
        }
    }
    
    public bool changeMoneyDirtyFlag = false;

    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Text moneyValueText;
    [SerializeField] private Text notifText;

    //private int prevMoney = -1;

    private StoreInteractNotification storeInteractNotification;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        storeInteractNotification = GetComponent<StoreInteractNotification>();
        moneyValueText.text = playerStats.money.ToString();
    }

    private void OnEnable()
    {
        notifText.text = "";
        
        Shop.onEnterStoreRange += SendStoreInteractNotification;
        Shop.onExitStoreRange += WipeNotification;
    }

    private void OnDisable()
    {    
        Shop.onEnterStoreRange -= SendStoreInteractNotification;
        Shop.onExitStoreRange -= WipeNotification;
    }

    private void Update()
    {
        if (changeMoneyDirtyFlag)
        {
            moneyValueText.text = playerStats.money.ToString();
            changeMoneyDirtyFlag = false;
        }
    }

    private void SendStoreInteractNotification()
    {
        SendTextNotification(storeInteractNotification);
    }

    private void SendTextNotification(ITextNotification textNotification)
    {
        notifText.text = textNotification.Message;
    }

    private void WipeNotification()
    {
        notifText.text = "";
    }
}
